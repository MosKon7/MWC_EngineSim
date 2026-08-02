#include "../include/engine_sim_native.h"

#include <cmath>
#include <cstdint>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

namespace {

void writeU16(std::ofstream &file, std::uint16_t value) {
    file.put(static_cast<char>(value & 0xff));
    file.put(static_cast<char>((value >> 8) & 0xff));
}

void writeU32(std::ofstream &file, std::uint32_t value) {
    file.put(static_cast<char>(value & 0xff));
    file.put(static_cast<char>((value >> 8) & 0xff));
    file.put(static_cast<char>((value >> 16) & 0xff));
    file.put(static_cast<char>((value >> 24) & 0xff));
}

void writeWav(const std::string &path, const std::vector<int16_t> &samples) {
    std::ofstream file(path, std::ios::binary);
    const int sampleRate = 44100;
    const int dataBytes = static_cast<int>(samples.size() * sizeof(int16_t));
    file.write("RIFF", 4);
    writeU32(file, 36 + dataBytes);
    file.write("WAVE", 4);
    file.write("fmt ", 4);
    writeU32(file, 16);
    writeU16(file, 1);
    writeU16(file, 1);
    writeU32(file, sampleRate);
    writeU32(file, sampleRate * 2);
    writeU16(file, 2);
    writeU16(file, 16);
    file.write("data", 4);
    writeU32(file, dataBytes);
    file.write(reinterpret_cast<const char *>(samples.data()), dataBytes);
}

} // namespace

int main(int argc, char **argv) {
    const char *script = argc > 1
        ? argv[1]
        : "assets/engines/sorbet/Sorbet.mr";
    const char *outPath = argc > 2
        ? argv[2]
        : "native_smoke.wav";

    EsmHandle *handle = esm_create(script);
    if (handle == nullptr) {
        std::cerr << "esm_create failed: " << esm_last_error() << '\n';
        return 1;
    }

    esm_set_quality(handle, 7000);

    std::vector<int16_t> pcm;
    pcm.reserve(44100 * 2);
    std::vector<int16_t> chunk(4096);

    const float duration = 2.0f;
    float elapsed = 0.0f;
    const float dt = 0.01f;
    while (elapsed < duration) {
        const float t = elapsed / duration;
        const float rpm = 900.0f + t * 4100.0f;
        const float thr = 0.25f + 0.55f * t;
        esm_set_targets(handle, rpm, thr, 1);
        if (esm_tick(handle, dt) != 0) {
            std::cerr << "esm_tick failed: " << esm_last_error() << '\n';
            esm_destroy(handle);
            return 2;
        }

        for (;;) {
            const int n = esm_read_pcm(handle, chunk.data(), static_cast<int>(chunk.size()));
            if (n <= 0) {
                break;
            }
            pcm.insert(pcm.end(), chunk.begin(), chunk.begin() + n);
        }

        elapsed += dt;
    }

    writeWav(outPath, pcm);
    std::cout << "Wrote " << outPath
              << " samples=" << pcm.size()
              << " lastTickMs=" << esm_last_tick_ms(handle)
              << '\n';

    esm_destroy(handle);
    return 0;
}
