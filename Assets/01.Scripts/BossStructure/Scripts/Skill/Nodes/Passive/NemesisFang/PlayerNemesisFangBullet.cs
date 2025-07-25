using System.Collections;
using UnityEngine;
using YUI.Agents;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.Bullets {
    public class PlayerNemesisFangBullet : BezierBullet {
        [SerializeField] private LayerMask whatIsBoss;

        private TrailRenderer trailRenderer;
        private float defaultTrailTime;
        private float defaultWidth;

        private SpriteRenderer spriteRenderer;

        private void Awake() {
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            trailRenderer.enabled = false;

            defaultTrailTime = trailRenderer.time;
            defaultWidth = trailRenderer.startWidth;
        }

        public void StartShootRoutine(int index, float delay) {
            StartCoroutine(ShootRoutine(index, delay));
        }

        private IEnumerator ShootRoutine(int index, float delay) {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(delay);
            spriteRenderer.enabled = true;

            trailRenderer.time = defaultTrailTime;
            trailRenderer.startWidth = defaultWidth;
            trailRenderer.enabled = true;

            SoundManager.Instance.PlaySound("SFX_Player_ShotSubBullet");

            Transform target = null;

            if (BossManager.Instance.Boss == null)
            {
                target = PlayerManager.Instance.Player.transform;
            }
            else
            {
                target = BossManager.Instance.Boss.transform;
            }

            Vector3 p0 = transform.position;
            Vector3 p3 = target.position;

            bool isRight = p0.x < p3.x;

            Vector3 mid = (p0 + p3) * 0.5f;
            Vector3 offset = Vector3.zero;

            if (index == 1) {
                offset = new Vector3(5f * (isRight ? -1 : 1), 0f, 5f);
            }
            else if (index == 2) {
                offset = new Vector3(5f * (isRight ? -1 : 1), 0f, -5f);
            }
            else if (index == 3) {
                offset = new Vector3(5f * (isRight ? -1 : 1), 0f, 2.5f);
            }
            else if (index == 4) {
                offset = new Vector3(5f * (isRight ? -1 : 1), 0f, -2.5f);
            }


            Vector3 p1 = Vector3.Lerp(p0, mid, 0.5f) + offset;
            Vector3 p2 = Vector3.Lerp(mid, p3, 0.5f) + offset;

            float totalLength = EstimateBezierLength(p0, p1, p2, p3);
            float duration = totalLength / _speed;

            float t = 0f;
            Vector3 previousPos = p0;
            float arriveDistance = 0.1f;

            while (true)
            {
                target = BossManager.Instance.Boss == null ? PlayerManager.Instance.Player.transform : BossManager.Instance.Boss.transform;
                p3 = target.position;

                mid = (p0 + p3) * 0.5f;
                p1 = Vector3.Lerp(p0, mid, 0.5f) + offset;
                p2 = Vector3.Lerp(mid, p3, 0.5f) + offset;


                Vector3 pos = FourPointBezier(p0, p1, p2, p3, t);
                transform.position = pos;

                Vector3 moveDir = (pos - previousPos).normalized;

                if (moveDir != Vector3.zero) {
                    float angle = Mathf.Atan2(moveDir.z, moveDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(-90, 0, -angle - 90);
                }

                if (Vector3.Distance(transform.position, p3) <= arriveDistance)
                    break;

                t += Time.deltaTime / duration;
                t = Mathf.Clamp01(t);

                previousPos = pos;
                yield return null;
            }

            transform.position = p3;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if ((whatIsBoss & (1 << other.gameObject.layer)) != 0) {
                float appliedDamage = PlayerManager.Instance.Player.AttackStat.Value * _damage;

                if (other.gameObject.TryGetComponent(out Agent agent))
                {
                    SoundManager.Instance.PlaySound("SFX_Boss_Hitted");
                    agent.GetCompo<AgentHealth>(true).ApplyDamage(appliedDamage);
                }

                CameraManager.Instance.ShakeCamera(3f, 4f, 0.1f);

                GameObject go = PoolingManager.Instance.Pop("HitEffect").gameObject;

                Vector3 direction = other.transform.position - transform.transform.position;
                direction.Normalize();

                go.transform.position = BossManager.Instance.Boss.transform.position;

                float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                go.transform.eulerAngles = new Vector3(0, 0, -angle);

                SlashEffect slashEffect = PoolingManager.Instance.Pop("SlashEffect") as SlashEffect;
                slashEffect.transform.SetPositionAndRotation(BossManager.Instance.Boss.transform.position, Quaternion.Euler(0 , 0, -angle + 30));
                slashEffect.StartRoutine();

                PoolingManager.Instance.Push(this);
            }
        }
    }
}
