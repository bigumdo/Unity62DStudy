using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YUI.Agents;

namespace YUI.Bullets.WordBullets
{
    public class WordBullet : Bullet
    {
        [SerializeField] protected float destroyTime = 7;

        [SerializeField] private List<string> BulletWordList;
        [SerializeField] private Color BulletTextColor1;
        [SerializeField] private Color BulletTextColor2;
        [SerializeField] private int minLinerBulletSize;
        [SerializeField] private int maxLinerBulletSize;
        [SerializeField] private LayerMask whatIsPlayer;
        private TextMeshProUGUI text;

        private Coroutine coroutine;
        private Coroutine destroyCoroutine;

        private void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public override void ResetItem()
        {
            base.ResetItem();

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
            }
        }

        public void StartShoot()
        {
            coroutine = StartCoroutine(ShootRoutine());
            destroyCoroutine = StartCoroutine(DestroyRoutine());
        }

        protected IEnumerator DestroyRoutine()
        {
            yield return new WaitForSeconds(destroyTime);
            PushBullet();
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        protected virtual IEnumerator ShootRoutine()
        {
            yield return null;
        }

        protected void SetRandomWord()
        {
            if (BulletWordList.Count > 0)
            {
                int randomIndex = Random.Range(0, BulletWordList.Count);
                text.text = BulletWordList[randomIndex];
            }
        }

        protected void SetRandomColor()
        {
            if (text != null)
            {
                text.color = Color.Lerp(BulletTextColor1, BulletTextColor2, Random.Range(0f, 1f));
            }
        }

        protected void SetRandomSize()
        {
            if (text != null)
            {
                int randomSize = Random.Range(minLinerBulletSize, maxLinerBulletSize);
                transform.localScale = new Vector3(randomSize, randomSize, 1f);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if ((whatIsPlayer & (1 << other.gameObject.layer)) != 0)
            {
                if (other.gameObject.TryGetComponent(out Agent agent))
                {
                    agent.GetCompo<AgentHealth>(true).ApplyDamage(_damage);
                }
                StopCoroutine(coroutine);
                StopCoroutine(destroyCoroutine);
                PushBullet();
            }
        }
    }
}
