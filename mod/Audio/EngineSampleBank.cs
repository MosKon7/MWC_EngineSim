using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MSCLoader;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Loads a vehicle sample bank from unity_audio_manifest.csv
    /// </summary>
    public sealed class EngineSampleBank
    {
        public readonly string VehicleId;
        public readonly float[] RpmAnchors;
        public readonly float[] ThrottleAnchors;
        public readonly AudioClip Startup;
        public readonly AudioClip IgnitionOff;

        private readonly Dictionary<long, AudioClip> _loops = new Dictionary<long, AudioClip>();

        private EngineSampleBank(
            string vehicleId,
            float[] rpmAnchors,
            float[] throttleAnchors,
            AudioClip startup,
            AudioClip ignitionOff)
        {
            VehicleId = vehicleId;
            RpmAnchors = rpmAnchors;
            ThrottleAnchors = throttleAnchors;
            Startup = startup;
            IgnitionOff = ignitionOff;
        }

        /// <summary>
        /// Builds a sample bank from a vehicle audio folder
        /// </summary>
        /// <param name="vehicleId">Vehicle folder id under mod assets</param>
        /// <param name="folder">Absolute path to folder with manifest and wav files</param>
        /// <returns>Loaded bank or null when manifest is missing</returns>
        public static EngineSampleBank Load(string vehicleId, string folder)
        {
            string manifestPath = Path.Combine(folder, "unity_audio_manifest.csv");
            if (!File.Exists(manifestPath))
            {
                ModConsole.Error("[RealisticMotorSound] Manifest not found: " + manifestPath);
                return null;
            }

            var rpmSet = new SortedDictionary<float, byte>();
            var thrSet = new SortedDictionary<float, byte>();
            var loopMeta = new List<LoopMeta>();
            AudioClip startup = null;
            AudioClip ignitionOff = null;

            string[] lines = File.ReadAllLines(manifestPath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0 || i == 0 && line.StartsWith("filename", StringComparison.OrdinalIgnoreCase))
                    continue;

                string[] parts = SplitCsv(line);
                if (parts.Length < 4)
                    continue;

                string fileName = parts[0];
                string type = parts[1];
                string fullPath = Path.Combine(folder, fileName);
                if (!File.Exists(fullPath))
                {
                    ModConsole.Error("[RealisticMotorSound] Missing audio file: " + fullPath);
                    continue;
                }

                AudioClip clip = ModAudio.LoadAudioClipFromFile(fullPath, false);
                if (clip == null)
                {
                    ModConsole.Error("[RealisticMotorSound] Failed to load: " + fullPath);
                    continue;
                }

                clip.name = fileName;

                if (type == "startup")
                {
                    startup = clip;
                    continue;
                }

                if (type == "ignition_off")
                {
                    ignitionOff = clip;
                    continue;
                }

                if (type != "rpm_loop")
                    continue;

                float rpm;
                float thr;
                if (!TryParseFloat(parts[2], out rpm) || !TryParseFloat(parts[3], out thr))
                    continue;

                rpmSet[rpm] = 0;
                thrSet[thr] = 0;
                loopMeta.Add(new LoopMeta(rpm, thr, clip));
            }

            if (loopMeta.Count == 0)
            {
                ModConsole.Error("[RealisticMotorSound] No rpm_loop samples in " + vehicleId);
                return null;
            }

            var rpms = new float[rpmSet.Count];
            rpmSet.Keys.CopyTo(rpms, 0);
            var throttles = new float[thrSet.Count];
            thrSet.Keys.CopyTo(throttles, 0);

            var bank = new EngineSampleBank(vehicleId, rpms, throttles, startup, ignitionOff);
            for (int i = 0; i < loopMeta.Count; i++)
                bank._loops[MakeKey(loopMeta[i].Rpm, loopMeta[i].Throttle)] = loopMeta[i].Clip;

            ModConsole.Print(string.Format(
                "[RealisticMotorSound] Loaded {0}: {1} loops, {2} rpm anchors, {3} throttle anchors",
                vehicleId, loopMeta.Count, rpms.Length, throttles.Length));
            return bank;
        }

        /// <summary>
        /// Returns loop clip for exact RPM/throttle anchors
        /// </summary>
        /// <param name="rpm">RPM anchor</param>
        /// <param name="throttlePercent">Throttle anchor percent</param>
        /// <returns>Clip or null</returns>
        public AudioClip GetLoop(float rpm, float throttlePercent)
        {
            AudioClip clip;
            return _loops.TryGetValue(MakeKey(rpm, throttlePercent), out clip) ? clip : null;
        }

        private static long MakeKey(float rpm, float throttlePercent)
        {
            int r = Mathf.RoundToInt(rpm);
            int t = Mathf.RoundToInt(throttlePercent);
            return ((long)r << 32) | (uint)t;
        }

        private static bool TryParseFloat(string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static string[] SplitCsv(string line)
        {
            return line.Split(',');
        }

        private struct LoopMeta
        {
            public readonly float Rpm;
            public readonly float Throttle;
            public readonly AudioClip Clip;

            public LoopMeta(float rpm, float throttle, AudioClip clip)
            {
                Rpm = rpm;
                Throttle = throttle;
                Clip = clip;
            }
        }
    }
}
