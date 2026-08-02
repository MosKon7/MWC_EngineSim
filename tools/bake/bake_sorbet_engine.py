#!/usr/bin/env python3
"""Bake loopable I4 OHV engine WAV clips from Sorbet.mr tuning targets.

Generates RPM layers for the MSCLoader mixer when Engine Sim exporter
is not available. Output layout matches unity_audio_manifest.csv.
"""

from __future__ import annotations

import argparse
import csv
import math
import struct
import wave
from pathlib import Path


SAMPLE_RATE = 44100
CYLINDERS = 4
FIRING_ORDER = (0, 2, 3, 1)  # 1-3-4-2 as zero-based
# One loop per 100 RPM, single throttle instance for cleaner RPM crossfade
DEFAULT_RPMS = tuple(range(800, 7001, 100))
DEFAULT_THROTTLES = (50,)


def clamp(value: float, lo: float, hi: float) -> float:
    return lo if value < lo else hi if value > hi else value


def soft_clip(x: float) -> float:
    return math.tanh(x * 1.35)


class OnePole:
    def __init__(self, cutoff_hz: float) -> None:
        self.set_cutoff(cutoff_hz)
        self.y = 0.0

    def set_cutoff(self, cutoff_hz: float) -> None:
        c = clamp(cutoff_hz, 20.0, SAMPLE_RATE * 0.45)
        self.a = math.exp(-2.0 * math.pi * c / SAMPLE_RATE)

    def lowpass(self, x: float) -> float:
        self.y = (1.0 - self.a) * x + self.a * self.y
        return self.y

    def highpass(self, x: float) -> float:
        lp = self.lowpass(x)
        return x - lp


