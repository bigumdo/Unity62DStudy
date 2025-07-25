using UnityEngine;
using YUI.Agents.players;
using YUI.StatSystem;

namespace YUI.StatusEffects {
    public class CounterEffect : StatusEffectBase {
        private AgentStat stats;

        public CounterEffect(float duration, Player player, StatusEffectType type) : base(duration, player, type)
        {
            stats = player.GetCompo<AgentStat>(true);
        }

        public override void Start()
        {
            stats.AddModifier(player.AttackStat, "CounterBonusAttackState", 5);
            stats.AddModifier(player.ChargingCountStat, "CounterBonusChargingCountStat", 5);
            stats.AddModifier(player.TotalChargingCompleteTimeStat, "CounterBonusTotalChargingCompleteTimeStat", -player.TotalChargingCompleteTimeStat.BaseValue);
            stats.AddModifier(player.OverloadIncreaseStat, "CounterBonusOverloadIncreaseStat", -20);
            stats.AddModifier(player.chargingAttackDelayStat, "CounterBonusChargingAttackDelayStat", -player.chargingAttackDelayStat.BaseValue);
            stats.AddModifier(player.IndividualChargingCompleteTimeStat, "CounterBonusIndividualChargingCompleteTimeStat", -player.IndividualChargingCompleteTimeStat.BaseValue);
        }

        public override void End() {
            stats.RemoveModifier(player.AttackStat, "CounterBonusAttackState");
            stats.RemoveModifier(player.ChargingCountStat, "CounterBonusChargingCountStat");
            stats.RemoveModifier(player.TotalChargingCompleteTimeStat, "CounterBonusTotalChargingCompleteTimeStat");
            stats.RemoveModifier(player.OverloadIncreaseStat, "CounterBonusOverloadIncreaseStat");
            stats.RemoveModifier(player.chargingAttackDelayStat, "CounterBonusChargingAttackDelayStat");
            stats.RemoveModifier(player.IndividualChargingCompleteTimeStat, "CounterBonusIndividualChargingCompleteTimeStat");
        }
    }
}
