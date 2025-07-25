using UnityEngine;
using YUI.Bullets;
using YUI.ObjPooling;

namespace YUI.Agents.players {
    public class PlayerShield : MonoBehaviour {
        [SerializeField] private LayerMask projectileLayer;

        private Player player; 
        private PlayerSkill playerSkill1;

        public void SetPlayer(Player player) {
            this.player = player;

            playerSkill1 = player.GetCompo<PlayerSkill>(true);
        }

        private void Update() {
            if (playerSkill1.GetVariable("VacuumArea") == null) {
                transform.position = player.transform.position;
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!IsInLayerMask(other.gameObject.layer)) return;

            PlayerSkill playerSkill = player.GetCompo<PlayerSkill>(true);

            if (playerSkill.GetVariable("DeletedBulletCount") == null) {
                playerSkill.AddVariable("DeletedBulletCount", 1);
            }
            else {
                playerSkill.AddVariable("DeletedBulletCount", 1 + (int)playerSkill.GetVariable("DeletedBulletCount"));
            }

            other.gameObject.GetComponent<Bullet>().PushBullet();
        }

        private bool IsInLayerMask(int layer) {
            return (projectileLayer.value & (1 << layer)) != 0;
        }
    }
}
