using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// RPM/throttle crossfade with double-buffered voices and fixed pitch
    /// </summary>
    public sealed class EngineAudioPlayer
    {
        private const float MinRunningRpm = 350f;
        private const float SilentVolume = 0.00005f;
        private const float RebindVolume = 0.02f;

        private EngineSampleBank _bank;
        private readonly GameObject _root;
        private readonly BufferedVoice _lowCoast;
        private readonly BufferedVoice _lowLoad;
        private readonly BufferedVoice _highCoast;
        private readonly BufferedVoice _highLoad;
        private readonly AudioSource _oneShot;

        private float _boundLowRpm = -1f;
        private float _boundHighRpm = -1f;
        private float _boundCoastThr = -1f;
        private float _boundLoadThr = -1f;
        private bool _wasRunning;

        public readonly MixDebugInfo Debug = new MixDebugInfo();

        public EngineAudioPlayer(EngineSampleBank bank, Transform attachPoint)
        {
            _bank = bank;

            _root = new GameObject("RMS_EngineAudio_" + bank.VehicleId);
            _root.transform.parent = attachPoint;
            _root.transform.localPosition = Vector3.zero;

            _lowCoast = new BufferedVoice(_root, "LowCoast");
            _lowLoad = new BufferedVoice(_root, "LowLoad");
            _highCoast = new BufferedVoice(_root, "HighCoast");
            _highLoad = new BufferedVoice(_root, "HighLoad");
            _oneShot = CreateOneShotSource(_root, "OneShot");
        }

        /// <summary>
        /// Replaces the sample bank and resets loop bindings
        /// </summary>
        /// <param name="bank">New bank to play</param>
        public void SetBank(EngineSampleBank bank)
        {
            if (bank == null)
                return;

            _bank = bank;
            _boundLowRpm = _boundHighRpm = -1f;
            _boundCoastThr = _boundLoadThr = -1f;
            _lowCoast.ResetClips();
            _lowLoad.ResetClips();
            _highCoast.ResetClips();
            _highLoad.ResetClips();
            Debug.ProfileId = bank.VehicleId;
        }

        /// <summary>
        /// Updates mix from live drivetrain state
        /// </summary>
        /// <param name="rpm">Current engine RPM</param>
        /// <param name="throttle01">Throttle 0..1</param>
        /// <param name="engineOn">Whether combustion should be audible</param>
        /// <param name="settings">Shared runtime settings</param>
        public void Update(float rpm, float throttle01, bool engineOn, AudioRuntimeSettings settings)
        {
            bool customEnabled = settings.Mode != AudioRuntimeSettings.SoundMode.VanillaOnly
                && settings.CustomVolume > 0.0001f;
            bool running = engineOn && rpm >= MinRunningRpm;

            if (customEnabled)
            {
                if (running && !_wasRunning)
                    PlayClip(_bank.Startup, settings.CustomVolume * 0.85f);
                else if (!running && _wasRunning)
                    PlayClip(_bank.IgnitionOff, settings.CustomVolume * 0.7f);
            }

            _wasRunning = running;
            Debug.Rpm = rpm;
            Debug.Throttle01 = throttle01;
            Debug.EngineOn = running;

            float smooth = Mathf.Clamp01(Time.deltaTime * settings.VolumeSmooth);

            if (!running || !customEnabled)
            {
                ApplyVoice(_lowCoast, null, 0f, 0f, smooth);
                ApplyVoice(_lowLoad, null, 0f, 0f, smooth);
                ApplyVoice(_highCoast, null, 0f, 0f, smooth);
                ApplyVoice(_highLoad, null, 0f, 0f, smooth);
                FillDebug(0f, 0f, 0f, 0f, 0f, 0f);
                return;
            }

            float thrPercent = Mathf.Clamp(throttle01 * 100f, 0f, 100f);

            int rpmIndex;
            float rpmBlend;
            FindPair(_bank.RpmAnchors, rpm, out rpmIndex, out rpmBlend);

            int thrIndex;
            float thrBlend;
            FindPair(_bank.ThrottleAnchors, thrPercent, out thrIndex, out thrBlend);

            float lowRpm = _bank.RpmAnchors[rpmIndex];
            float highRpm = _bank.RpmAnchors[Mathf.Min(rpmIndex + 1, _bank.RpmAnchors.Length - 1)];
            float coastThr = _bank.ThrottleAnchors[thrIndex];
            float loadThr = _bank.ThrottleAnchors[Mathf.Min(thrIndex + 1, _bank.ThrottleAnchors.Length - 1)];

            bool windowChanged = !Mathf.Approximately(lowRpm, _boundLowRpm)
                || !Mathf.Approximately(highRpm, _boundHighRpm)
                || !Mathf.Approximately(coastThr, _boundCoastThr)
                || !Mathf.Approximately(loadThr, _boundLoadThr);

            float syncNorm = LoudestNorm();

            if (windowChanged)
            {
                _boundLowRpm = lowRpm;
                _boundHighRpm = highRpm;
                _boundCoastThr = coastThr;
                _boundLoadThr = loadThr;
            }

            float rpmA = Mathf.Sqrt(Mathf.Clamp01(1f - rpmBlend));
            float rpmB = Mathf.Sqrt(Mathf.Clamp01(rpmBlend));
            float thrA = Mathf.Sqrt(Mathf.Clamp01(1f - thrBlend));
            float thrB = Mathf.Sqrt(Mathf.Clamp01(thrBlend));
            float k = settings.CustomVolume;

            ApplyVoice(_lowCoast, _bank.GetLoop(lowRpm, coastThr), rpmA * thrA * k, syncNorm, smooth);
            ApplyVoice(_lowLoad, _bank.GetLoop(lowRpm, loadThr), rpmA * thrB * k, syncNorm, smooth);
            ApplyVoice(_highCoast, _bank.GetLoop(highRpm, coastThr), rpmB * thrA * k, syncNorm, smooth);
            ApplyVoice(_highLoad, _bank.GetLoop(highRpm, loadThr), rpmB * thrB * k, syncNorm, smooth);

            FillDebug(lowRpm, highRpm, coastThr, loadThr, rpmBlend, thrBlend);
        }

        /// <summary>
        /// Fades all loop voices to silence without hard Stop
        /// </summary>
        public void StopAll()
        {
            _wasRunning = false;
            _boundLowRpm = _boundHighRpm = -1f;
            _boundCoastThr = _boundLoadThr = -1f;
            _lowCoast.ForceSilence();
            _lowLoad.ForceSilence();
            _highCoast.ForceSilence();
            _highLoad.ForceSilence();
            if (_oneShot.isPlaying)
                _oneShot.Stop();
        }

        private void ApplyVoice(
            BufferedVoice voice,
            AudioClip wanted,
            float targetVol,
            float syncNorm,
            float smooth)
        {
            voice.Update(wanted, targetVol, syncNorm, smooth);
        }

        private void FillDebug(
            float lowRpm,
            float highRpm,
            float coastThr,
            float loadThr,
            float rpmBlend,
            float thrBlend)
        {
            Debug.LowRpm = lowRpm;
            Debug.HighRpm = highRpm;
            Debug.CoastThr = coastThr;
            Debug.LoadThr = loadThr;
            Debug.RpmBlend = rpmBlend;
            Debug.ThrBlend = thrBlend;
            Debug.LowCoastVol = _lowCoast.Volume;
            Debug.LowLoadVol = _lowLoad.Volume;
            Debug.HighCoastVol = _highCoast.Volume;
            Debug.HighLoadVol = _highLoad.Volume;
            Debug.ActiveClips = string.Format(
                "L={0} H={1} coast={2} load={3} bank={4}",
                lowRpm.ToString("0"),
                highRpm.ToString("0"),
                coastThr.ToString("0"),
                loadThr.ToString("0"),
                _bank != null ? _bank.VehicleId : "-");
        }

        private float LoudestNorm()
        {
            BufferedVoice best = _lowCoast;
            float bestVol = _lowCoast.Volume;
            if (_lowLoad.Volume > bestVol) { best = _lowLoad; bestVol = _lowLoad.Volume; }
            if (_highCoast.Volume > bestVol) { best = _highCoast; bestVol = _highCoast.Volume; }
            if (_highLoad.Volume > bestVol) { best = _highLoad; bestVol = _highLoad.Volume; }
            return best.NormTime;
        }

        private void PlayClip(AudioClip clip, float volume)
        {
            if (clip == null)
                return;
            _oneShot.clip = clip;
            _oneShot.loop = false;
            _oneShot.pitch = 1f;
            _oneShot.volume = Mathf.Clamp01(volume);
            _oneShot.Play();
        }

        private static AudioSource CreateOneShotSource(GameObject root, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.parent = root.transform;
            go.transform.localPosition = Vector3.zero;
            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0.85f;
            src.dopplerLevel = 0f;
            src.minDistance = 8f;
            src.maxDistance = 120f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.priority = 32;
            src.volume = 0f;
            src.pitch = 1f;
            return src;
        }

        private static void FindPair(float[] anchors, float value, out int index, out float blend)
        {
            if (anchors == null || anchors.Length == 0)
            {
                index = 0;
                blend = 0f;
                return;
            }

            if (value <= anchors[0])
            {
                index = 0;
                blend = 0f;
                return;
            }

            int last = anchors.Length - 1;
            if (value >= anchors[last])
            {
                index = Mathf.Max(0, last - 1);
                blend = anchors.Length == 1 ? 0f : 1f;
                return;
            }

            for (int i = 0; i < last; i++)
            {
                if (value >= anchors[i] && value <= anchors[i + 1])
                {
                    index = i;
                    float span = anchors[i + 1] - anchors[i];
                    blend = span > 0.0001f ? (value - anchors[i]) / span : 0f;
                    return;
                }
            }

            index = last;
            blend = 0f;
        }

        /// <summary>
        /// One logical layer with front/back AudioSources for click-free clip swaps
        /// </summary>
        private sealed class BufferedVoice
        {
            private readonly AudioSource _a;
            private readonly AudioSource _b;
            private int _front;
            private float _volume;

            public float Volume
            {
                get { return _volume; }
            }

            public float NormTime
            {
                get
                {
                    AudioSource front = Front;
                    if (front.clip == null || front.clip.length <= 0.01f)
                        return 0f;
                    return (front.time % front.clip.length) / front.clip.length;
                }
            }

            private AudioSource Front
            {
                get { return _front == 0 ? _a : _b; }
            }

            private AudioSource Back
            {
                get { return _front == 0 ? _b : _a; }
            }

            public BufferedVoice(GameObject root, string name)
            {
                _a = CreateLoopSource(root, name + "A");
                _b = CreateLoopSource(root, name + "B");
                _front = 0;
            }

            /// <summary>
            /// Advances volume and binds clips only on a silent buffer
            /// </summary>
            /// <param name="wanted">Desired loop clip</param>
            /// <param name="targetVol">Target front volume</param>
            /// <param name="syncNorm">Normalized phase for new binds</param>
            /// <param name="smooth">0..1 lerp factor</param>
            public void Update(AudioClip wanted, float targetVol, float syncNorm, float smooth)
            {
                AudioSource front = Front;
                AudioSource back = Back;

                if (wanted != null && front.clip != wanted)
                    TrySwapTo(wanted, syncNorm);

                front = Front;
                back = Back;

                _volume = Mathf.Lerp(_volume, Mathf.Max(0f, targetVol), smooth);

                front.pitch = 1f;
                back.pitch = 1f;
                front.mute = false;
                back.mute = false;

                front.volume = _volume <= SilentVolume ? 0f : Mathf.Clamp(_volume, 0f, 3f);
                if (back.volume > SilentVolume)
                    back.volume = Mathf.Lerp(back.volume, 0f, Mathf.Clamp01(smooth * 1.5f));
                else
                    back.volume = 0f;

                if (!front.isPlaying && front.clip != null && front.volume > SilentVolume)
                    front.Play();
                if (!back.isPlaying && back.clip != null && back.volume > SilentVolume)
                    back.Play();
            }

            /// <summary>
            /// Clears bound clips after fading volumes to zero
            /// </summary>
            public void ResetClips()
            {
                ForceSilence();
                _a.clip = null;
                _b.clip = null;
            }

            /// <summary>
            /// Forces both buffers to silent volume
            /// </summary>
            public void ForceSilence()
            {
                _volume = 0f;
                _a.volume = 0f;
                _b.volume = 0f;
            }

            private void TrySwapTo(AudioClip wanted, float syncNorm)
            {
                AudioSource front = Front;
                AudioSource back = Back;

                // Silent front can be rebound in place
                if (_volume <= RebindVolume && front.volume <= RebindVolume)
                {
                    Bind(front, wanted, syncNorm);
                    return;
                }

                // Audible front: prepare silent back, then flip roles
                if (back.volume > RebindVolume)
                    back.volume = 0f;

                Bind(back, wanted, syncNorm);

                float carried = front.volume > SilentVolume ? front.volume : _volume;
                _front = 1 - _front;
                Front.volume = 0f;
                Back.volume = carried;
            }

            private static void Bind(AudioSource source, AudioClip clip, float syncNorm)
            {
                if (source.clip == clip)
                    return;

                // Only assign when this buffer is effectively silent
                if (source.volume > SilentVolume)
                    source.volume = 0f;

                source.clip = clip;
                source.loop = true;
                source.pitch = 1f;
                if (clip != null && clip.length > 0.01f)
                    source.time = Mathf.Clamp01(syncNorm) * clip.length;
                if (!source.isPlaying)
                    source.Play();
            }

            private static AudioSource CreateLoopSource(GameObject root, string name)
            {
                GameObject go = new GameObject(name);
                go.transform.parent = root.transform;
                go.transform.localPosition = Vector3.zero;
                AudioSource src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.loop = true;
                src.spatialBlend = 0.85f;
                src.dopplerLevel = 0f;
                src.minDistance = 8f;
                src.maxDistance = 120f;
                src.rolloffMode = AudioRolloffMode.Linear;
                src.priority = 32;
                src.volume = 0f;
                src.pitch = 1f;
                return src;
            }
        }
    }
}
