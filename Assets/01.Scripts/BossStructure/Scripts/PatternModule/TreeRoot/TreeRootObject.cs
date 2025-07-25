using UnityEngine;
using YUI.Agents.players;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    public class TreeRootObject : PoolableMono
    {
        public TreeRootAttackObject attackObject;
        [SerializeField] private LayerMask whatIsPlayer;
        private float damage = 0f;
        private bool damaged = false;

        public override void ResetItem()
        {
            damage = 0f;
            damaged = false;
            attackObject.SetCanAttack(false);
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }

        public void SetDamaged(bool value)
        {
            damaged = value;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (damaged) return;

            if ((whatIsPlayer & (1 << other.gameObject.layer)) != 0)
            {
                if (other.TryGetComponent(out Player player))
                {
                    player.GetCompo<PlayerHealth>().ApplyDamage(damage);
                    damaged = true;
                }
            }
        }
    }
}
