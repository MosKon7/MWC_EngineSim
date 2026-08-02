#ifndef ENGINE_SIM_NATIVE_H
#define ENGINE_SIM_NATIVE_H

#include <stdint.h>

#ifdef _WIN32
#  ifdef ESM_BUILDING_DLL
#    define ESM_API __declspec(dllexport)
#  else
#    define ESM_API __declspec(dllimport)
#  endif
#else
#  define ESM_API
#endif

#ifdef __cplusplus
extern "C" {
#endif

typedef struct EsmHandle EsmHandle;

/**
 * Loads an engine .mr script and prepares a headless simulator
 * @param scriptPath Absolute or relative path to .mr
 * @return Opaque handle or NULL on failure
 */
ESM_API EsmHandle *esm_create(const char *scriptPath);

/**
 * Releases simulator resources
 * @param handle Handle from esm_create
 */
ESM_API void esm_destroy(EsmHandle *handle);

/**
 * Sets live control targets for the next ticks
 * @param handle Simulator handle
 * @param rpm Target engine RPM (dyno hold)
 * @param throttle01 Throttle 0..1
 * @param ignitionOn Non-zero enables ignition
 */
ESM_API void esm_set_targets(EsmHandle *handle, float rpm, float throttle01, int ignitionOn);

/**
 * Advances physics and synthesizer by dtSeconds
 * @param handle Simulator handle
 * @param dtSeconds Simulation timestep (clamped)
 * @return 0 on success, non-zero on error
 */
ESM_API int esm_tick(EsmHandle *handle, float dtSeconds);

/**
 * Reads available PCM samples (mono int16, 44100 Hz)
 * @param handle Simulator handle
 * @param dst Destination buffer
 * @param maxSamples Capacity of dst
 * @return Number of samples written
 */
ESM_API int esm_read_pcm(EsmHandle *handle, int16_t *dst, int maxSamples);

/**
 * Sets physics simulation frequency in Hz
 * @param handle Simulator handle
 * @param simHz Frequency clamped to a safe range
 */
ESM_API void esm_set_quality(EsmHandle *handle, int simHz);

/**
 * Sets combustion fluid substeps per physics step
 * @param handle Simulator handle
 * @param steps Substeps clamped to 2..8
 */
ESM_API void esm_set_fluid_steps(EsmHandle *handle, int steps);

/**
 * Sets synthesizer mix parameters for live playback
 * @param handle Simulator handle
 * @param volume Master synth volume
 * @param noise Air noise amount
 * @param jitter Input sample jitter/noise
 * @param hfGain High-frequency mix (dF/F)
 * @param convolution Impulse response wet amount
 */
ESM_API void esm_set_mix(
    EsmHandle *handle,
    float volume,
    float noise,
    float jitter,
    float hfGain,
    float convolution);

/**
 * Sets high-pass cutoff for bass cut on PCM output
 * @param handle Simulator handle
 * @param cutoffHz Cutoff in Hz, 0 disables the filter
 */
ESM_API void esm_set_highpass(EsmHandle *handle, float cutoffHz);

/**
 * Returns last error message (static buffer, never NULL)
 */
ESM_API const char *esm_last_error(void);

/**
 * Returns queued PCM sample count
 * @param handle Simulator handle
 */
ESM_API int esm_queued_samples(EsmHandle *handle);

/**
 * Returns last tick cost in milliseconds
 * @param handle Simulator handle
 */
ESM_API float esm_last_tick_ms(EsmHandle *handle);

#ifdef __cplusplus
}
#endif

#endif /* ENGINE_SIM_NATIVE_H */
