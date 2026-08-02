using System.Reflection;
using Harmony;
using MSCLoader;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Forces stock engine volumes after SoundController.FixedUpdate
    /// </summary>
    [HarmonyPatch(typeof(SoundController), "FixedUpdate")]
    internal static class SoundControllerMutePatch
    {
        private static readonly FieldInfo ThrottleSourceField =
            typeof(SoundController).GetField("engineThrottleSource", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo NoThrottleSourceField =
            typeof(SoundController).GetField("engineNoThrottleSource", BindingFlags.Instance | BindingFlags.NonPublic);

        private static bool _logged;
        private static int _applyCount;

        /// <summary>
        /// Applies vanilla volume scale after the game writes engine AudioSource volumes
        /// </summary>
        /// <param name="__instance">Patched SoundController</param>
        [HarmonyPostfix]
        private static void Postfix(SoundController __instance)
        {
            if (__instance == null)
                return;

            AudioRuntimeSettings settings = AudioRuntimeSettings.Instance;
            if (settings == null)
                return;

            if (!VehicleMuteGate.ShouldControl(__instance))
                return;

            bool silence = settings.Mode == AudioRuntimeSettings.SoundMode.CustomOnly
                || settings.VanillaVolume <= 0.0001f;
            float scale = silence ? 0f : Mathf.Clamp(settings.VanillaVolume, 0f, 2f);

            ApplySource(ThrottleSourceField, __instance, scale, silence);
            ApplySource(NoThrottleSourceField, __instance, scale, silence);

            if (silence)
            {
                __instance.engineThrottleVolume = 0f;
                __instance.engineNoThrottleVolume = 0f;
            }

            if (!_logged)
            {
                _logged = true;
                ModConsole.Print("[RealisticMotorSound] Harmony mute patch active on SoundController.FixedUpdate");
            }

            _applyCount++;
            if (settings.VerboseLog && _applyCount % 100 == 0)
            {
                AudioSource thr = ThrottleSourceField != null
                    ? ThrottleSourceField.GetValue(__instance) as AudioSource
                    : null;
                AudioSource no = NoThrottleSourceField != null
                    ? NoThrottleSourceField.GetValue(__instance) as AudioSource
                    : null;
                ModConsole.Print(string.Format(
                    "[RMS-mute] silence={0} thrVol={1:0.000} noVol={2:0.000} thrMute={3} noMute={4}",
                    silence,
                    thr != null ? thr.volume : -1f,
                    no != null ? no.volume : -1f,
                    thr != null && thr.mute,
                    no != null && no.mute));
            }
        }

        private static void ApplySource(FieldInfo field, SoundController sc, float scale, bool silence)
        {
            if (field == null)
                return;

            AudioSource source = field.GetValue(sc) as AudioSource;
            if (source == null)
                return;

            if (silence)
            {
                source.volume = 0f;
                source.mute = true;
                source.pitch = 0.01f;
                if (source.isPlaying)
                    source.Stop();
            }
            else
            {
                source.mute = false;
                source.volume = Mathf.Clamp01(source.volume * scale);
                if (!source.isPlaying && source.clip != null)
                    source.Play();
            }
        }
    }

    /// <summary>
    /// Tracks which SoundController instances are owned by the mod
    /// </summary>
    internal static class VehicleMuteGate
    {
        private static readonly System.Collections.Generic.HashSet<int> Controlled =
            new System.Collections.Generic.HashSet<int>();

        /// <summary>
        /// Marks a SoundController as managed by RealisticMotorSound
        /// </summary>
        /// <param name="soundController">Vehicle sound controller</param>
        public static void Register(SoundController soundController)
        {
            if (soundController == null)
                return;

            int id = soundController.GetInstanceID();
            if (Controlled.Add(id))
            {
                ModConsole.Print("[RealisticMotorSound] Mute gate register: "
                    + soundController.gameObject.name + " id=" + id);
            }
        }

        /// <summary>
        /// Checks whether mute policy should run for this controller
        /// </summary>
        /// <param name="soundController">Candidate controller</param>
        /// <returns>True when registered or Sorbet by name</returns>
        public static bool ShouldControl(SoundController soundController)
        {
            if (soundController == null)
                return false;

            if (Controlled.Contains(soundController.GetInstanceID()))
                return true;

            // Fallback if session registered a different component instance
            string n = soundController.gameObject.name;
            if (n != null && n.ToUpperInvariant().Contains("SORBET"))
            {
                Register(soundController);
                return true;
            }

            Transform root = soundController.transform.root;
            if (root != null && root.name.ToUpperInvariant().Contains("SORBET"))
            {
                Register(soundController);
                return true;
            }

            return false;
        }
    }
}
