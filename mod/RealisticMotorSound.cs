using System.Reflection;
using CarList;
using Harmony;
using MSCLoader;
using RealisticMotorSound.Audio;
using RealisticMotorSound.Vehicles;
using UnityEngine;

namespace RealisticMotorSound
{
    public class RealisticMotorSound : Mod
    {
        public override string ID => "RealisticMotorSound";
        public override string Name => "RealisticMotorSound";
        public override string Author => "Moss";
        public override string Version => "1.4.2";
        public override string Description => "Live Engine Sim motor audio for My Winter Car";
        public override Game SupportedGames => Game.MyWinterCar;

        private VehicleFinder _vehicleFinder;
        private VehicleAudioRegistry _audioRegistry;
        private readonly DebugHud _hud = new DebugHud();
        private AudioRuntimeSettings _settings;
        private HarmonyInstance _harmony;
        private float _nextRescanTime;
        private float _nextLogTime;

        private SettingsSlider _customVolSlider;
        private SettingsSlider _vanillaVolSlider;
        private SettingsSlider _simHzSlider;
        private SettingsSlider _synthVolSlider;
        private SettingsSlider _convSlider;
        private SettingsSlider _noiseSlider;
        private SettingsSlider _jitterSlider;
        private SettingsSlider _hfSlider;
        private SettingsSlider _bassCutSlider;
        private SettingsCheckBox _hudCheck;
        private SettingsCheckBox _verboseCheck;
        private SettingsDropDownList _modeDrop;

