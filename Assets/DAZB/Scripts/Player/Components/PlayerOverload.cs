using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using YUI.Cores;
using YUI.StatSystem;

namespace YUI.Agents.players {
    [Serializable]
    public class OverloadFeedbackData
    {
        public PlayerMode playerMode;
        public Color color;
        public List<OverloadIntensityData> overloadIntensities;
    }

    [Serializable]
    public class OverloadIntensityData
    {
        public float intensity;
        public float time;
        public string soundName;
    }

    public class PlayerOverload : MonoBehaviour, IAgentComponent, IAfterInit
    {
        [SerializeField] private GameObject OverloadEffect;

        [Space]

        [SerializeField] private StatSO maxOverloadStat;

        [Space]

        [SerializeField] private Image overloadGaugeImage;
        [SerializeField] private TextMeshProUGUI overloadText;
        private float currentFillAmount;

        [Space]

        public List<OverloadFeedbackData> overloadFeedbackData;

        private Player player;
        private AgentStat agentStat;

        public float Overload => currentOverload;
        public float MaxHealth => maxOverloadStat.Value;

        private float currentOverload;

        public event Action OverloadEvent;
        public event Action ReleaseEvent;
        public event Action ReleaseEndEvent;
        public event Action OverloadEndEvent;

        public bool isFirstVer = true;

        public event Action<float> OverloadChanged;

        public void Initialize(Agent agent)
        {
            player = agent as Player;
            agentStat = agent.GetCompo<AgentStat>();
        }

        public void AfterInit()
        {
            maxOverloadStat = agentStat.GetStat(maxOverloadStat);

            currentOverload = 0;
        }

        private void Start()
        {
            StartCoroutine(OverloadFeedbackRoutine());
        }

        private void Update()
        {
            if (isFirstVer && overloadGaugeImage != null)
            {
                overloadText.enabled = false;
                overloadGaugeImage.enabled = true;

                float targetFill = currentOverload / maxOverloadStat.Value;
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFill, Time.deltaTime * 10f);

                overloadGaugeImage.fillAmount = currentFillAmount;

                overloadGaugeImage.transform.position = player.transform.position;
            }
            else if (overloadText != null)
            {
                overloadText.enabled = true;
                overloadGaugeImage.enabled = false;

                int overloadCount = (int)(currentOverload / 10);


                if (player.PlayerMode == PlayerMode.OVERLOAD)
                {
                    overloadText.color = Color.red;
                }
                else if (player.PlayerMode == PlayerMode.NORMAL)
                {
                    overloadText.color = Color.black;
                }
                else if (player.PlayerMode == PlayerMode.RELEASE)
                {
                    overloadText.color = Color.yellow;
                }

                overloadText.text = $"{overloadCount}";
            }


            if (player.PlayerMode == PlayerMode.OVERLOAD)
            {
                OverloadEffect.SetActive(true);
                overloadText.color = Color.red;
            }
            else if (player.PlayerMode == PlayerMode.NORMAL)
            {
                OverloadEffect.SetActive(false);
                overloadText.color = Color.black;
            }
            else if (player.PlayerMode == PlayerMode.RELEASE)
            {
                OverloadEffect.SetActive(false);
                overloadText.color = Color.yellow;
            }
        }

        public void SetOverload(float value)
        {
            currentOverload = value;
            OverloadChanged?.Invoke(currentOverload);
        }

        public void AddOverload(float value, bool isOverload = false)
        {
            currentOverload += value;

            if (isOverload)
            {
                if (currentOverload <= 0)
                {
                    ResetOverload();

                    OverloadEndEvent?.Invoke();
                }
            }
            else
            {
                if (player.PlayerMode != PlayerMode.RELEASE && currentOverload >= maxOverloadStat.Value / 2)
                {
                    ReleaseEvent?.Invoke();
                }
                else if (currentOverload >= maxOverloadStat.Value)
                {
                    currentOverload = maxOverloadStat.Value;
                    ReleaseEndEvent?.Invoke();
                    OverloadEvent?.Invoke();
                }
                else if (player.PlayerMode == PlayerMode.RELEASE && currentOverload < maxOverloadStat.Value / 2)
                {
                    ReleaseEndEvent?.Invoke();
                }
            }

            if (currentOverload <= 0)
            {
                currentOverload = 0;
            }
            OverloadChanged?.Invoke(currentOverload);

        }

        public void ResetOverload()
        {
            if (player.PlayerMode == PlayerMode.RELEASE)
            {
                ReleaseEndEvent?.Invoke();
            }
            else if (player.PlayerMode == PlayerMode.OVERLOAD)
            {
                OverloadEndEvent?.Invoke();
            }

            currentOverload = 0;
            OverloadChanged?.Invoke(currentOverload);
        }

        private IEnumerator OverloadFeedbackRoutine()
        {
            Vignette vignette = CameraManager.Instance.Vignette;

            while (true)
            {
                if (player.GetCompo<PlayerMover>(true).moveType == PlayerMoveType.NORMAL) 
                {
                    vignette.intensity.value = 0f;
                    yield return null;
                    continue;
                }

                OverloadFeedbackData feedbackData = overloadFeedbackData.Find(x => x.playerMode == player.PlayerMode);
                vignette.color.value = feedbackData.color;
                if (feedbackData != null && feedbackData.overloadIntensities != null && feedbackData.overloadIntensities.Count > 0)
                {
                    for (int i = 0; i < feedbackData.overloadIntensities.Count; i++)
                    {
                        float startIntensity = vignette.intensity.value;
                        float targetIntensity = feedbackData.overloadIntensities[i].intensity;
                        float duration = feedbackData.overloadIntensities[i].time;
                        float elapsed = 0f;

                        if (feedbackData.overloadIntensities[i].soundName != "") {
                            SoundManager.Instance.PlaySound(feedbackData.overloadIntensities[i].soundName);
                        }

                        while (elapsed < duration)
                        {
                            elapsed += Time.deltaTime;
                            float t = Mathf.Clamp01(elapsed / duration);
                            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
                            yield return null;
                        }
                        vignette.intensity.value = targetIntensity;
                    }
                }
                else
                {
                    vignette.intensity.value = 0f;
                    yield return null;
                }
            }
        }
    }
}
