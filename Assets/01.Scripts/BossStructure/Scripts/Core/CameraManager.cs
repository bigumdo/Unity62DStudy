using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace YUI.Cores {
    public class CameraManager : MonoSingleton<CameraManager> {
        [SerializeField] private CinemachineBasicMultiChannelPerlin perlin;
        [SerializeField] private CinemachineConfiner2D confiner;
        [SerializeField] private CinemachineCamera defaultCam;
        [SerializeField] private CinemachineCamera battleCam;
        [SerializeField] private Material panicMat;
        public Volume globalVolume;

        [Space, Space]

        [SerializeField] private Image fadePanel;

        private Coroutine resetCoroutine;

        private Vignette vg;
        private DepthOfField depth;
        private ChromaticAberration chromaticAberration;

        public ChromaticAberration ChromaticAberration => chromaticAberration;
        public Vignette Vignette => vg;
        
        private void Awake()
        {
            if (globalVolume == null)
            {
                Debug.LogError("globalVolume is not assigned in the Inspector.");
                return;
            }

            if (globalVolume.profile == null)
            {
                Debug.LogError("globalVolume.profile is missing. Please assign a Volume Profile.");
                return;
            }

            if (!globalVolume.profile.TryGet(out vg))
            {
                Debug.LogError("Vignette not found in the Volume Profile.");
            }
            if (!globalVolume.profile.TryGet(out depth))
            {
                Debug.LogError("DepthOfField not found in the Volume Profile.");
            }
            if (!globalVolume.profile.TryGet(out chromaticAberration))
            {
                Debug.LogError("ChromaticAberration not found in the Volume Profile.");
            }
        }

        private void OnDestroy()
        {
            SetPlayerPanicScreen(false);
        }

        public void ShakeCamera(float amplitude, float frequency, float time)
        {
            if (perlin == null) return;

            perlin.AmplitudeGain = amplitude;
            perlin.FrequencyGain = frequency;

            if (resetCoroutine != null)
                StopCoroutine(resetCoroutine);

            resetCoroutine = StartCoroutine(ResetCamera(time));
        }

        public void SetDefaultCam()
        {
            defaultCam.gameObject.SetActive(true);
            battleCam.gameObject.SetActive(false);
        }

        public void SetBattleCam()
        {
            defaultCam.gameObject.SetActive(false);
            battleCam.gameObject.SetActive(true);
        }

        private IEnumerator ResetCamera(float time)
        {
            yield return new WaitForSeconds(time);
            perlin.AmplitudeGain = 0f;
            perlin.FrequencyGain = 0f;
        }
        
        public void FadeIn(float time = 0.5f, Action Callback = null)
        {
            StartCoroutine(FadeRoutine(1f, 0f, time, Callback));
        }

        public void FadeOut(float time = 0.5f, Action Callback = null)
        {
            StartCoroutine(FadeRoutine(0f, 1f, time, Callback));
        }

        private IEnumerator FadeRoutine(float from, float to, float time, Action Callback)
        {
            float elapsed = 0f;
            Color color = fadePanel.color;
            color.a = from;
            fadePanel.color = color;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / time);
                color.a = Mathf.Lerp(from, to, t);
                fadePanel.color = color;
                yield return null;
            }
            color.a = to;
            fadePanel.color = color;
            Callback?.Invoke();
        }

        public void SetFadeColor(Color ChangeColor)
        {
            ChangeColor.a = 0;
            fadePanel.color = ChangeColor;
        }

        public void SetConfiner(Collider2D collider)
        {
            if (confiner != null)
            {
                confiner.BoundingShape2D = collider;
            }
            else
            {
                Debug.LogWarning("Confiner is null.");
            }
        }

        public void SetPlayerPanicScreen(bool set)
        {
            panicMat.SetFloat("_IsPanicState", set ? 1: 0);
        }
    }
}
