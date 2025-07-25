using UnityEngine;
using YUI.Agents.players;
using YUI.Bullets;
using YUI.ObjPooling;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "BloomingPain", menuName = "Skills/Passive/BloomingPain")]
    public class BloomingPain : PassiveSkill {
        public override void ExecuteSkill(Player player) {
            base.ExecuteSkill(player);

            PlayerSkill skill = player.GetCompo<PlayerSkill>();

            int ateCount = 0;

            if (skill.GetVariable("AteCount") != null) {
                ateCount = (int)skill.GetVariable("AteCount");
            }

            if (ateCount > 4) {
                ateCount = 4;
            }

            for (int i = 0; i < ateCount; i++) {
                PlayerNemesisFangBullet bullet = PoolingManager.Instance.Pop("PlayerNemesisFangBullet") as PlayerNemesisFangBullet;
                bullet.transform.position = player.transform.position;
                bullet.StartShootRoutine(i + 1, 0.1f * i);
            }

            skill.AddVariable("AteCount", 0);
        }
    }
}
