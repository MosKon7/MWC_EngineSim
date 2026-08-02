using System;
using System.Threading;
using MSCLoader;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Streams live Engine Sim PCM into an AudioSource via OnAudioFilterRead
    /// </summary>
    public sealed class EngineSimLivePlayer : MonoBehaviour
    {
        private const int SampleRate = 44100;
        private const int RingCapacity = SampleRate / 2; // ~500 ms
        private const float TickDt = 0.01f;
        private const float TargetFillLow = 0.35f;
        private const float TargetFillHigh = 0.75f;

        private IntPtr _handle = IntPtr.Zero;
        private AudioSource _source;
        private Thread _worker;
        private volatile bool _running;
        private volatile bool _engineOn;
        private volatile float _rpm;
        private volatile float _throttle01;
        private volatile float _volume = 1f;
        private volatile float _lastTickMs;
        private volatile float _avgTickMs;
        private volatile int _queuedSamples;
        private volatile float _bufferFill01;
        private volatile int _underrunFrames;
        private volatile int _playedFrames;

        private readonly object _ringLock = new object();
        private float[] _ring;
        private int _ringRead;
        private int _ringWrite;
        private int _ringCount;
        private float _lastSample;

        private short[] _pcmScratch;
        private string _scriptPath;
        private int _simHz = 6000;
        private bool _started;
        private float _synthVolume = 0.7f;
        private float _airNoise = 0.04f;
        private float _jitter = 0.025f;
        private float _hfGain = 0.0008f;
        private float _convolution = 0.75f;
        private float _bassCutHz = 120f;
        private float _tickEma;

        public bool IsReady
        {
            get { return _handle != IntPtr.Zero && _started; }
        }

        public float LastTickMs
        {
            get { return _lastTickMs; }
        }

        public float AvgTickMs
        {
            get { return _avgTickMs; }
        }

        public float BufferFill01
        {
            get { return _bufferFill01; }
        }

        public int QueuedSamples
        {
            get { return _queuedSamples; }
        }

        /// <summary>
        /// Share of audio frames that starved the ring buffer
        /// </summary>
        public float UnderrunRate01
        {
            get
            {
                int played = _playedFrames;
                if (played <= 0)
                    return 0f;
                return Mathf.Clamp01((float)_underrunFrames / (float)played);
            }
        }

        public int UnderrunFrames
        {
            get { return _underrunFrames; }
        }

        /// <summary>
        /// Creates a live player component on an attach point
        /// </summary>
        /// <param name="attach">Parent transform</param>
        /// <param name="scriptPath">Absolute path to Sorbet.mr</param>
        /// <param name="simHz">Physics rate</param>
        /// <returns>Player or null when native init fails</returns>
        public static EngineSimLivePlayer Create(Transform attach, string scriptPath, int simHz)
        {
            GameObject go = new GameObject("RMS_EngineSimLive");
            go.transform.parent = attach;
            go.transform.localPosition = Vector3.zero;
            EngineSimLivePlayer player = go.AddComponent<EngineSimLivePlayer>();
            if (!player.Initialize(scriptPath, simHz))
            {
                Destroy(go);
                return null;
            }
            return player;
        }

        /// <summary>
        /// Applies drivetrain targets and master volume
        /// </summary>
        /// <param name="rpm">Engine RPM</param>
        /// <param name="throttle01">Throttle 0..1</param>
        /// <param name="engineOn">Ignition/combustion active</param>
        /// <param name="volume">Output gain</param>
        public void UpdateTargets(float rpm, float throttle01, bool engineOn, float volume)
        {
            _rpm = rpm;
            _throttle01 = Mathf.Clamp01(throttle01);
            _engineOn = engineOn;
            _volume = Mathf.Clamp(volume, 0f, 3f);
            if (_source != null)
                _source.mute = volume <= 0.0001f;
        }

        /// <summary>
        /// Returns pedal throttle for native targets
        /// </summary>
        private float TargetThrottle01()
        {
            float target = Mathf.Clamp01(_throttle01);
            // Small idle floor so closed pedal still combusts under dyno hold
            if (_engineOn && target < 0.2f)
                target = 0.2f;
            return target;
        }

        /// <summary>
        /// Updates simulation frequency
        /// </summary>
        /// <param name="simHz">Physics Hz</param>
        public void SetQuality(int simHz)
        {
            _simHz = Mathf.Clamp(simHz, 500, 18000);
            if (_handle != IntPtr.Zero)
                EngineSimNative.esm_set_quality(_handle, _simHz);
        }

        /// <summary>
        /// Pushes synthesizer mix and bass-cut settings into the native sim
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        public void ApplyMix(AudioRuntimeSettings settings)
        {
            if (settings == null || _handle == IntPtr.Zero)
                return;

            _synthVolume = Mathf.Clamp(settings.SynthVolume, 0.05f, 2f);
            _airNoise = Mathf.Clamp01(settings.AirNoise);
            _jitter = Mathf.Clamp01(settings.Jitter);
            _hfGain = Mathf.Clamp(settings.HfGain, 0f, 0.05f);
            _convolution = Mathf.Clamp01(settings.Convolution);
            float bass = Mathf.Clamp(settings.BassCutHz, 0f, 800f);
            _bassCutHz = bass;

            EngineSimNative.esm_set_mix(
                _handle,
                _synthVolume,
                _airNoise,
                _jitter,
                _hfGain,
                _convolution);
            EngineSimNative.esm_set_highpass(_handle, _bassCutHz);
        }

        /// <summary>
        /// Clears underrun counters after a restart or quality change
        /// </summary>
        public void ResetStats()
        {
            _underrunFrames = 0;
            _playedFrames = 0;
            _tickEma = 0f;
            _avgTickMs = 0f;
        }

        /// <summary>
        /// Stops worker and releases native handle
        /// </summary>
        public void Shutdown()
        {
            _running = false;
            _started = false;
            if (_worker != null && _worker.IsAlive)
            {
                try { _worker.Join(500); }
                catch { }
            }
            _worker = null;

            if (_handle != IntPtr.Zero)
            {
                EngineSimNative.esm_destroy(_handle);
                _handle = IntPtr.Zero;
            }

            if (_source != null && _source.isPlaying)
                _source.Stop();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private bool Initialize(string scriptPath, int simHz)
        {
            _scriptPath = scriptPath;
            _simHz = simHz;
            _ring = new float[RingCapacity];
            _pcmScratch = new short[4096];

            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.loop = true;
            _source.spatialBlend = 0.85f;
            _source.dopplerLevel = 0f;
            _source.minDistance = 8f;
            _source.maxDistance = 120f;
            _source.rolloffMode = AudioRolloffMode.Linear;
            _source.priority = 32;
            _source.volume = 1f;
            _source.pitch = 1f;

            AudioClip silence = AudioClip.Create("rms_live_carrier", SampleRate, 1, SampleRate, false, false);
            float[] zeros = new float[SampleRate];
            silence.SetData(zeros, 0);
            _source.clip = silence;

            try
            {
                _handle = EngineSimNative.esm_create(scriptPath);
            }
            catch (Exception ex)
            {
                ModConsole.Error("[RealisticMotorSound] esm_create exception: " + ex.Message);
                return false;
            }

            if (_handle == IntPtr.Zero)
            {
                ModConsole.Error("[RealisticMotorSound] esm_create failed: " + EngineSimNative.LastError());
                return false;
            }

            EngineSimNative.esm_set_quality(_handle, _simHz);
            EngineSimNative.esm_set_mix(
                _handle,
                _synthVolume,
                _airNoise,
                _jitter,
                _hfGain,
                _convolution);
            EngineSimNative.esm_set_highpass(_handle, _bassCutHz);
            ResetStats();
            _running = true;
            _worker = new Thread(WorkerLoop);
            _worker.IsBackground = true;
            _worker.Name = "RMS_EngineSimWorker";
            _worker.Priority = System.Threading.ThreadPriority.AboveNormal;
            _worker.Start();

            _source.Play();
            _started = true;
            ModConsole.Print("[RealisticMotorSound] Live Engine Sim ready (" + _simHz + " Hz)");
            return true;
        }

        private void WorkerLoop()
        {
            while (_running && _handle != IntPtr.Zero)
            {
                try
                {
                    float fill = CurrentFill01();
                    int burst = 0;
                    // Catch up while ring is thin; cap burst so one stall cannot freeze audio thread waiters
                    do
                    {
                        float rpm = _rpm;
                        float thr = TargetThrottle01();
                        int ign = (_engineOn && rpm >= 350f) ? 1 : 0;
                        EngineSimNative.esm_set_targets(_handle, rpm, thr, ign);
                        EngineSimNative.esm_tick(_handle, TickDt);
                        _lastTickMs = EngineSimNative.esm_last_tick_ms(_handle);
                        if (_tickEma <= 0.01f)
                            _tickEma = _lastTickMs;
                        else
                            _tickEma = _tickEma * 0.9f + _lastTickMs * 0.1f;
                        _avgTickMs = _tickEma;
                        _queuedSamples = EngineSimNative.esm_queued_samples(_handle);

                        for (;;)
                        {
                            int n = EngineSimNative.esm_read_pcm(_handle, _pcmScratch, _pcmScratch.Length);
                            if (n <= 0)
                                break;
                            PushPcm(_pcmScratch, n);
                        }

                        fill = CurrentFill01();
                        _bufferFill01 = fill;
                        burst++;
                    }
                    while (_running
                        && fill < TargetFillLow
                        && burst < 8
                        && _lastTickMs < 28f);

                    // Sleep only when buffer is healthy; never sleep while starving
                    if (fill >= TargetFillHigh)
                        Thread.Sleep(_lastTickMs > 9f ? 4 : 8);
                    else if (fill >= TargetFillLow)
                        Thread.Sleep(_lastTickMs > 12f ? 1 : 3);
                    // else: immediate next burst
                }
                catch (Exception)
                {
                    Thread.Sleep(20);
                }
            }
        }

        private float CurrentFill01()
        {
            lock (_ringLock)
                return (float)_ringCount / (float)RingCapacity;
        }

        private void PushPcm(short[] pcm, int count)
        {
            lock (_ringLock)
            {
                for (int i = 0; i < count; i++)
                {
                    if (_ringCount >= RingCapacity)
                    {
                        // Drop oldest to keep latency bounded
                        _ringRead = (_ringRead + 1) % RingCapacity;
                        _ringCount--;
                    }

                    float sample = pcm[i] / 32768f;
                    _ring[_ringWrite] = sample;
                    _ringWrite = (_ringWrite + 1) % RingCapacity;
                    _ringCount++;
                    _lastSample = sample;
                }
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!_started || data == null || data.Length == 0)
                return;

            // Flat gain like the simulator UI: no idle/load make-up, no slew
            float gain = _volume * 0.55f;

            int ch = Mathf.Max(1, channels);
            int frames = data.Length / ch;
            int underruns = 0;

            lock (_ringLock)
            {
                for (int f = 0; f < frames; f++)
                {
                    float sample;
                    if (_ringCount > 0)
                    {
                        sample = _ring[_ringRead];
                        _ringRead = (_ringRead + 1) % RingCapacity;
                        _ringCount--;
                        _lastSample = sample;
                    }
                    else
                    {
                        underruns++;
                        // Hold last sample instead of hard zero click
                        sample = _lastSample * 0.96f;
                        _lastSample = sample;
                    }

                    sample *= gain;
                    // Soft limit after game volume so peaks don't rasp
                    if (sample > 1f) sample = 1f;
                    else if (sample < -1f) sample = -1f;
                    sample = sample * (1.5f - 0.5f * sample * sample);

                    int baseIndex = f * ch;
                    for (int c = 0; c < ch; c++)
                        data[baseIndex + c] = sample;
                }

                _bufferFill01 = (float)_ringCount / (float)RingCapacity;
            }

            _playedFrames += frames;
            if (underruns > 0)
                _underrunFrames += underruns;
        }
    }
}
