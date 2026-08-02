using System;
using System.Collections;
using UnityEngine;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Runs engine mix after stock SoundController updates
    /// </summary>
    public sealed class EngineAudioDriver : MonoBehaviour
    {
        private Action _onUpdate;
        private Action _onLateUpdate;
        private Action _onEndOfFrame;

        /// <summary>
        /// Creates a persistent driver host in the scene
        /// </summary>
        /// <param name="onUpdate">Per-frame gameplay callback</param>
        /// <param name="onLateUpdate">Callback after Update</param>
        /// <param name="onEndOfFrame">Optional callback after all scripts this frame</param>
        /// <returns>Created driver</returns>
        public static EngineAudioDriver Create(Action onUpdate, Action onLateUpdate, Action onEndOfFrame)
        {
            GameObject go = new GameObject("RMS_EngineAudioDriver");
            DontDestroyOnLoad(go);
            EngineAudioDriver driver = go.AddComponent<EngineAudioDriver>();
            driver._onUpdate = onUpdate;
            driver._onLateUpdate = onLateUpdate;
            driver._onEndOfFrame = onEndOfFrame;
            if (onEndOfFrame != null)
                driver.StartCoroutine(driver.EndOfFrameLoop());
            return driver;
        }

        private void Update()
        {
            if (_onUpdate != null)
                _onUpdate();
        }

        private void LateUpdate()
        {
            if (_onLateUpdate != null)
                _onLateUpdate();
        }

        private IEnumerator EndOfFrameLoop()
        {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            while (true)
            {
                yield return wait;
                if (_onEndOfFrame != null)
                    _onEndOfFrame();
            }
        }
    }
}
