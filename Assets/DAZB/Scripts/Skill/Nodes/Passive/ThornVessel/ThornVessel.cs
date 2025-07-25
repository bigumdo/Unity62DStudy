using UnityEngine;
using YUI.Agents.players;
using YUI.StatSystem;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "ThornVessel", menuName = "Skills/Passive/ThornVessel")]
    public class ThornVessel : PassiveSkill {
        public override void ApplyStatModifier(Player player) {
            base.ApplyStatModifier(player);

            AgentStat stat = player.GetCompo<AgentStat>();

            stat.AddModifier(stat.HpStat, "ThornVesselBuff", stat.HpStat.BaseValue * 0.2f);
        }

        public override void RemoveStatModifier(Player player) {
            base.RemoveStatModifier(player);

            AgentStat stat = player.GetCompo<AgentStat>();

            stat.RemoveModifier(stat.HpStat, "ThornVesselBuff");
        }

        public override void ExecuteSkill(Player player)
        {
            base.ExecuteSkill(player);

            player.GetCompo<PlayerHealth>().ApplyDamage(1);
        }
    }
}
