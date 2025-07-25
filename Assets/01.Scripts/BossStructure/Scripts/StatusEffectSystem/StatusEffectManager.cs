using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;

namespace YUI.StatusEffects {
    public enum StatusEffectType {
        Slow,
        Reversal,
        Counter,
        ForceSlowMode,
        DashCooldown,
        DamageIncrease,
    }

    public class StatusEffectManager : MonoSingleton<StatusEffectManager> {
        private List<StatusEffectBase> statusEffectList = new List<StatusEffectBase>();
        private Player player;

        private void Start() {
            player = PlayerManager.Instance.Player;
        }

        public void AddStatusEffect(StatusEffectType type, float duration, float value = 0) {
            StatusEffectBase statusEffect = statusEffectList.FirstOrDefault(p => p.type == type);

            if (statusEffect != null) {
                statusEffect.ResetTime();
                return;
            }

            switch (type) {
                case StatusEffectType.Slow: {
                    statusEffect = new SlowEffect(duration, player, type);
                    break;
                }
                case StatusEffectType.Reversal: {
                    statusEffect = new ReversalEffect(duration, player, type);
                    break;
                }
                case StatusEffectType.Counter: {
                    statusEffect = new CounterEffect(duration, player, type);
                    break;
                }
                case StatusEffectType.ForceSlowMode: {
                    statusEffect = new ForceSlowModeEffect(duration, player, type);
                    break;
                }
                case StatusEffectType.DashCooldown: {
                    statusEffect = new DashCooldownEffect(duration, player, type);
                    break;
                }
                case StatusEffectType.DamageIncrease: {
                    statusEffect = new DamageIncreaseEffect(duration, player, type);
                    break;
                }
            }

            statusEffect.SetValue(value);

            statusEffectList.Add(statusEffect);
            statusEffect.ResetTime();
            statusEffect.Start();
        }

        private void Update() {
            for (int i = 0; i < statusEffectList.Count; i++) {
                if (statusEffectList[i].IsEnd()) {
                    statusEffectList[i].End();
                    statusEffectList.RemoveAt(i);
                    i--;
                }
            }
        }

        public bool CheckStatusEffect(StatusEffectType forceSlowMode)
        {
            return statusEffectList.Any(p => p.type == forceSlowMode);
        }
    }
}
