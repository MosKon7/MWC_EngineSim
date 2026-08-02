#include "../include/engine_sim_native.h"

#include "../include/engine.h"
#include "../include/piston_engine_simulator.h"
#include "../include/simulator.h"
#include "../include/transmission.h"
#include "../include/units.h"
#include "../include/vehicle.h"
#include "../scripting/include/compiler.h"

#include <algorithm>
#include <chrono>
#include <cmath>
#include <cstdint>
#include <cstring>
#include <filesystem>
#include <fstream>
#include <limits>
#include <mutex>
#include <string>
#include <vector>

namespace {

constexpr int kAudioSampleRate = 44100;
constexpr float kMaxTickSeconds = 0.05f;
constexpr int kDefaultSimHz = 18000;
constexpr float kAudioDt = 1.0f / static_cast<float>(kAudioSampleRate);

thread_local std::string g_lastError = "ok";

void setError(const std::string &message) {
    g_lastError = message;
}

struct PcmWave {
    int sampleRate = 0;
    std::vector<int16_t> samples;
};

std::uint16_t readU16(std::ifstream &file) {
    unsigned char bytes[2] = {};
    file.read(reinterpret_cast<char *>(bytes), sizeof(bytes));
    return static_cast<std::uint16_t>(
        static_cast<std::uint16_t>(bytes[0])
        | (static_cast<std::uint16_t>(bytes[1]) << 8));
}

std::uint32_t readU32(std::ifstream &file) {
    unsigned char bytes[4] = {};
    file.read(reinterpret_cast<char *>(bytes), sizeof(bytes));
    return
        static_cast<std::uint32_t>(bytes[0])
        | (static_cast<std::uint32_t>(bytes[1]) << 8)
        | (static_cast<std::uint32_t>(bytes[2]) << 16)
        | (static_cast<std::uint32_t>(bytes[3]) << 24);
}

std::string readFourCc(std::ifstream &file) {
    char id[4] = {};
    file.read(id, sizeof(id));
    return std::string(id, id + 4);
}

PcmWave readPcm16Wave(const std::filesystem::path &path) {
    std::ifstream file(path, std::ios::binary);
    if (!file) {
        throw std::runtime_error("Could not open impulse response: " + path.string());
    }

    if (readFourCc(file) != "RIFF") {
        throw std::runtime_error("Invalid WAV RIFF header: " + path.string());
    }

    readU32(file);
    if (readFourCc(file) != "WAVE") {
        throw std::runtime_error("Invalid WAV WAVE header: " + path.string());
    }

    int sampleRate = 0;
    int channels = 0;
    int bitsPerSample = 0;
    std::vector<int16_t> samples;

    while (file && !file.eof()) {
        const std::string chunkId = readFourCc(file);
        if (!file) {
            break;
        }

        const std::uint32_t chunkSize = readU32(file);
        if (chunkId == "fmt ") {
            const std::uint16_t audioFormat = readU16(file);
            channels = readU16(file);
            sampleRate = static_cast<int>(readU32(file));
            readU32(file);
            readU16(file);
            bitsPerSample = readU16(file);
            if (chunkSize > 16) {
                file.seekg(chunkSize - 16, std::ios::cur);
            }

            if (audioFormat != 1 || bitsPerSample != 16) {
                throw std::runtime_error("Impulse response must be PCM16: " + path.string());
            }
        }
        else if (chunkId == "data") {
            samples.resize(chunkSize / sizeof(int16_t));
            file.read(
                reinterpret_cast<char *>(samples.data()),
                static_cast<std::streamsize>(chunkSize));
        }
        else {
            file.seekg(chunkSize, std::ios::cur);
        }
    }

    if (samples.empty() || sampleRate <= 0 || channels <= 0) {
        throw std::runtime_error("Empty impulse response: " + path.string());
    }

    if (channels > 1) {
        std::vector<int16_t> mono;
        mono.reserve(samples.size() / static_cast<size_t>(channels));
        for (size_t i = 0; i + static_cast<size_t>(channels) <= samples.size(); i += channels) {
            int sum = 0;
            for (int c = 0; c < channels; ++c) {
                sum += samples[i + static_cast<size_t>(c)];
            }
            mono.push_back(static_cast<int16_t>(sum / channels));
        }
        samples.swap(mono);
    }

    PcmWave wave;
    wave.sampleRate = sampleRate;
    wave.samples = std::move(samples);
    return wave;
}

void addFallbackVehicleAndTransmission(Engine *engine, Vehicle **vehicle, Transmission **transmission) {
    if (*vehicle == nullptr) {
        Vehicle::Parameters vehParams;
        vehParams.mass = units::mass(1500, units::kg);
        vehParams.diffRatio = 3.9;
        vehParams.tireRadius = units::distance(12, units::inch);
        vehParams.dragCoefficient = 0.35;
        vehParams.crossSectionArea =
            units::distance(65, units::inch) * units::distance(55, units::inch);
        vehParams.rollingResistance = 280.0;
        *vehicle = new Vehicle;
        (*vehicle)->initialize(vehParams);
    }

    if (*transmission == nullptr) {
        const double gearRatios[] = { 3.55, 2.16, 1.40, 1.03, 0.82 };
        Transmission::Parameters tParams;
        tParams.GearCount = 5;
        tParams.GearRatios = gearRatios;
        tParams.MaxClutchTorque = units::torque(180.0, units::ft_lb);
        *transmission = new Transmission;
        (*transmission)->initialize(tParams);
    }

    (void)engine;
}

void initializeImpulseResponses(Simulator *simulator, Engine *engine) {
    for (int i = 0; i < engine->getExhaustSystemCount(); ++i) {
        ImpulseResponse *response = engine->getExhaustSystem(i)->getImpulseResponse();
        if (response == nullptr) {
            continue;
        }

        const PcmWave wave = readPcm16Wave(response->getFilename());
        simulator->synthesizer().initializeImpulseResponse(
            wave.samples.data(),
            static_cast<unsigned int>(wave.samples.size()),
            static_cast<float>(response->getVolume()),
            i);
    }
}

} // namespace

