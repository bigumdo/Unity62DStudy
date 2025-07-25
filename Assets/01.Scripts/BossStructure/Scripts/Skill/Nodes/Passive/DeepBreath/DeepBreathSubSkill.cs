using UnityEngine;
using YUI.Agents.players;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "DeepBreathSubSkill", menuName = "Skills/Passive/Sub/DeepBreath")]
    public class DeepBreathSubSkill : PassiveSkill {
        public override void ExecuteSkill(Player player)
        {
            base.ExecuteSkill(player);

            player.GetCompo<PlayerSkill>().GetMainSkillSO().RestoreCooldownByTime(1f);
        }
    }
}
