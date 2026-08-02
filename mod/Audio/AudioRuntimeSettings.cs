using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Shared runtime mix and debug controls for live Engine Sim
    /// </summary>
    public sealed class AudioRuntimeSettings
    {
        public enum SoundMode
        {
            CustomOnly = 0,
            VanillaOnly = 1,
            Both = 2
        }

        public enum VolumeTarget
        {
            Custom = 0,
            Vanilla = 1
        }

        public SoundMode Mode = SoundMode.CustomOnly;
        public VolumeTarget EditTarget = VolumeTarget.Custom;
        public float CustomVolume = 1.4f;
        public float VanillaVolume = 0f;
        public bool ShowHud = true;
        public bool VerboseLog = true;
        public float VolumeSmooth = 8f;

        // Live Engine Sim quality / mixer (tuned test preset)
        public int SimHz = 6000;
        public float SynthVolume = 1f;
        public float Convolution = 0.87f;
        public float AirNoise = 0.5f;
        public float Jitter = 0.09f;
        public float HfGain = 0f;
        // 400 kills idle; keep 120 and let live player ease further under 1400 RPM
        public float BassCutHz = 120f;

        public static readonly AudioRuntimeSettings Instance = new AudioRuntimeSettings();

        /// <summary>
        /// Cycles CustomOnly -> VanillaOnly -> Both
        /// </summary>
        public void CycleMode()
        {
            Mode = (SoundMode)(((int)Mode + 1) % 3);
        }

        /// <summary>
        /// Cycles volume edit target between custom and vanilla
        /// </summary>
        public void CycleEditTarget()
        {
            EditTarget = EditTarget == VolumeTarget.Custom
                ? VolumeTarget.Vanilla
                : VolumeTarget.Custom;
        }

        /// <summary>
        /// Applies delta to the currently selected volume target
        /// </summary>
        /// <param name="delta">Volume step</param>
        public void AdjustSelectedVolume(float delta)
        {
            if (EditTarget == VolumeTarget.Custom)
                CustomVolume = Mathf.Clamp(CustomVolume + delta, 0f, 3f);
            else
                VanillaVolume = Mathf.Clamp(VanillaVolume + delta, 0f, 2f);
        }

        /// <summary>
        /// Returns human-readable mode label
        /// </summary>
        public string ModeLabel
        {
            get
            {
                switch (Mode)
                {
                    case SoundMode.CustomOnly: return "CUSTOM";
                    case SoundMode.VanillaOnly: return "VANILLA";
                    default: return "BOTH";
                }
            }
        }
    }
}
