using UnityEngine;
using YUI.Agents.players;
using YUI.StatusEffects;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "DanceOnTacks", menuName = "Skills/Passive/DanceOnTacks")]
    public class DanceOnTacks : PassiveSkill {
        public override bool CanExecuteSkill(Player player) {
            PlayerHealth health = PlayerManager.Instance.Player.GetCompo<PlayerHealth>();

            return base.CanExecuteSkill(player) && health.GetCurrentHp() <= health.GetMaxHp() / 2;
        }

        public override void ExecuteSkill(Player player)
        {
            base.ExecuteSkill(player);

            StatusEffectManager.Instance.AddStatusEffect(StatusEffectType.DashCooldown, 0.1f, -1.5f);
        }
    }
}
