using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Draws on-screen live mix diagnostics
    /// </summary>
    public sealed class DebugHud
    {
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _smallStyle;
        private bool _stylesReady;
        private Vector2 _scroll;

        /// <summary>
        /// Renders HUD for current settings and optional session debug
        /// </summary>
        /// <param name="settings">Shared runtime settings</param>
        /// <param name="info">Active vehicle mix snapshot</param>
        public void Draw(AudioRuntimeSettings settings, MixDebugInfo info)
        {
            if (settings == null || !settings.ShowHud)
                return;

            EnsureStyles();

            float width = 520f;
            float height = Mathf.Min(560f, Screen.height - 24f);
            Rect area = new Rect(12f, 12f, width, height);
            GUI.Box(area, "RealisticMotorSound DEBUG", _boxStyle);

            GUILayout.BeginArea(new Rect(area.x + 8f, area.y + 26f, width - 16f, height - 34f));
            _scroll = GUILayout.BeginScrollView(_scroll);

            Line("Mode", settings.ModeLabel + "  (F7)");
            GUILayout.Label(ModeHelp(settings.Mode), _smallStyle);
            Line("Edit vol", settings.EditTarget + "  (F8, -/=)");
            Line("Custom vol", settings.CustomVolume.ToString("0.00"));
            Line("Vanilla vol", settings.VanillaVolume.ToString("0.00")
                + (settings.Mode == AudioRuntimeSettings.SoundMode.CustomOnly
                    ? "  (ignored in CUSTOM)"
                    : string.Empty));
            Line("Sim Hz", settings.SimHz.ToString());
            Line("Synth Vol", settings.SynthVolume.ToString("0.00"));
            Line("Conv", settings.Convolution.ToString("0.00"));
            Line("+HF", settings.HfGain.ToString("0.0000"));
            Line("~LF noise", settings.AirNoise.ToString("0.000"));
            Line("~HF jitter", settings.Jitter.ToString("0.000"));
            Line("Bass cut", settings.BassCutHz.ToString("0") + " Hz");

            if (info != null)
            {
                GUILayout.Space(6f);
                Line("Vehicle", info.VehicleName);
                Line("Profile", info.ProfileId);
                Line("Backend", info.ActiveBank);
                Line("Live tick", info.SimTickMs.ToString("0.0") + " ms (avg "
                    + info.SimTickAvgMs.ToString("0.0") + ")");
                Line("Live buffer", (info.BufferFill01 * 100f).ToString("0") + "%");
                Line("Underrun", (info.UnderrunRate01 * 100f).ToString("0.00")
                    + "%  (" + info.UnderrunFrames + " frames)");
                string hint = info.UnderrunRate01 > 0.02f
                    ? "  << CPU starve, lower Sim Hz or Restart"
                    : (info.BufferFill01 > 0.25f
                        ? "  ok buffer; if rasp remains = synth grit"
                        : string.Empty);
                if (!string.IsNullOrEmpty(hint))
                    GUILayout.Label(hint, _smallStyle);
                Line("RPM", info.Rpm.ToString("0"));
                Line("Throttle", (info.Throttle01 * 100f).ToString("0") + "%");
                Line("Engine", info.EngineOn ? "ON" : (info.Starter ? "STARTER" : "OFF"));
                Line("SC engine src", info.MutedSources + " | live vol "
                    + info.VanillaThrottleVolField.ToString("0.00") + " / "
                    + info.VanillaNoThrottleVolField.ToString("0.00"));
                Line("Foreign audio", info.ForeignSources + " total, playing "
                    + info.PlayingForeign + "  (F9 dump)");
                if (!string.IsNullOrEmpty(info.ActiveClips))
                    GUILayout.Label(info.ActiveClips, _smallStyle);
            }

            GUILayout.Space(8f);
            GUILayout.Label("F6 HUD | F7 mode | F8 vol target | -/= vol | F9 dump | F10 restart live", _smallStyle);
            GUILayout.Label("CUSTOM = only mod | VANILLA = only stock | BOTH = stock*VanillaVol + mod", _smallStyle);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static string ModeHelp(AudioRuntimeSettings.SoundMode mode)
        {
            switch (mode)
            {
                case AudioRuntimeSettings.SoundMode.CustomOnly:
                    return "Stock engine muted by Harmony. Only live Engine Sim plays.";
                case AudioRuntimeSettings.SoundMode.VanillaOnly:
                    return "Live silent. Stock engine uses Vanilla vol.";
                default:
                    return "Stock scaled by Vanilla vol + live scaled by Custom vol.";
            }
        }

        private void Line(string key, string value)
        {
            GUILayout.Label(key + ": " + value, _labelStyle);
        }

        private void EnsureStyles()
        {
            if (_stylesReady)
                return;

            _boxStyle = new GUIStyle(GUI.skin.box);
            _boxStyle.alignment = TextAnchor.UpperLeft;
            _boxStyle.fontSize = 14;
            _boxStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.fontSize = 13;
            _labelStyle.normal.textColor = Color.white;

            _smallStyle = new GUIStyle(GUI.skin.label);
            _smallStyle.fontSize = 11;
            _smallStyle.wordWrap = true;
            _smallStyle.normal.textColor = new Color(0.85f, 0.9f, 1f);

            _stylesReady = true;
        }
    }
}