struct EsmHandle {
    Engine *engine = nullptr;
    Vehicle *vehicle = nullptr;
    Transmission *transmission = nullptr;
    Simulator *simulator = nullptr;
    std::mutex mutex;
    float targetRpm = 900.0f;
    float targetThrottle = 0.2f;
    bool ignitionOn = true;
    bool started = false;
    int simHz = kDefaultSimHz;
    float lastTickMs = 0.0f;
    // One-pole high-pass (x - LPF) for realtime bass cut
    float highpassHz = 80.0f;
    float hpLp = 0.0f;
    float smoothedThrottle = 0.28f;
};

static void applyTargetsLocked(EsmHandle *handle) {
    const float rpm = std::max(0.0f, handle->targetRpm);
    float throttle = std::clamp(handle->targetThrottle, 0.0f, 1.0f);

    handle->engine->getIgnitionModule()->m_enabled = handle->ignitionOn;

    if (!handle->ignitionOn || rpm < 350.0f) {
        handle->simulator->m_starterMotor.m_enabled = false;
        handle->simulator->m_dyno.m_enabled = false;
        handle->simulator->m_dyno.m_hold = false;
        handle->smoothedThrottle = 0.28f;
        handle->engine->setSpeedControl(0.0);
        return;
    }

    // Soft idle floor + smoothed speedControl avoid coast↔throttle jumps
    float targetSc = throttle;
    if (targetSc < 0.28f) {
        targetSc = 0.28f;
    }

    handle->smoothedThrottle +=
        (targetSc - handle->smoothedThrottle) * 0.12f;
    handle->engine->setSpeedControl(handle->smoothedThrottle);

    handle->simulator->m_starterMotor.m_enabled = false;
    handle->simulator->m_dyno.m_enabled = true;
    handle->simulator->m_dyno.m_hold = true;
    handle->simulator->m_dyno.m_rotationSpeed = units::rpm(rpm);
}

static void drainDiscard(Simulator *simulator) {
    std::vector<int16_t> temp(4096);
    while (simulator->synthesizer().m_audioBuffer.size() > 0) {
        const int n = std::min<int>(
            static_cast<int>(simulator->synthesizer().m_audioBuffer.size()),
            static_cast<int>(temp.size()));
        if (simulator->readAudioOutput(n, temp.data()) <= 0) {
            break;
        }
    }
}

static void advanceSeconds(EsmHandle *handle, double seconds) {
    double elapsed = 0.0;
    while (elapsed < seconds) {
        const double frame = std::min(0.01, seconds - elapsed);
        drainDiscard(handle->simulator);
        handle->simulator->startFrame(frame);
        while (handle->simulator->simulateStep()) {
        }
        handle->simulator->endFrame();
        while (handle->simulator->synthesizer().renderAudioNonblocking()) {
        }
        drainDiscard(handle->simulator);
        elapsed += frame;
    }
}

