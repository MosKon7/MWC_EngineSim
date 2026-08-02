using System.Reflection;
using MSCLoader;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Tracks stock SoundController engine sources for HUD and registration
    /// </summary>
    public sealed class VanillaSoundMuter
    {
        private static readonly FieldInfo ThrottleSourceField =
            typeof(SoundController).GetField("engineThrottleSource", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo NoThrottleSourceField =
            typeof(SoundController).GetField("engineNoThrottleSource", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly SoundController _soundController;
        private AudioSource _throttleSource;
        private AudioSource _noThrottleSource;
        private float _savedThrottleVolume = 1f;
        private float _savedNoThrottleVolume = 1f;
        private bool _saved;

        public int MutedSourceCount
        {
            get
            {
                int n = 0;
                if (_throttleSource != null) n++;
                if (_noThrottleSource != null) n++;
                return n;
            }
        }

        public float CurrentThrottleVolumeField
        {
            get { return _throttleSource != null ? _throttleSource.volume : 0f; }
        }

        public float CurrentNoThrottleVolumeField
        {
            get { return _noThrottleSource != null ? _noThrottleSource.volume : 0f; }
        }

        public float FieldThrottle
        {
            get { return _soundController != null ? _soundController.engineThrottleVolume : 0f; }
        }

        public float FieldNoThrottle
        {
            get { return _soundController != null ? _soundController.engineNoThrottleVolume : 0f; }
        }

        public VanillaSoundMuter(SoundController soundController)
        {
            _soundController = soundController;
            VehicleMuteGate.Register(soundController);
            ResolveSources();
        }

        /// <summary>
        /// Refreshes source refs and stores baseline volumes once
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        public void Apply(AudioRuntimeSettings settings)
        {
            if (_soundController == null)
                return;

            if (_throttleSource == null || _noThrottleSource == null)
                ResolveSources();

            if (!_saved)
            {
                _savedThrottleVolume = Mathf.Max(0.01f, _soundController.engineThrottleVolume);
                _savedNoThrottleVolume = Mathf.Max(0.01f, _soundController.engineNoThrottleVolume);
                _saved = true;
                ModConsole.Print(string.Format(
                    "[RealisticMotorSound] Vanilla base volumes throttle={0:0.00} noThrottle={1:0.00} sources={2}",
                    _savedThrottleVolume, _savedNoThrottleVolume, MutedSourceCount));
            }
        }

        /// <summary>
        /// Restores original SoundController volume fields
        /// </summary>
        public void Restore()
        {
            if (_soundController == null || !_saved)
                return;

            _soundController.engineThrottleVolume = _savedThrottleVolume;
            _soundController.engineNoThrottleVolume = _savedNoThrottleVolume;
            if (_throttleSource != null)
                _throttleSource.mute = false;
            if (_noThrottleSource != null)
                _noThrottleSource.mute = false;
        }

        /// <summary>
        /// Logs current private engine sources
        /// </summary>
        public void ForceResolveAndLog()
        {
            ResolveSources();
            ModConsole.Print("[RealisticMotorSound] Vanilla engine sources: " + MutedSourceCount);
            LogSource("throttle", _throttleSource);
            LogSource("noThrottle", _noThrottleSource);
            ModConsole.Print(string.Format(
                "[RealisticMotorSound] fields throttle={0:0.00} noThrottle={1:0.00} | live src vol={2:0.00}/{3:0.00} mute={4}/{5}",
                FieldThrottle,
                FieldNoThrottle,
                _throttleSource != null ? _throttleSource.volume : -1f,
                _noThrottleSource != null ? _noThrottleSource.volume : -1f,
                _throttleSource != null && _throttleSource.mute,
                _noThrottleSource != null && _noThrottleSource.mute));
        }

        private void ResolveSources()
        {
            if (_soundController == null)
                return;

            _throttleSource = ThrottleSourceField != null
                ? ThrottleSourceField.GetValue(_soundController) as AudioSource
                : null;
            _noThrottleSource = NoThrottleSourceField != null
                ? NoThrottleSourceField.GetValue(_soundController) as AudioSource
                : null;
        }

        private static void LogSource(string tag, AudioSource source)
        {
            if (source == null)
            {
                ModConsole.Print("  [" + tag + "] null");
                return;
            }

            ModConsole.Print(string.Format(
                "  [{0}] go={1} clip={2} vol={3:0.00} mute={4} playing={5}",
                tag,
                source.gameObject.name,
                source.clip != null ? source.clip.name : "null",
                source.volume,
                source.mute,
                source.isPlaying));
        }
    }
}
