using UnityEngine;
using YUI.Agents.players;
using YUI.Bullets;
using YUI.ObjPooling;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "SublimationOfThorns", menuName = "Skills/Passive/SublimationOfThorns")]
    public class SublimationOfThorns : PassiveSkill {
        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);

            PlayerSublimationOfThornsBullet bullet =  PoolingManager.Instance.Pop("PlayerSublimationOfThornsBullet") as PlayerSublimationOfThornsBullet;
            bullet.transform.position = player.transform.position;
            bullet.StartShootRoutine();
        }
    }
}