static void warmup(EsmHandle *handle) {
    handle->engine->setSpeedControl(0.15);
    handle->engine->getIgnitionModule()->m_enabled = true;
    handle->simulator->m_starterMotor.m_enabled = true;
    handle->simulator->m_dyno.m_enabled = false;
    advanceSeconds(handle, 0.8);

    handle->simulator->m_starterMotor.m_enabled = false;
    handle->simulator->m_dyno.m_enabled = true;
    handle->simulator->m_dyno.m_hold = true;
    handle->simulator->m_dyno.m_rotationSpeed = units::rpm(900);
    handle->engine->setSpeedControl(0.2);
    advanceSeconds(handle, 0.6);
    handle->started = true;
}

extern "C" {

ESM_API EsmHandle *esm_create(const char *scriptPath) {
    setError("ok");
    if (scriptPath == nullptr || scriptPath[0] == '\0') {
        setError("scriptPath is empty");
        return nullptr;
    }

    try {
        es_script::Compiler compiler;
        compiler.initialize();
        *es_script::Compiler::output() = es_script::Compiler::Output();

        if (!compiler.compile(scriptPath)) {
            compiler.destroy();
            setError(std::string("Could not compile script: ") + scriptPath + " (see error_log.log)");
            return nullptr;
        }

        const es_script::Compiler::Output output = compiler.execute();
        compiler.destroy();

        if (output.engine == nullptr) {
            setError("Script did not create an engine");
            return nullptr;
        }

        auto *handle = new EsmHandle();
        handle->engine = output.engine;
        handle->vehicle = output.vehicle;
        handle->transmission = output.transmission;
        addFallbackVehicleAndTransmission(handle->engine, &handle->vehicle, &handle->transmission);

        handle->simulator = handle->engine->createSimulator(handle->vehicle, handle->transmission);
        handle->engine->calculateDisplacement();

        handle->simHz = kDefaultSimHz;
        handle->simulator->setSimulationFrequency(handle->simHz);
        handle->simulator->synthesizer().setInputSampleRate(static_cast<double>(handle->simHz));

        Synthesizer::AudioParameters audioParams =
            handle->simulator->synthesizer().getAudioParameters();
        // Live defaults: script noise/jitter are tuned for offline export loudness
        // and sound raspy when streamed into Unity at game volumes.
        audioParams.volume = 0.65f;
        audioParams.inputSampleNoise = std::min(
            0.05f, static_cast<float>(handle->engine->getInitialJitter()));
        audioParams.airNoise = std::min(
            0.08f, static_cast<float>(handle->engine->getInitialNoise()));
        audioParams.dF_F_mix = std::min(
            0.002f, static_cast<float>(handle->engine->getInitialHighFrequencyGain()));
        audioParams.convolution = 0.75f;
        // Tight leveler: wide min/max inverts loudness (WOT ducked, coast boosted)
        audioParams.levelerMaxGain = 1.05f;
        audioParams.levelerMinGain = 0.85f;
        handle->simulator->synthesizer().setAudioParameters(audioParams);

        initializeImpulseResponses(handle->simulator, handle->engine);
        warmup(handle);
        return handle;
    }
    catch (const std::exception &ex) {
        setError(ex.what());
        return nullptr;
    }
    catch (...) {
        setError("Unknown error in esm_create");
        return nullptr;
    }
}

ESM_API void esm_destroy(EsmHandle *handle) {
    if (handle == nullptr) {
        return;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    if (handle->simulator != nullptr) {
        handle->simulator->endAudioRenderingThread();
        handle->simulator->destroy();
        delete handle->simulator;
        handle->simulator = nullptr;
    }

    if (handle->engine != nullptr) {
        handle->engine->destroy();
        delete handle->engine;
        handle->engine = nullptr;
    }

    delete handle->transmission;
    delete handle->vehicle;
    delete handle;
}

ESM_API void esm_set_targets(EsmHandle *handle, float rpm, float throttle01, int ignitionOn) {
    if (handle == nullptr) {
        return;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    handle->targetRpm = rpm;
    handle->targetThrottle = throttle01;
    handle->ignitionOn = ignitionOn != 0;
}

ESM_API int esm_tick(EsmHandle *handle, float dtSeconds) {
    if (handle == nullptr) {
        setError("null handle");
        return 1;
    }

    try {
        std::lock_guard<std::mutex> lock(handle->mutex);
        const auto t0 = std::chrono::steady_clock::now();

        float dt = dtSeconds;
        if (dt < 0.0005f) {
            dt = 0.0005f;
        }
        if (dt > kMaxTickSeconds) {
            dt = kMaxTickSeconds;
        }

        applyTargetsLocked(handle);

        handle->simulator->startFrame(dt);
        while (handle->simulator->simulateStep()) {
        }
        handle->simulator->endFrame();
        while (handle->simulator->synthesizer().renderAudioNonblocking()) {
        }

        const auto t1 = std::chrono::steady_clock::now();
        handle->lastTickMs = std::chrono::duration<float, std::milli>(t1 - t0).count();
        return 0;
    }
    catch (const std::exception &ex) {
        setError(ex.what());
        return 2;
    }
}

ESM_API int esm_read_pcm(EsmHandle *handle, int16_t *dst, int maxSamples) {
    if (handle == nullptr || dst == nullptr || maxSamples <= 0) {
        return 0;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    const int available = static_cast<int>(handle->simulator->synthesizer().m_audioBuffer.size());
    const int n = std::min(available, maxSamples);
    if (n <= 0) {
        return 0;
    }

    const int read = handle->simulator->readAudioOutput(n, dst);
    const bool useHp = handle->highpassHz > 1.0f;
    const float hpAlpha = useHp
        ? (kAudioDt / (kAudioDt + 1.0f / (handle->highpassHz * 2.0f
            * static_cast<float>(3.14159265358979323846))))
        : 0.0f;

    // Soft-clip to kill harsh peaks that read as rasp/crackle in Unity
    for (int i = 0; i < read; ++i) {
        float x = dst[i] / 32768.0f;
        if (useHp) {
            handle->hpLp += hpAlpha * (x - handle->hpLp);
            x -= handle->hpLp;
        }
        const float y = std::tanh(x * 1.35f) * 0.92f;
        const long v = std::lround(y * 32767.0f);
        dst[i] = static_cast<int16_t>(std::clamp<long>(
            v,
            std::numeric_limits<int16_t>::min(),
            std::numeric_limits<int16_t>::max()));
    }
    return read;
}

ESM_API void esm_set_quality(EsmHandle *handle, int simHz) {
    if (handle == nullptr) {
        return;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    handle->simHz = std::clamp(simHz, 500, 18000);
    handle->simulator->setSimulationFrequency(handle->simHz);
    handle->simulator->synthesizer().setInputSampleRate(static_cast<double>(handle->simHz));

    // Keep fluid work bounded so high SimHz can still meet realtime
    int fluidSteps = 8;
    if (handle->simHz >= 14000) {
        fluidSteps = 3;
    }
    else if (handle->simHz >= 10000) {
        fluidSteps = 4;
    }
    else if (handle->simHz >= 8000) {
        fluidSteps = 5;
    }
    else if (handle->simHz >= 6000) {
        fluidSteps = 6;
    }

    auto *piston = dynamic_cast<PistonEngineSimulator *>(handle->simulator);
    if (piston != nullptr) {
        piston->setFluidSimulationSteps(fluidSteps);
    }
}

ESM_API void esm_set_fluid_steps(EsmHandle *handle, int steps) {
    if (handle == nullptr) {
        return;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    auto *piston = dynamic_cast<PistonEngineSimulator *>(handle->simulator);
    if (piston == nullptr) {
        return;
    }
    piston->setFluidSimulationSteps(std::clamp(steps, 2, 8));
}

ESM_API void esm_set_mix(
    EsmHandle *handle,
    float volume,
    float noise,
    float jitter,
    float hfGain,
    float convolution) {
    if (handle == nullptr) {
        return;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    Synthesizer::AudioParameters p = handle->simulator->synthesizer().getAudioParameters();
    p.volume = std::clamp(volume, 0.05f, 2.0f);
    p.airNoise = std::clamp(noise, 0.0f, 1.0f);
    p.inputSampleNoise = std::clamp(jitter, 0.0f, 1.0f);
    p.dF_F_mix = std::clamp(hfGain, 0.0f, 0.05f);
    p.convolution = std::clamp(convolution, 0.0f, 1.0f);
    p.levelerMaxGain = 1.05f;
    p.levelerMinGain = 0.85f;
    handle->simulator->synthesizer().setAudioParameters(p);
}

ESM_API void esm_set_highpass(EsmHandle *handle, float cutoffHz) {
    if (handle == nullptr) {
        return;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    handle->highpassHz = std::clamp(cutoffHz, 0.0f, 800.0f);
    if (handle->highpassHz <= 1.0f) {
        handle->hpLp = 0.0f;
    }
}

ESM_API const char *esm_last_error(void) {
    return g_lastError.c_str();
}

ESM_API int esm_queued_samples(EsmHandle *handle) {
    if (handle == nullptr) {
        return 0;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    return static_cast<int>(handle->simulator->synthesizer().m_audioBuffer.size());
}

ESM_API float esm_last_tick_ms(EsmHandle *handle) {
    if (handle == nullptr) {
        return 0.0f;
    }

    std::lock_guard<std::mutex> lock(handle->mutex);
    return handle->lastTickMs;
}

} // extern "C"
