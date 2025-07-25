using System.Collections;
using UnityEngine;
using YUI.Agents;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.Bullets {
    public class PlayerOverloadModeBullet : Bullet {
        [SerializeField] private LayerMask whatIsBoss;
        private TrailRenderer trailRenderer;

        private void Awake() {
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        public override void ResetItem()
        {
            base.ResetItem();
        }

        public void StartRoutine() {
            StartCoroutine(Routine());
        }

        public void SetPosition(Vector3 position) {
            transform.position = position;
        }

        private IEnumerator Routine() {
            trailRenderer.Clear();
            trailRenderer.enabled = true;

            Transform target = BossManager.Instance.Boss.transform;

            while (true) {
                transform.position += Time.deltaTime * _speed * (target.position - transform.position).normalized;

                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other) {

            if ((whatIsBoss & (1 << other.gameObject.layer)) != 0) {
                float appliedDamage = PlayerManager.Instance.Player.AttackStat.Value * _damage;

                if (other.gameObject.TryGetComponent(out AgentHealth health)) {
                    health.ApplyDamage(appliedDamage);
                }

                CameraManager.Instance.ShakeCamera(1f, 3f, 0.1f);

                trailRenderer.enabled = false;

                GameObject go = PoolingManager.Instance.Pop("HitEffect").gameObject;

                Vector3 direction = other.transform.position - transform.position;
                direction.Normalize();

                go.transform.position = transform.position;

                float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                go.transform.localRotation = Quaternion.Euler(-90, 0, -angle);

                PoolingManager.Instance.Push(this);
            }
        }
    }
}
