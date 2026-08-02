using System.Collections.Generic;
using System.IO;
using CarList;
using MSCLoader;
using RealisticMotorSound.Audio;

namespace RealisticMotorSound.Vehicles
{
    /// <summary>
    /// Maps scene vehicles to live Engine Sim sessions
    /// </summary>
    public sealed class VehicleAudioRegistry
    {
        private readonly Dictionary<string, string> _namePrefixToProfile = new Dictionary<string, string>();
        private readonly List<VehicleEngineSession> _sessions = new List<VehicleEngineSession>();
        private readonly HashSet<int> _boundRoots = new HashSet<int>();

        private string _assetsRoot;
        private string _engineSimRoot;
        private string _liveScriptPath;
        private bool _nativeReady;

        public VehicleAudioRegistry()
        {
            _namePrefixToProfile["SORBET"] = "sorbet";
        }

        /// <summary>
        /// Active session used for HUD/logging, prefers Sorbet
        /// </summary>
        public VehicleEngineSession PrimarySession
        {
            get
            {
                for (int i = 0; i < _sessions.Count; i++)
                {
                    if (_sessions[i].ProfileId == "sorbet")
                        return _sessions[i];
                }
                return _sessions.Count > 0 ? _sessions[0] : null;
            }
        }

        public int SessionCount
        {
            get { return _sessions.Count; }
        }

        /// <summary>
        /// Prepares asset root and native Engine Sim DLL
        /// </summary>
        /// <param name="mod">Owning mod instance</param>
        public void Initialize(Mod mod)
        {
            _assetsRoot = ModLoader.GetModAssetsFolder(mod);
            _engineSimRoot = Path.Combine(_assetsRoot, "engine_sim");
            _liveScriptPath = Path.Combine(_engineSimRoot, "Sorbet.mr");

            // Native DLLs live under Assets/.../engine_sim, NOT Mods root
            // (MSCLoader treats every Mods/*.dll as a managed mod).
            _nativeReady = EngineSimNative.TryBootstrap(_engineSimRoot);
            if (!_nativeReady)
                ModConsole.Error("[RealisticMotorSound] Live backend unavailable (native load failed)");
            else if (!File.Exists(_liveScriptPath))
                ModConsole.Error("[RealisticMotorSound] Missing live script: " + _liveScriptPath);
        }

        /// <summary>
        /// Creates live sessions for newly found supported vehicles
        /// </summary>
        /// <param name="finder">Scene vehicle index</param>
        public void BindNewVehicles(VehicleFinder finder)
        {
            if (finder == null || finder.Vehicles == null)
                return;

            if (!_nativeReady || string.IsNullOrEmpty(_liveScriptPath) || !File.Exists(_liveScriptPath))
                return;

            AudioRuntimeSettings settings = AudioRuntimeSettings.Instance;

            for (int i = 0; i < finder.Vehicles.Count; i++)
            {
                VehicleInfo vehicle = finder.Vehicles[i];
                if (vehicle == null || vehicle.Root == null || vehicle.Drivetrain == null)
                    continue;

                int id = vehicle.Root.GetInstanceID();
                if (_boundRoots.Contains(id))
                    continue;

                string profileId;
                if (!TryResolveProfile(vehicle.Root.name, out profileId))
                    continue;

                var session = new VehicleEngineSession(
                    profileId,
                    vehicle,
                    _engineSimRoot,
                    _liveScriptPath,
                    settings.SimHz);

                _sessions.Add(session);
                _boundRoots.Add(id);
                ModConsole.Print("[RealisticMotorSound] Bound profile '" + profileId
                    + "' live to " + vehicle.Root.name);
            }
        }

        /// <summary>
        /// Ticks all active vehicle sessions
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        public void Tick(AudioRuntimeSettings settings)
        {
            for (int i = 0; i < _sessions.Count; i++)
                _sessions[i].Tick(settings);
        }

        /// <summary>
        /// Re-applies stock mute after other scripts rewrite AudioSource volumes
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        public void TickMuteOnly(AudioRuntimeSettings settings)
        {
            for (int i = 0; i < _sessions.Count; i++)
                _sessions[i].TickMuteOnly(settings);
        }

        /// <summary>
        /// Dumps mute diagnostics for all sessions
        /// </summary>
        public void DumpMuteState()
        {
            for (int i = 0; i < _sessions.Count; i++)
                _sessions[i].DumpMuteState();
        }

        /// <summary>
        /// Restarts live Engine Sim for every bound vehicle
        /// </summary>
        /// <param name="settings">Current mix and quality settings</param>
        /// <returns>Number of sessions that came back ready</returns>
        public int RestartLive(AudioRuntimeSettings settings)
        {
            int ok = 0;
            for (int i = 0; i < _sessions.Count; i++)
            {
                if (_sessions[i].RestartLive(settings))
                    ok++;
            }
            return ok;
        }

        private bool TryResolveProfile(string rootName, out string profileId)
        {
            foreach (KeyValuePair<string, string> pair in _namePrefixToProfile)
            {
                if (rootName.StartsWith(pair.Key))
                {
                    profileId = pair.Value;
                    return true;
                }
            }

            profileId = null;
            return false;
        }
    }
}
