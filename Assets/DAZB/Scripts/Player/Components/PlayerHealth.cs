using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using YUI.Cores;
using YUI.ObjPooling;
using YUI.Skills;

namespace YUI.Agents.players {
    public class PlayerHealth : AgentHealth {
        [SerializeField] private Color damageTextColor;
        [SerializeField] private float feedbackStartHpPer;
        [SerializeField] private float feedbackEndHpPer;

        private bool isInvincible = false;
        public event Action PlayerHealthChanged;

        public override void ApplyDamage(float damage)
        {
            if (isInvincible) return;

            Player player = _agent as Player;

            CameraManager.Instance.ShakeCamera(6f, 8f, 0.1f);

            DamageIndicator damageIndicator = PoolingManager.Instance.Pop("DamageIndicator") as DamageIndicator;
            damageIndicator.SetTextColor(damageTextColor);
            damageIndicator.SetText(damage.ToString());
            damageIndicator.SetPosition(player.transform.position);
            damageIndicator.StartRoutine();

            SoundManager.Instance.PlaySound("SFX_Player_Hitted");

            player.GetCompo<PlayerSkill>().PassiveSkillExecution(SkillExecutionType.Hurt);

            if (player.IsProtected)
            {
                _agent.OnHitEvent?.Invoke();
                return;
            }

            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            if (_currentHealth <= 0)
                _agent.OnDeadEvent?.Invoke();
            else
            {
                //StartInvincible(1.5f);
                _agent.OnHitEvent?.Invoke();
                PlayerHealthChanged?.Invoke();
            }

            StartCoroutine(FeedbackRoutine(1f));
        }

        public void PlayerHeal(float heal)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + heal, 0, _maxHealth);
            PlayerHealthChanged?.Invoke();
        }

        public void StartInvincible(float time) {
            StartCoroutine(InvincibleRoutine(time));
        }
        
        public float GetCurrentHealthPercentage()
        {
            return _currentHealth / _maxHealth;
        }

        private IEnumerator FeedbackRoutine(float time)
        {
            float hpPer = GetCurrentHealthPercentage();

            float min = Mathf.Max(feedbackStartHpPer, feedbackEndHpPer);
            float max = Mathf.Min(feedbackStartHpPer, feedbackEndHpPer);

            if (hpPer > min || hpPer < max)
                yield break;

            float elapsedTime = 0f;
            ChromaticAberration chromaticAberration = CameraManager.Instance.ChromaticAberration;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                hpPer = GetCurrentHealthPercentage();
                float t = Mathf.InverseLerp(min, max, hpPer);
                chromaticAberration.intensity.value = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
        }

        private IEnumerator InvincibleRoutine(float time)
        {
            isInvincible = true;
            yield return new WaitForSeconds(time);
            isInvincible = false;
        }

        public void SetInvincible(bool v)
        {
            isInvincible = v;
        }
    }
}
