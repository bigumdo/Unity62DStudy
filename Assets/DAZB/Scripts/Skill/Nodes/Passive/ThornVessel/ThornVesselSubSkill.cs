using UnityEngine;
using YUI.Agents.players;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "ThornVesselSubSkill", menuName = "Skills/Passive/Sub/ThornVesselSubSkill")]
    public class ThornVesselSubSkill : PassiveSkill {
        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);

            PlayerHealth hp = player.GetCompo<PlayerHealth>();

            hp.PlayerHeal(hp.GetMaxHp() * 0.1f);
        }
    }
}