        private SettingsKeybind _keyHud;
        private SettingsKeybind _keyMode;
        private SettingsKeybind _keyVolTarget;
        private SettingsKeybind _keyVolDown;
        private SettingsKeybind _keyVolUp;
        private SettingsKeybind _keyDump;
        private SettingsKeybind _keyRestart;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.OnGUI, Mod_OnGUI);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
        }

        private void Mod_Settings()
        {
            Settings.AddHeader("Audio mix");
            _modeDrop = Settings.AddDropDownList(
                "sound_mode",
                "Sound source",
                new[] { "CUSTOM only mod", "VANILLA only stock", "BOTH mix" },
                0,
                OnModeChanged);
            Settings.AddText("BOTH = stock*VanillaVol + mod*CustomVol. In CUSTOM stock is force-muted.");
            _customVolSlider = Settings.AddSlider("custom_vol", "Custom volume", 0f, 3f, 1.4f, OnCustomVolChanged, 2);
            _vanillaVolSlider = Settings.AddSlider("vanilla_vol", "Vanilla volume", 0f, 2f, 0f, OnVanillaVolChanged, 2);

            Settings.AddHeader("Live Engine Sim");
            _simHzSlider = Settings.AddSlider("sim_hz", "Sim Hz", 500f, 18000f, 6000f, OnSimHzChanged, 0);
            Settings.AddText("Sweet spot ~6k. 8–12k if underrun stays near 0%. 18k only if tick avg < 8ms.");
            _synthVolSlider = Settings.AddSlider("synth_vol", "Synth Vol", 0.05f, 2f, 1f, OnSynthMixChanged, 2);
            _convSlider = Settings.AddSlider("synth_conv", "Convolution", 0f, 1f, 0.87f, OnSynthMixChanged, 2);
            _hfSlider = Settings.AddSlider("synth_hf", "HF (+HF)", 0f, 0.01f, 0f, OnSynthMixChanged, 4);
            _noiseSlider = Settings.AddSlider("synth_noise", "Air noise (~LF)", 0f, 0.5f, 0.5f, OnSynthMixChanged, 3);
            _jitterSlider = Settings.AddSlider("synth_jitter", "Jitter (~HF)", 0f, 0.5f, 0.09f, OnSynthMixChanged, 3);
            _bassCutSlider = Settings.AddSlider("bass_cut_hz", "Bass cut Hz", 0f, 400f, 120f, OnSynthMixChanged, 0);
            Settings.AddText("Bass cut >200 Hz makes idle thin. Live eases cut under 1400 RPM automatically.");
            Settings.AddButton("Restart live sim", OnRestartLiveClicked);
            Settings.AddText("Recreates Engine Sim if audio dies after Sim Hz / mix changes. Hotkey F10.");

            Settings.AddHeader("Debug");
            _hudCheck = Settings.AddCheckBox("show_hud", "Show HUD", true, OnHudChanged);
            _verboseCheck = Settings.AddCheckBox("verbose_log", "Verbose log (2s)", true, OnVerboseChanged);

            Settings.AddHeader("Hotkeys");
            Settings.AddText("Also work while driving. Values sync with sliders above.");
            _keyHud = Keybind.Add("key_hud", "Toggle HUD", KeyCode.F6);
            _keyMode = Keybind.Add("key_mode", "Cycle sound source", KeyCode.F7);
            _keyVolTarget = Keybind.Add("key_vol_target", "Cycle volume target", KeyCode.F8);
            _keyVolDown = Keybind.Add("key_vol_down", "Volume down", KeyCode.Minus);
            _keyVolUp = Keybind.Add("key_vol_up", "Volume up", KeyCode.Equals);
            _keyDump = Keybind.Add("key_dump", "Dump mute sources", KeyCode.F9);
            _keyRestart = Keybind.Add("key_restart_live", "Restart live sim", KeyCode.F10);
        }

        private void Mod_SettingsLoaded()
        {
            _settings = AudioRuntimeSettings.Instance;
            ApplySettingsFromUi();
            ModConsole.Print(string.Format(
                "[RealisticMotorSound] Settings loaded mode={0} custom={1:0.00} simHz={2} bassCut={3:0}",
                _settings.ModeLabel, _settings.CustomVolume, _settings.SimHz, _settings.BassCutHz));
        }

        private void Mod_OnLoad()
        {
            _settings = AudioRuntimeSettings.Instance;
            ApplySettingsFromUi();

            ModConsole.Print(Name + " v" + Version + " loading...");
            ModConsole.Print("[RealisticMotorSound] Hotkeys: F6 HUD, F7 mode, F8 vol target, -/= vol, F9 dump, F10 restart live");

            try
            {
                _harmony = HarmonyInstance.Create("Moss.RealisticMotorSound");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                ModConsole.Print("[RealisticMotorSound] Harmony patched SoundController.FixedUpdate");
            }
            catch (System.Exception ex)
            {
                ModConsole.Error("[RealisticMotorSound] Harmony patch failed: " + ex.Message);
            }

            _vehicleFinder = new VehicleFinder();
            _vehicleFinder.InitializeOnce();

            _audioRegistry = new VehicleAudioRegistry();
            _audioRegistry.Initialize(this);
            _audioRegistry.BindNewVehicles(_vehicleFinder);

            _nextRescanTime = Time.time + 2f;
            _nextLogTime = Time.time + 2f;

            // End-of-frame re-silence: PlayMaker may rewrite AudioSource.volume after LateUpdate
            EngineAudioDriver.Create(OnDriverUpdate, OnDriverLateUpdate, OnDriverEndOfFrame);
            ModConsole.Print(Name + " ready");
        }

        private void Mod_OnGUI()
        {
            if (_settings == null || !_settings.ShowHud)
                return;

            MixDebugInfo info = null;
            if (_audioRegistry != null && _audioRegistry.PrimarySession != null)
                info = _audioRegistry.PrimarySession.DebugInfo;

            _hud.Draw(_settings, info);
        }

        private void OnDriverUpdate()
        {
            if (_settings == null)
                _settings = AudioRuntimeSettings.Instance;

            HandleHotkeys();

            if (_vehicleFinder == null || _audioRegistry == null)
                return;

            if (Time.time >= _nextRescanTime)
            {
                _vehicleFinder.RefreshAll();
                _audioRegistry.BindNewVehicles(_vehicleFinder);
                _nextRescanTime = Time.time + 3f;
            }

            if (_settings.VerboseLog && Time.time >= _nextLogTime)
            {
                LogSnapshot();
                _nextLogTime = Time.time + 2f;
            }
        }

        private void OnDriverLateUpdate()
        {
            if (_audioRegistry != null && _settings != null)
                _audioRegistry.Tick(_settings);
        }

        private void OnDriverEndOfFrame()
        {
            if (_audioRegistry != null && _settings != null)
                _audioRegistry.TickMuteOnly(_settings);
        }

        private void HandleHotkeys()
        {
            if (GetDown(_keyHud, KeyCode.F6))
            {
                _settings.ShowHud = !_settings.ShowHud;
                if (_hudCheck != null)
                    _hudCheck.SetValue(_settings.ShowHud);
                ModConsole.Print("[RealisticMotorSound] HUD=" + _settings.ShowHud);
            }

            if (GetDown(_keyMode, KeyCode.F7))
            {
                _settings.CycleMode();
                if (_modeDrop != null)
                    _modeDrop.SetSelectedItemIndex((int)_settings.Mode);
                ModConsole.Print("[RealisticMotorSound] Mode=" + _settings.ModeLabel);
            }

            if (GetDown(_keyVolTarget, KeyCode.F8))
            {
                _settings.CycleEditTarget();
                ModConsole.Print("[RealisticMotorSound] EditTarget=" + _settings.EditTarget);
            }

            if (GetDown(_keyVolDown, KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                _settings.AdjustSelectedVolume(-0.1f);
                PushVolumesToUi();
                ModConsole.Print(string.Format(
                    "[RealisticMotorSound] {0} vol={1:0.00}",
                    _settings.EditTarget,
                    _settings.EditTarget == AudioRuntimeSettings.VolumeTarget.Custom
                        ? _settings.CustomVolume
                        : _settings.VanillaVolume));
            }

            if (GetDown(_keyVolUp, KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                _settings.AdjustSelectedVolume(0.1f);
                PushVolumesToUi();
                ModConsole.Print(string.Format(
                    "[RealisticMotorSound] {0} vol={1:0.00}",
                    _settings.EditTarget,
                    _settings.EditTarget == AudioRuntimeSettings.VolumeTarget.Custom
                        ? _settings.CustomVolume
                        : _settings.VanillaVolume));
            }

            if (GetDown(_keyDump, KeyCode.F9))
            {
                if (_audioRegistry != null)
                    _audioRegistry.DumpMuteState();
            }

            if (GetDown(_keyRestart, KeyCode.F10))
                RestartLiveSim();
        }

        private void OnRestartLiveClicked()
        {
            RestartLiveSim();
        }

        private void RestartLiveSim()
        {
            if (_settings == null)
                _settings = AudioRuntimeSettings.Instance;
            ApplySettingsFromUi();

            if (_audioRegistry == null)
            {
                ModConsole.Error("[RealisticMotorSound] Restart skipped: registry not ready");
                return;
            }

            int ok = _audioRegistry.RestartLive(_settings);
            ModConsole.Print(string.Format(
                "[RealisticMotorSound] Live restart ok={0}/{1} simHz={2}",
                ok,
                _audioRegistry.SessionCount,
                _settings.SimHz));
        }

        private void LogSnapshot()
        {
            VehicleEngineSession session = _audioRegistry != null ? _audioRegistry.PrimarySession : null;
            if (session == null || session.DebugInfo == null)
            {
                ModConsole.Print("[RealisticMotorSound] No active session yet");
                return;
            }

            MixDebugInfo d = session.DebugInfo;
            ModConsole.Print(string.Format(
                "[RMS] mode={0} live sim={1} hp={2:0} cVol={3:0.00} rpm={4:0} thr={5:0}% eng={6} tick={7:0.0}/{8:0.0}ms buf={9:0}% und={10:0.0}%",
                _settings.ModeLabel,
                _settings.SimHz,
                _settings.BassCutHz,
                _settings.CustomVolume,
                d.Rpm,
                d.Throttle01 * 100f,
                d.EngineOn ? "on" : (d.Starter ? "starter" : "off"),
                d.SimTickMs,
                d.SimTickAvgMs,
                d.BufferFill01 * 100f,
                d.UnderrunRate01 * 100f));
        }

        private void ApplySettingsFromUi()
        {
            if (_settings == null)
                _settings = AudioRuntimeSettings.Instance;

            if (_modeDrop != null)
                _settings.Mode = (AudioRuntimeSettings.SoundMode)_modeDrop.GetSelectedItemIndex();
            if (_customVolSlider != null)
                _settings.CustomVolume = _customVolSlider.GetValue();
            if (_vanillaVolSlider != null)
                _settings.VanillaVolume = _vanillaVolSlider.GetValue();
            if (_simHzSlider != null)
                _settings.SimHz = (int)_simHzSlider.GetValue();
            ApplySynthFromUi();
            if (_hudCheck != null)
                _settings.ShowHud = _hudCheck.GetValue();
            if (_verboseCheck != null)
                _settings.VerboseLog = _verboseCheck.GetValue();
        }

        private void ApplySynthFromUi()
        {
            if (_settings == null)
                return;
            if (_synthVolSlider != null)
                _settings.SynthVolume = _synthVolSlider.GetValue();
            if (_convSlider != null)
                _settings.Convolution = _convSlider.GetValue();
            if (_noiseSlider != null)
                _settings.AirNoise = _noiseSlider.GetValue();
            if (_jitterSlider != null)
                _settings.Jitter = _jitterSlider.GetValue();
            if (_hfSlider != null)
                _settings.HfGain = _hfSlider.GetValue();
            if (_bassCutSlider != null)
                _settings.BassCutHz = _bassCutSlider.GetValue();
        }

        private void PushVolumesToUi()
        {
            if (_customVolSlider != null)
                _customVolSlider.SetValue(_settings.CustomVolume);
            if (_vanillaVolSlider != null)
                _vanillaVolSlider.SetValue(_settings.VanillaVolume);
        }

        private void OnModeChanged()
        {
            if (_settings == null || _modeDrop == null)
                return;
            _settings.Mode = (AudioRuntimeSettings.SoundMode)_modeDrop.GetSelectedItemIndex();
        }

        private void OnCustomVolChanged()
        {
            if (_settings == null || _customVolSlider == null)
                return;
            _settings.CustomVolume = _customVolSlider.GetValue();
        }

        private void OnVanillaVolChanged()
        {
            if (_settings == null || _vanillaVolSlider == null)
                return;
            _settings.VanillaVolume = _vanillaVolSlider.GetValue();
        }

        private void OnSimHzChanged()
        {
            if (_settings == null || _simHzSlider == null)
                return;
            _settings.SimHz = (int)_simHzSlider.GetValue();
        }

        private void OnSynthMixChanged()
        {
            if (_settings == null)
                return;
            ApplySynthFromUi();
        }

        private void OnHudChanged()
        {
            if (_settings == null || _hudCheck == null)
                return;
            _settings.ShowHud = _hudCheck.GetValue();
        }

        private void OnVerboseChanged()
        {
            if (_settings == null || _verboseCheck == null)
                return;
            _settings.VerboseLog = _verboseCheck.GetValue();
        }

        private static bool GetDown(SettingsKeybind bind, KeyCode fallback)
        {
            if (bind != null && bind.GetKeybindDown())
                return true;
            return Input.GetKeyDown(fallback);
        }
    }
}
