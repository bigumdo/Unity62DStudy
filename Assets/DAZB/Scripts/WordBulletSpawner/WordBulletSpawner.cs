using System.Collections;
using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Bullets.WordBullets
{
    public class WordBulletSpawner : MonoBehaviour
    {
        [Header("Spawn Range")]
        public Vector2 rangeOffset;
        public float rangeY = 5f;

        [Header("Bullet Settings")]
        public float minSpeed = 3f;
        public float maxSpeed = 8f;
        public int damage = 1;

        [Header("Spawn Counts")]
        public int linerBulletCount = 3;
        public int rotationBulletCount = 2;

        [Header("Spawn Timing")]
        public float spawnDelay = 0.5f;

        private bool isShootable = false;

        private void Start()
        {
            StartCoroutine(Routine());
        }

        private IEnumerator Routine()
        {
            int currentIndex = 0;

            while (true)
            {
                if (!isShootable)
                {
                    yield return null;
                    continue;
                }

                if (currentIndex == 0)
                {
                    for (int i = 0; i < linerBulletCount; i++)
                    {
                        WordBullet wordBullet = PoolingManager.Instance.Pop("WordLinerBullet") as WordBullet;
                        wordBullet.SetPosition(GetRandomPosition());
                        wordBullet.SetSpeed(GetRandomSpeed());
                        wordBullet.SetDamage(damage);
                        wordBullet.StartShoot();
                        yield return new WaitForSeconds(spawnDelay);
                    }
                    currentIndex = 1;
                }
                else if (currentIndex == 1)
                {
                    for (int i = 0; i < rotationBulletCount; i++)
                    {
                        WordBullet wordBullet = PoolingManager.Instance.Pop("WordRotationBullet") as WordBullet;
                        wordBullet.SetPosition(GetRandomPosition());
                        wordBullet.SetSpeed(GetRandomSpeed());
                        wordBullet.SetDamage(damage);
                        wordBullet.StartShoot();
                        yield return new WaitForSeconds(spawnDelay);
                    }
                    currentIndex = 0;
                }

                yield return null;
            }
        }

        public void SetShootable(bool value)
        {
            isShootable = value;
        }

        private Vector2 GetRandomPosition()
        {
            float x = rangeOffset.x;
            float y = Random.Range(-rangeY / 2f, rangeY / 2f);
            return new Vector2(x, y) + (Vector2)transform.position;
        }

        private float GetRandomSpeed()
        {
            return Random.Range(minSpeed, maxSpeed);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gold;

            Gizmos.DrawWireCube((Vector2)transform.position + rangeOffset, new Vector3(0, rangeY, 0f));
        }
    }
}
