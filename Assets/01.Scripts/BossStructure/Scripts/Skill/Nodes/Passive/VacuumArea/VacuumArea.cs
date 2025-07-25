using UnityEngine;
using YUI.Agents.players;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "VacuumArea", menuName = "Skills/Passive/VacuumArea")]
    public class VacuumArea : PassiveSkill {
        public override bool CanExecuteSkill(Player player)
        {
            return base.CanExecuteSkill(player) && player.GetCompo<PlayerSkill>().GetVariable("VacuumArea") == null;
        }

        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);

            player.GetCompo<PlayerSkill>().AddVariable("VacuumArea", true);
        }
    }
}