def write_wav(path: Path, samples: list[float]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    peak = max(1e-6, max(abs(s) for s in samples))
    norm = 0.92 / peak
    with wave.open(str(path), "wb") as wf:
        wf.setnchannels(1)
        wf.setsampwidth(2)
        wf.setframerate(SAMPLE_RATE)
        frames = bytearray()
        for s in samples:
            v = int(clamp(s * norm, -1.0, 1.0) * 32767.0)
            frames.extend(struct.pack("<h", v))
        wf.writeframes(frames)


def find_quiet_seam(samples: list[float], search_ms: float = 80.0) -> int:
    """Pick loop cut near the quietest window in the last search_ms."""
    n = len(samples)
    window = max(16, int(SAMPLE_RATE * 0.002))
    search = min(n // 3, int(SAMPLE_RATE * search_ms / 1000.0))
    if search <= window:
        return n
    best_i = n
    best_e = 1e9
    start = n - search
    for i in range(start, n - window):
        e = 0.0
        for j in range(window):
            e += abs(samples[i + j])
        if e < best_e:
            best_e = e
            best_i = i
    return best_i


def apply_loop_crossfade(samples: list[float], fade_ms: float = 100.0) -> list[float]:
    cut = find_quiet_seam(samples)
    trimmed = samples[:cut]
    n = len(trimmed)
    fade = min(n // 4, int(SAMPLE_RATE * fade_ms / 1000.0))
    if fade < 8:
        return trimmed
    out = list(trimmed)
    for i in range(fade):
        t = i / float(fade)
        a = 0.5 - 0.5 * math.cos(math.pi * t)
        out[i] = trimmed[i] * a + trimmed[n - fade + i] * (1.0 - a)
    return out[: n - fade]


def synthesize_loop(rpm: float, throttle: float, duration: float) -> list[float]:
    """Synthesize one steady-state engine loop for given RPM and throttle percent."""
    n = int(SAMPLE_RATE * duration)
    fire_hz = (rpm / 60.0) * (CYLINDERS / 2.0)
    crank_hz = rpm / 60.0
    load = clamp(throttle / 100.0, 0.0, 1.0)

    # First character pass: soft OHV, mild exhaust, modest high end
    body_lp = OnePole(420.0 + load * 900.0)
    mid_lp = OnePole(1400.0 + load * 1800.0)
    air_hp = OnePole(1800.0 + load * 1200.0)
    rumble_lp = OnePole(90.0)

    pulse_phase = [0.0] * CYLINDERS
    for i, cyl in enumerate(FIRING_ORDER):
        pulse_phase[cyl] = i / float(CYLINDERS)

    noise_state = 0.1234567
    out = [0.0] * n

    for i in range(n):
        t = i / float(SAMPLE_RATE)
        crank_phase = (t * crank_hz) % 1.0

        pulse = 0.0
        for cyl in range(CYLINDERS):
            cyl_phase = (crank_phase * 0.5 + pulse_phase[cyl]) % 1.0
            d = cyl_phase - 0.25
            if d < -0.5:
                d += 1.0
            elif d > 0.5:
                d -= 1.0
            width = 0.035 + (1.0 - load) * 0.02
            env = math.exp(-(d * d) / (2.0 * width * width))
            atten = (0.9, 1.1, 0.8, 0.9)[cyl]
            pulse += env * atten

        noise_state = (noise_state * 1664525.0 + 1013904223.0) % 4294967296.0
        white = (noise_state / 2147483648.0) - 1.0
        rumble = rumble_lp.lowpass(white) * (0.25 + 0.35 * load)
        intake = air_hp.highpass(white) * (0.04 + 0.18 * load)

        fund = fire_hz
        tone = 0.0
        for h, amp in ((1, 1.0), (2, 0.55), (3, 0.28), (4, 0.16), (6, 0.08)):
            tone += amp * math.sin(2.0 * math.pi * fund * h * t + 0.15 * h)

        jitter = 1.0 + 0.012 * math.sin(2.0 * math.pi * 7.3 * t) + 0.008 * white
        raw = (
            pulse * (0.55 + 0.75 * load) * jitter
            + tone * (0.12 + 0.22 * load)
            + rumble
            + intake
        )

        body = body_lp.lowpass(raw)
        mid = mid_lp.lowpass(raw)
        sample = body * 0.75 + mid * 0.35
        sample *= 0.28 + 0.72 * (0.35 + 0.65 * load)
        sample *= 0.55 + 0.45 * clamp((rpm - 700.0) / 4800.0, 0.0, 1.0)
        out[i] = soft_clip(sample * 0.85)

    return apply_loop_crossfade(out, fade_ms=80.0)


def synthesize_startup(duration: float = 2.5) -> list[float]:
    n = int(SAMPLE_RATE * duration)
    out = [0.0] * n
    for i in range(n):
        t = i / float(SAMPLE_RATE)
        if t < 1.1:
            rpm = 180.0 + t * 220.0
            load = 0.15
        else:
            u = (t - 1.1) / 1.4
            rpm = 400.0 + u * 500.0
            load = 0.25 + 0.2 * u
        fire_hz = (rpm / 60.0) * 2.0
        crank = math.sin(2.0 * math.pi * (rpm / 60.0) * t)
        pulse = max(0.0, math.sin(2.0 * math.pi * fire_hz * t)) ** 8
        starter = 0.0
        if t < 1.15:
            starter = 0.35 * math.sin(2.0 * math.pi * 18.0 * t) * (1.0 - t / 1.15)
        out[i] = soft_clip((pulse * (0.4 + load) + 0.08 * crank + starter) * 0.7)
    fade = int(0.15 * SAMPLE_RATE)
    for i in range(fade):
        out[n - fade + i] *= 1.0 - i / float(fade)
    return out


def synthesize_ignition_off(duration: float = 1.8) -> list[float]:
    n = int(SAMPLE_RATE * duration)
    out = [0.0] * n
    for i in range(n):
        t = i / float(SAMPLE_RATE)
        rpm = max(0.0, 1000.0 * (1.0 - t / duration) ** 1.4)
        if rpm < 40.0:
            out[i] = 0.0
            continue
        fire_hz = (rpm / 60.0) * 2.0
        pulse = max(0.0, math.sin(2.0 * math.pi * fire_hz * t)) ** 10
        out[i] = soft_clip(pulse * 0.35 * (1.0 - t / duration))
    return out


def bake(out_dir: Path, duration: float, rpms: tuple[int, ...], throttles: tuple[int, ...]) -> None:
    out_dir.mkdir(parents=True, exist_ok=True)
    for old in out_dir.glob("*.wav"):
        old.unlink()
    manifest_path = out_dir / "unity_audio_manifest.csv"
    if manifest_path.exists():
        manifest_path.unlink()

    rows: list[dict[str, str]] = []
    prefix = "sorbet_talbot1510"

    startup_name = f"{prefix}_startup_{int(duration)}s.wav"
    write_wav(out_dir / startup_name, synthesize_startup(min(duration, 2.8)))
    rows.append(
        {
            "filename": startup_name,
            "type": "startup",
            "rpm": "",
            "throttle_percent": "",
            "loop": "0",
            "duration_seconds": f"{min(duration, 2.8):.3f}",
        }
    )

    off_name = f"{prefix}_ignition_off_{int(duration)}s.wav"
    write_wav(out_dir / off_name, synthesize_ignition_off(min(duration, 2.0)))
    rows.append(
        {
            "filename": off_name,
            "type": "ignition_off",
            "rpm": "1000",
            "throttle_percent": "",
            "loop": "0",
            "duration_seconds": f"{min(duration, 2.0):.3f}",
        }
    )

    for rpm in rpms:
        for thr in throttles:
            name = f"{prefix}_rpm_{rpm}_throttle_{thr}_loop_{int(duration)}s.wav"
            samples = synthesize_loop(float(rpm), float(thr), duration)
            write_wav(out_dir / name, samples)
            rows.append(
                {
                    "filename": name,
                    "type": "rpm_loop",
                    "rpm": str(rpm),
                    "throttle_percent": str(thr),
                    "loop": "1",
                    "duration_seconds": f"{duration:.3f}",
                }
            )

    with manifest_path.open("w", newline="", encoding="utf-8") as fh:
        writer = csv.DictWriter(
            fh,
            fieldnames=[
                "filename",
                "type",
                "rpm",
                "throttle_percent",
                "loop",
                "duration_seconds",
            ],
        )
        writer.writeheader()
        writer.writerows(rows)

    print(f"Baked {len(rows)} clips into {out_dir}")


def parse_ints(text: str) -> tuple[int, ...]:
    return tuple(int(x.strip()) for x in text.split(",") if x.strip())


def main() -> None:
    parser = argparse.ArgumentParser(description="Bake Sorbet engine audio layers")
    parser.add_argument(
        "--out",
        type=Path,
        default=Path(__file__).resolve().parents[2] / "assets" / "audio" / "sorbet",
    )
    parser.add_argument("--duration", type=float, default=4.0)
    parser.add_argument("--rpm", type=str, default=",".join(str(x) for x in DEFAULT_RPMS))
    parser.add_argument(
        "--throttle", type=str, default=",".join(str(x) for x in DEFAULT_THROTTLES)
    )
    args = parser.parse_args()
    bake(args.out, args.duration, parse_ints(args.rpm), parse_ints(args.throttle))


if __name__ == "__main__":
    main()
