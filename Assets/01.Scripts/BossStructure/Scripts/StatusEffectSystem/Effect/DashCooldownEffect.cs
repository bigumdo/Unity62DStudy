using UnityEngine;
using YUI.Agents.players;
using YUI.StatSystem;

namespace YUI.StatusEffects {
    public class DashCooldownEffect : StatusEffectBase {

        public DashCooldownEffect(float duration, Player player, StatusEffectType type) : base(duration, player, type)
        {
        }

        public override void Start() {
            AgentStat agentStat = player.GetCompo<AgentStat>();

            agentStat.AddModifier(player.DashCooldownStat, "DashCooldownEffect", value);
        }

        public override void End() {
            AgentStat agentStat = player.GetCompo<AgentStat>();

            agentStat.RemoveModifier(player.DashCooldownStat, "DashCooldownEffect");
        }
    }
}
