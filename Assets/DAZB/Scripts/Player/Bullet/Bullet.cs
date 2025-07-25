using UnityEngine;
using YUI.Agents.players;
using YUI.ObjPooling;

namespace YUI.Bullets {
    public class Bullet : PoolableMono {
        [SerializeField] protected float _speed;
        [SerializeField] protected float _damage;
        private Player player;

        private void Awake()
        {
            player = PlayerManager.Instance.Player;
        }

        private void Update()
        {
            
            if (player != null && player.IsDead)
            {
                PushBullet();
            }
        }

        public void AddDamage(float damage)
        {
            _damage += damage;
        }

        public void SetDamage(float damage) {
            _damage = damage;
        }

        public override void ResetItem() {
            
        }

        public void PushBullet() {
            PoolingManager.Instance.Push(this);
        }
    }
}
