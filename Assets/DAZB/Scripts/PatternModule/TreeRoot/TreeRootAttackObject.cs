using UnityEngine;
using YUI.Agents.players;

namespace YUI.PatternModules
{
    public class TreeRootAttackObject : MonoBehaviour
    {
        [SerializeField] private LayerMask whatIsPlayer;
        private float damage = 999999999f;
        private bool canAttack = false;

        public void SetCanAttack(bool value)
        {
            canAttack = value;
        } 

        private void OnTriggerStay2D(Collider2D other)
        {
            if (canAttack == false) return;

            if ((whatIsPlayer & (1 << other.gameObject.layer)) != 0)
            {
                if (other.TryGetComponent(out Player player))
                {
                    player.GetCompo<PlayerHealth>().ApplyDamage(damage);
                }
            }
        }
    }
}
