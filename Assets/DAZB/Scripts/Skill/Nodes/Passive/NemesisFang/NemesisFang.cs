using UnityEngine;
using YUI.Agents.players;
using YUI.Bullets;
using YUI.ObjPooling;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "NemesisFang", menuName = "Skills/Passive/NemesisFang")]
    public class NemesisFang : PassiveSkill {
        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);

            for (int i = 0; i < 4; i++) {
                PlayerNemesisFangBullet bullet = PoolingManager.Instance.Pop("PlayerNemesisFangBullet") as PlayerNemesisFangBullet;
                bullet.transform.position = player.transform.position;
                bullet.StartShootRoutine(i + 1, 0.1f * i);
            }
        }
    }
}
