using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;

namespace YUI.Apples
{
    public class AppleObject : MonoBehaviour
    {
        [SerializeField] private LayerMask whatIsPlayer;
        private AppleSpawner appleSpawner;

        public void Init(AppleSpawner spawner)
        {
            appleSpawner = spawner;
        }

        void Update()
        {
            if (GameManager.Instance.IsBattle == false)
            {
                appleSpawner.spawnedApples.Remove(this);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((whatIsPlayer & (1 << other.gameObject.layer)) != 0)
            {
                appleSpawner.spawnedApples.Remove(this);
                PlayerManager.Instance.Player.AppleEatEvent();
                PlayerManager.Instance.Player.GetCompo<PlayerHealth>().PlayerHeal(5);
            }
        }
    }
}
