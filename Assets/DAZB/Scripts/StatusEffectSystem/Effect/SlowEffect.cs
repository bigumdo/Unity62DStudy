using UnityEngine;
using YUI.Agents.players;
using YUI.StatSystem;

namespace YUI.StatusEffects {
    public class SlowEffect : StatusEffectBase {
        private AgentStat playerStat;

        public SlowEffect(float duration, Player player, StatusEffectType type) : base(duration, player, type)
        {
            playerStat = player.GetCompo<AgentStat>(true);
        }

        public override void Start() {
            float slowAmount = playerStat.MoveSpeedStat.BaseValue * value;

            playerStat.AddModifier(playerStat.MoveSpeedStat, "SlowPenalty", -slowAmount);
        }

        public override void End() {
            playerStat.RemoveModifier(playerStat.MoveSpeedStat, "SlowPenalty");
        }
    }
}
