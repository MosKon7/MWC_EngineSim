using System;
using System.IO;
using CarList;
using MSCLoader;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Binds one vehicle to live Engine Sim playback
    /// </summary>
    public sealed class VehicleEngineSession
    {
        public const string LiveBackendId = "live";

        public readonly string ProfileId;
        public readonly VehicleInfo Vehicle;
        public readonly MixDebugInfo DebugInfo;

        private readonly VanillaSoundMuter _muter;
        private readonly VehicleAudioProbe _probe;
        private readonly Drivetrain _drivetrain;
        private readonly Transform _attach;
        private readonly string _engineSimRoot;
        private readonly string _liveScriptPath;

        private EngineSimLivePlayer _live;

        public VehicleEngineSession(
            string profileId,
            VehicleInfo vehicle,
            string engineSimRoot,
            string liveScriptPath,
            int simHz)
        {
            ProfileId = profileId;
            Vehicle = vehicle;
            _drivetrain = vehicle.Drivetrain as Drivetrain;
            _engineSimRoot = engineSimRoot;
            _liveScriptPath = liveScriptPath;

            _attach = ResolveAttachPoint(vehicle);
            DebugInfo = new MixDebugInfo();
            DebugInfo.ProfileId = profileId;
            DebugInfo.ActiveBank = LiveBackendId;
            DebugInfo.VehicleName = vehicle.Root != null ? vehicle.Root.name : profileId;

            _probe = new VehicleAudioProbe(vehicle.Root.transform);

            SoundController soundController = vehicle.Root.GetComponent<SoundController>();
            if (soundController == null)
                soundController = vehicle.Root.GetComponentInChildren<SoundController>();

            if (soundController == null)
            {
                SoundController[] all = UnityEngine.Object.FindObjectsOfType<SoundController>();
                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i] == null)
                        continue;
                    if (all[i].transform.root == vehicle.Root.transform)
                    {
                        soundController = all[i];
                        break;
                    }
                }
            }

            if (soundController != null)
            {
                _muter = new VanillaSoundMuter(soundController);
                ModConsole.Print("[RealisticMotorSound] SoundController on "
                    + soundController.gameObject.name
                    + " (root " + soundController.transform.root.name + ")");
            }
            else
                ModConsole.Print("[RealisticMotorSound] SoundController not found on " + vehicle.Root.name);

            ModConsole.Print("[RealisticMotorSound] Foreign audio sources found: " + _probe.ForeignCount);

            TryStartLive(simHz);
            if (_live == null || !_live.IsReady)
                ModConsole.Error("[RealisticMotorSound] Live Engine Sim failed to start for " + profileId);
        }

        /// <summary>
        /// Advances mute and live synth for the current frame
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        public void Tick(AudioRuntimeSettings settings)
        {
            if (Vehicle == null || Vehicle.Root == null || _drivetrain == null)
                return;

            ApplyMute(settings);

            float rpm = _drivetrain.rpm;
            float throttle = Mathf.Clamp01(_drivetrain.throttle);
            bool starter = _drivetrain.startEngine;
            bool engineOn = rpm > 300f && !starter;

            DebugInfo.Starter = starter;
            DebugInfo.ActiveBank = LiveBackendId;
            DebugInfo.Rpm = rpm;
            DebugInfo.Throttle01 = throttle;
            DebugInfo.EngineOn = engineOn;

            bool customEnabled = settings.Mode != AudioRuntimeSettings.SoundMode.VanillaOnly
                && settings.CustomVolume > 0.0001f;

            if (_live == null || !_live.IsReady)
            {
                DebugInfo.SimTickMs = 0f;
                DebugInfo.BufferFill01 = 0f;
                DebugInfo.ActiveClips = "LIVE unavailable";
                return;
            }

            float liveVol = customEnabled ? settings.CustomVolume : 0f;
            _live.UpdateTargets(rpm, throttle, engineOn && customEnabled, liveVol);
            _live.SetQuality(settings.SimHz);
            _live.ApplyMix(settings);
            DebugInfo.SimTickMs = _live.LastTickMs;
            DebugInfo.SimTickAvgMs = _live.AvgTickMs;
            DebugInfo.BufferFill01 = _live.BufferFill01;
            DebugInfo.UnderrunRate01 = _live.UnderrunRate01;
            DebugInfo.UnderrunFrames = _live.UnderrunFrames;
            DebugInfo.ActiveClips = "LIVE simHz=" + settings.SimHz
                + " hp=" + settings.BassCutHz.ToString("0")
                + " tick=" + _live.LastTickMs.ToString("0.0")
                + "/" + _live.AvgTickMs.ToString("0.0") + "ms"
                + " buf=" + (_live.BufferFill01 * 100f).ToString("0") + "%"
                + " und=" + (_live.UnderrunRate01 * 100f).ToString("0.0") + "%";
        }

        /// <summary>
        /// Re-applies stock mute only, without advancing the live synth
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        public void TickMuteOnly(AudioRuntimeSettings settings)
        {
            if (Vehicle == null || Vehicle.Root == null)
                return;
            ApplyMute(settings);
        }

        private void ApplyMute(AudioRuntimeSettings settings)
        {
            bool silenceStock = settings.Mode == AudioRuntimeSettings.SoundMode.CustomOnly
                || settings.VanillaVolume <= 0.0001f;

            if (_probe != null)
            {
                _probe.Apply(silenceStock);
                DebugInfo.ForeignSources = _probe.ForeignCount;
                DebugInfo.PlayingForeign = _probe.PlayingForeignCount;
            }

            if (_muter != null)
            {
                _muter.Apply(settings);
                DebugInfo.MutedSources = _muter.MutedSourceCount;
                DebugInfo.VanillaThrottleVolField = _muter.CurrentThrottleVolumeField;
                DebugInfo.VanillaNoThrottleVolField = _muter.CurrentNoThrottleVolumeField;
            }
        }

        /// <summary>
        /// Forces mute diagnostics dump
        /// </summary>
        public void DumpMuteState()
        {
            if (_muter != null)
                _muter.ForceResolveAndLog();
            if (_probe != null)
                _probe.DumpToConsole();
        }

        /// <summary>
        /// Tears down and recreates the live Engine Sim instance
        /// </summary>
        /// <param name="settings">Current mix and quality settings</param>
        /// <returns>True when live player is ready again</returns>
        public bool RestartLive(AudioRuntimeSettings settings)
        {
            int simHz = settings != null ? settings.SimHz : 7000;
            StopLive();
            TryStartLive(simHz);
            if (_live == null || !_live.IsReady)
            {
                DebugInfo.ActiveClips = "LIVE restart failed";
                return false;
            }

            if (settings != null)
            {
                _live.SetQuality(settings.SimHz);
                _live.ApplyMix(settings);
            }
            _live.ResetStats();

            DebugInfo.ActiveBank = LiveBackendId;
            DebugInfo.UnderrunFrames = 0;
            DebugInfo.UnderrunRate01 = 0f;
            DebugInfo.ActiveClips = "LIVE restarted simHz=" + simHz;
            return true;
        }

        /// <summary>
        /// Releases runtime audio state
        /// </summary>
        public void Dispose()
        {
            StopLive();
            if (_probe != null)
                _probe.Restore();
            if (_muter != null)
                _muter.Restore();
        }

        private void TryStartLive(int simHz)
        {
            if (!EngineSimNative.IsAvailable)
            {
                ModConsole.Error("[RealisticMotorSound] Native DLL unavailable");
                return;
            }

            if (string.IsNullOrEmpty(_liveScriptPath) || !File.Exists(_liveScriptPath))
            {
                ModConsole.Error("[RealisticMotorSound] Live script missing: " + _liveScriptPath);
                return;
            }

            string prev = Directory.GetCurrentDirectory();
            try
            {
                if (!string.IsNullOrEmpty(_engineSimRoot) && Directory.Exists(_engineSimRoot))
                    Directory.SetCurrentDirectory(_engineSimRoot);

                _live = EngineSimLivePlayer.Create(_attach, _liveScriptPath, simHz);
            }
            catch (Exception ex)
            {
                ModConsole.Error("[RealisticMotorSound] Live start failed: " + ex.Message);
                _live = null;
            }
            finally
            {
                try { Directory.SetCurrentDirectory(prev); }
                catch { }
            }
        }

        private void StopLive()
        {
            if (_live == null)
                return;
            GameObject go = _live.gameObject;
            _live.Shutdown();
            _live = null;
            if (go != null)
                UnityEngine.Object.DestroyImmediate(go);
        }

        private static Transform ResolveAttachPoint(VehicleInfo vehicle)
        {
            SoundController sc = vehicle.Root.GetComponent<SoundController>();
            if (sc == null)
                sc = vehicle.Root.GetComponentInChildren<SoundController>();

            if (sc != null && sc.enginePosition != null)
                return sc.enginePosition.transform;

            return vehicle.Root.transform;
        }
    }
}
