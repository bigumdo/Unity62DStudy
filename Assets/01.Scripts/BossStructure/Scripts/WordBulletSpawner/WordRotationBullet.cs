using System.Collections;
using UnityEngine;
using YUI.Agents.players;

namespace YUI.Bullets.WordBullets
{
    public class WordRotationBullet : WordBullet
    {
        [SerializeField] private float minRotationStartTime;
        [SerializeField] private float maxRotationStartTime;
        [SerializeField] private float rotationTime;
        [SerializeField] private float startFireTime;
        [SerializeField] private float startFireSpeed;
        [SerializeField] private float changeFireSpeed;
        [SerializeField] private float changeFireSpeedTime;

        public override void ResetItem()
        {
            base.ResetItem();

            transform.eulerAngles = Vector3.zero;
        }

        protected override IEnumerator ShootRoutine()
        {
            SetRandomWord();
            SetRandomColor();
            SetRandomSize();

            float rotationStartTime = GetRandomRotationStartTime();
            float elapsedTime = 0;

            while (elapsedTime < rotationStartTime)
            {
                transform.Translate(Vector2.right * _speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0;
            Vector2 playerDirection = PlayerManager.Instance.Player.transform.position - transform.position;

            float startAngle = transform.eulerAngles.z;
            float targetAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;

            while (elapsedTime < rotationTime)
            {
                float t = elapsedTime / rotationTime;
                float eased = EaseInOutElastic(t);
                float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, eased);
                transform.eulerAngles = new Vector3(0, 0, currentAngle);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(startFireTime);

            elapsedTime = 0;
            Vector2 fireDir = Quaternion.Euler(0, 0, transform.eulerAngles.z) * Vector2.right;

            while (elapsedTime < changeFireSpeedTime)
            {
                float t = elapsedTime / changeFireSpeedTime;
                float eased = EaseOutCirc(t);
                float currentSpeed = Mathf.Lerp(startFireSpeed, changeFireSpeed, eased);
                transform.Translate(fireDir.normalized * currentSpeed * Time.deltaTime, Space.World);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            while (true)
            {
                transform.Translate(fireDir.normalized * changeFireSpeed * Time.deltaTime, Space.World);
                yield return null;
            }

        }

        private float GetRandomRotationStartTime()
        {
            return Random.Range(minRotationStartTime, maxRotationStartTime);
        }

        private  float EaseInOutElastic(float x)
        {
            float c5 = (2 * Mathf.PI) / 4.5f;

            if (x == 0f)
                return 0f;
            else if (x == 1f)
                return 1f;
            else if (x < 0.5f)
                return -Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * c5) / 2f;
            else
                return Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * c5) / 2f + 1f;
        }
        
        private float EaseOutCirc(float x)
        {
            return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
        }
    }
}
