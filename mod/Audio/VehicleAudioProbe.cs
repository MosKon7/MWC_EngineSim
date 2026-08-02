using System.Collections.Generic;
using MSCLoader;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Silences stock vehicle audio that is not on the keep-list
    /// </summary>
    public sealed class VehicleAudioProbe
    {
        // Non-engine sounds we must not kill
        private static readonly string[] KeepHints = new string[]
        {
            "skid", "brake", "wind", "crash", "scrape", "abs", "shift",
            "horn", "door", "radio", "blink", "turn", "signal", "tyre",
            "tire", "gravel", "sand", "grass", "rolling", "collision",
            "bump", "suspension", "creak", "handbrake", "reversebeep",
            "indicator", "wiper", "seatbelt"
        };

        private readonly Transform _root;
        private readonly List<AudioSource> _muteSet = new List<AudioSource>();
        private readonly List<AudioSource> _allForeign = new List<AudioSource>();
        private readonly Dictionary<int, SourceState> _saved = new Dictionary<int, SourceState>();
        private float _nextScanTime;
        private bool _silencing;

        private struct SourceState
        {
            public float Volume;
            public bool Mute;
            public bool WasPlaying;
        }

        public int ForeignCount
        {
            get { return _muteSet.Count; }
        }

        public int PlayingForeignCount
        {
            get
            {
                int n = 0;
                for (int i = 0; i < _muteSet.Count; i++)
                {
                    AudioSource s = _muteSet[i];
                    if (s != null && s.isPlaying && s.volume > 0.001f && !s.mute)
                        n++;
                }
                return n;
            }
        }

        public VehicleAudioProbe(Transform root)
        {
            _root = root;
            Rescan(true);
        }

        /// <summary>
        /// Rescans vehicle hierarchy for foreign audio sources
        /// </summary>
        /// <param name="force">Ignore scan cooldown</param>
        public void Rescan(bool force)
        {
            if (!force && Time.time < _nextScanTime)
                return;

            _muteSet.Clear();
            _allForeign.Clear();
            if (_root == null)
                return;

            AudioSource[] all = _root.GetComponentsInChildren<AudioSource>(true);
            for (int i = 0; i < all.Length; i++)
            {
                AudioSource source = all[i];
                if (source == null || IsOurs(source))
                    continue;

                _allForeign.Add(source);
                if (ShouldMute(source))
                    _muteSet.Add(source);
            }

            _nextScanTime = Time.time + 1.5f;
        }

        /// <summary>
        /// Applies or restores silence for mute-set sources
        /// </summary>
        /// <param name="silence">True to mute stock engine-ish audio</param>
        public void Apply(bool silence)
        {
            Rescan(false);

            if (silence)
            {
                _silencing = true;
                for (int i = 0; i < _muteSet.Count; i++)
                {
                    AudioSource source = _muteSet[i];
                    if (source == null)
                        continue;

                    int id = source.GetInstanceID();
                    if (!_saved.ContainsKey(id))
                    {
                        SourceState st;
                        st.Volume = source.volume;
                        st.Mute = source.mute;
                        st.WasPlaying = source.isPlaying;
                        _saved[id] = st;
                    }

                    source.volume = 0f;
                    source.mute = true;
                    if (source.isPlaying)
                        source.Stop();
                }
            }
            else if (_silencing)
            {
                Restore();
            }
        }

        /// <summary>
        /// Restores previously saved source states
        /// </summary>
        public void Restore()
        {
            for (int i = 0; i < _muteSet.Count; i++)
            {
                AudioSource source = _muteSet[i];
                if (source == null)
                    continue;

                SourceState st;
                if (!_saved.TryGetValue(source.GetInstanceID(), out st))
                    continue;

                source.mute = st.Mute;
                source.volume = st.Volume;
                if (st.WasPlaying && source.clip != null && !source.isPlaying)
                    source.Play();
            }

            _saved.Clear();
            _silencing = false;
        }

        /// <summary>
        /// Logs foreign sources and mute decisions
        /// </summary>
        public void DumpToConsole()
        {
            Rescan(true);
            ModConsole.Print(string.Format(
                "[RealisticMotorSound] Foreign={0}, mute-set={1}",
                _allForeign.Count, _muteSet.Count));

            for (int i = 0; i < _allForeign.Count; i++)
            {
                AudioSource s = _allForeign[i];
                if (s == null)
                    continue;

                bool muted = _muteSet.Contains(s);
                ModConsole.Print(string.Format(
                    "  [{0}] mute={1} path={2} clip={3} vol={4:0.00} srcMute={5} playing={6} loop={7} pitch={8:0.00}",
                    i,
                    muted ? "Y" : "n",
                    GetPath(s.transform),
                    s.clip != null ? s.clip.name : "null",
                    s.volume,
                    s.mute,
                    s.isPlaying,
                    s.loop,
                    s.pitch));
            }
        }

        private static bool ShouldMute(AudioSource source)
        {
            string clip = source.clip != null ? source.clip.name.ToLowerInvariant() : string.Empty;
            string go = source.gameObject.name.ToLowerInvariant();
            string path = GetPath(source.transform).ToLowerInvariant();

            if (IsKeep(clip) || IsKeep(go))
                return false;

            // SoundController children are always named "audio"
            if (go == "audio")
                return true;

            // Looping 3D sources under the car are almost always drivetrain/engine beds
            if (source.loop && source.spatialBlend > 0.15f)
                return true;

            // Explicit engine-ish names even if one-shot
            if (clip.IndexOf("idle") >= 0 || clip.IndexOf("rev") >= 0
                || clip.IndexOf("engine") >= 0 || clip.IndexOf("corris") >= 0
                || clip.IndexOf("throttle") >= 0 || clip.IndexOf("exhaust") >= 0)
                return true;

            if (path.IndexOf("engine") >= 0 && source.loop)
                return true;

            return false;
        }

        private static bool IsKeep(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;
            for (int i = 0; i < KeepHints.Length; i++)
            {
                if (text.IndexOf(KeepHints[i]) >= 0)
                    return true;
            }
            return false;
        }

        private static bool IsOurs(AudioSource source)
        {
            Transform t = source.transform;
            while (t != null)
            {
                if (t.name.StartsWith("RMS_"))
                    return true;
                t = t.parent;
            }
            return false;
        }

        private static string GetPath(Transform t)
        {
            string path = t.name;
            Transform p = t.parent;
            int guard = 0;
            while (p != null && guard < 8)
            {
                path = p.name + "/" + path;
                p = p.parent;
                guard++;
            }
            return path;
        }
    }
}
