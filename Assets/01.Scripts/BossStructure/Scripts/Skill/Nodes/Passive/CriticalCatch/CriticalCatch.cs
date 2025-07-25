using UnityEngine;
using YUI.Agents.players;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "CriticalCatch", menuName = "Skills/Passive/CriticalCatch")]
    public class CriticalCatch : PassiveSkill {
        public override bool CanExecuteSkill(Player player) {
            PlayerSkill playerSkill = player.GetCompo<PlayerSkill>();

            bool isCriticalCatch;

            if (playerSkill.GetVariable("CriticalCatch") != null) {
                isCriticalCatch = (bool)playerSkill.GetVariable("CriticalCatch");
            } 
            else {
                isCriticalCatch = false;
            }

            return base.CanExecuteSkill(player) && isCriticalCatch;
        }

        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);
            PlayerSkill playerSkill = player.GetCompo<PlayerSkill>();
            playerSkill.AddVariable("CriticalCatch", false);

            Debug.Log("Critical Catch Activated!");
        }
    }
}
