using UnityEngine;
using YUI.Agents.players;
using YUI.StatSystem;

namespace YUI.StatusEffects {
    public class DamageIncreaseEffect : StatusEffectBase {
        private AgentStat playerStat;

        public DamageIncreaseEffect(float duration, Player player, StatusEffectType type) : base(duration, player, type)
        {
            playerStat = player.GetCompo<AgentStat>(true);
        }

        public override void Start()
        {
            playerStat.AddModifier(player.AttackStat, "DamageIncreaseEffect", value);
        }

        public override void End()
        {
            playerStat.RemoveModifier(player.AttackStat, "DamageIncreaseEffect");
        }
    }
}
