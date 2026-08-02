# MWC EngineSim / RealisticMotorSound

Live Engine Sim audio for **My Winter Car** (MSCLoader mod). Physics-driven motor sound via `EngineSimNative.dll` + `OnAudioFilterRead`.

Current test baseline: **v1.4.2** (Sorbet / Talbot 1510).

## Layout

| Path | Contents |
|---|---|
| `mod/` | C# MSCLoader mod sources |
| `tools/engine-sim-sound-exporter/` | Headless Engine Sim + `EngineSimNative` C ABI |
| `assets/engines/sorbet/Sorbet.mr` | Engine script |
| `assets/audio/` | Legacy baked loops (fallback / reference) |

## In-game

- Deploy: `Mods/RealisticMotorSound.dll`
- Native + script: `Mods/Assets/RealisticMotorSound/engine_sim/` (`EngineSimNative.dll`, Boost DLLs, `Sorbet.mr`, `es/`)
- Hotkeys: F6 HUD, F7 mode, F8 vol target, -/= volume, F9 mute dump, **F10 restart live**

### Tuned mix preset (defaults)

- Sim Hz `6000`, Synth Vol `1`, Convolution `0.87`, HF `0`
- Air noise `0.5`, Jitter `0.09`, Bass cut `120` (eased under 1400 RPM)

## Build

### Mod

```powershell
msbuild mod\RealisticMotorSound.csproj /p:Configuration=Release
```

### Native

```powershell
# after vcvars64 + existing CMake build dir
cmake --build tools\engine-sim-sound-exporter\build-exporter --target EngineSimNative
```

Copy `EngineSimNative.dll` into the game `engine_sim` assets folder.

## Notes

- Do not put native/Boost DLLs in `Mods/` root (MSCLoader loads them as managed mods).
- `tools/vcpkg` and Engine Sim submodules are gitignored; bootstrap locally to rebuild native.
