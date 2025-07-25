using UnityEngine;
using YUI.Agents.players;
using YUI.StatusEffects;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "ThornCatalyst", menuName = "Skills/Passive/ThornCatalyst")]
    public class ThornCatalyst : PassiveSkill {
        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);

            PlayerOverload playerOverload = player.GetCompo<PlayerOverload>();

            int deletedBulletCount = 0;

            if (player.GetCompo<PlayerSkill>().GetVariable("DeletedBulletCount") != null) {
                deletedBulletCount = (int)player.GetCompo<PlayerSkill>().GetVariable("DeletedBulletCount");
            }

            bool isDeletedBulletCountSevenOver = deletedBulletCount >= 7;

            if (player.PlayerMode == PlayerMode.NORMAL) {
                if (isDeletedBulletCountSevenOver) {
                    StatusEffectManager.Instance.AddStatusEffect(StatusEffectType.DamageIncrease, 5f, player.AttackStat.Value * 0.3f);
                }
                else if (deletedBulletCount != 0) {
                    playerOverload.AddOverload(5 * deletedBulletCount);
                }
                else {
                    playerOverload.AddOverload(-20);
                }
            }
            else if (player.PlayerMode == PlayerMode.RELEASE) {
                if (isDeletedBulletCountSevenOver) {
                    player.GetCompo<PlayerSkill>().GetMainSkillSO().RestoreCooldownByPercent(0.3f);
                }
                else if (deletedBulletCount != 0) {
                    playerOverload.AddOverload(-5 * deletedBulletCount);
                }
                else {
                    playerOverload.AddOverload(20);
                }
            }

            player.GetCompo<PlayerSkill>().AddVariable("DeletedBulletCount", 0);
        }
    }
}
