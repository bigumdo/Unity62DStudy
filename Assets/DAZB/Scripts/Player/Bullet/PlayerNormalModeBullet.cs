using System.Collections;
using UnityEngine;
using YUI.Agents;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;
using YUI.Skills;

namespace YUI.Bullets {
    public class PlayerNormalModeBullet : BezierBullet {
        [SerializeField] private LayerMask whatIsBoss;
        [SerializeField] private Vector3 maxScale = new Vector3(0.25f, 0.25f, 0.25f);
        [SerializeField] private float radiusX;
        [SerializeField] private float radiusZ;
        private bool chargingComplete = false;
        private float chargePercent;

        private Coroutine chargingRoutine;

        private TrailRenderer trailRenderer;
        private float defaultTrailTime;
        private float defaultWidth;

        private bool isShooting = false;

        private void Awake() {
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            trailRenderer.enabled = false;

            defaultTrailTime = trailRenderer.time;
            defaultWidth = trailRenderer.startWidth;
        }

        public void StartShootRoutine() {
            StartCoroutine(ShootRoutine());
        }

        public void StartChargingRoutine(float chargingTime, Player player, Transform followTrm) {
            chargingRoutine = StartCoroutine(ChargingRoutine(chargingTime, player, followTrm));
        }

        public void SetPosition(Vector3 position) {
            transform.position = position;
        }

        public override void ResetItem()
        {
            base.ResetItem();

            chargingRoutine = null;
            chargingComplete = false;
            chargePercent = 0;

            transform.localScale = Vector3.zero;
            trailRenderer.time = defaultTrailTime;

            isShooting = false;
        }

        public bool IsChargingComplete() => chargingComplete;
 
        private IEnumerator ChargingRoutine(float chargingTime, Player player, Transform followTrm) {
            trailRenderer.enabled = true;

            float elapsedTime = 0f;
            Vector3 lastPlayerPos = player.transform.position;

            while (true) {
                elapsedTime += Time.deltaTime;
                
                transform.position = Vector3.Lerp(transform.position, followTrm.position, 5 * Time.deltaTime);

                Vector3 currentPlayerPos = player.transform.position;
                Vector3 playerDelta = currentPlayerPos - lastPlayerPos;

                transform.position += playerDelta;

                lastPlayerPos = currentPlayerPos;

                chargePercent = Mathf.Clamp01(elapsedTime / chargingTime);

                transform.localScale = Vector3.Lerp(Vector3.zero, maxScale, chargePercent);

                if (elapsedTime >= chargingTime) {
                    chargingComplete = true;
                }

                yield return null;
            }
        }

        private IEnumerator ShootRoutine()
        {
            if (chargingRoutine != null)
            {
                StopCoroutine(chargingRoutine);
            }

            SoundManager.Instance.PlaySound("SFX_Player_ShotBullet");

            isShooting = true;

            trailRenderer.time = defaultTrailTime;
            trailRenderer.startWidth = defaultWidth;
            trailRenderer.enabled = true;

            Transform target = BossManager.Instance.Boss == null
                ? PlayerManager.Instance.Player.transform
                : BossManager.Instance.Boss.transform;

            Vector3 p0 = transform.position;
            Vector3 p3 = target.position;

            Vector3 p1 = GetRandomControlPoint(p0, radiusX, radiusZ);
            Vector3 p2 = GetRandomControlPoint(p3, radiusX, radiusZ);

            float totalLength = EstimateBezierLength(p0, p1, p2, p3);
            float duration = totalLength / _speed;

            float t = 0f;
            float arriveDistance = 0.1f;

            while (true)
            {
                target = BossManager.Instance.Boss == null ? PlayerManager.Instance.Player.transform : BossManager.Instance.Boss.transform;
                p3 = target.position;

                Vector3 pos = FourPointBezier(p0, p1, p2, p3, t);
                transform.position = pos;

                if (Vector3.Distance(transform.position, p3) <= arriveDistance)
                    break;

                t += Time.deltaTime / duration;
                t = Mathf.Clamp01(t);

                yield return null;
            }

            transform.position = p3;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (isShooting == false) return;

            if ((whatIsBoss & (1 << other.gameObject.layer)) != 0)
            {
                int appliedDamage = (int)(PlayerManager.Instance.Player.AttackStat.Value * _damage * chargePercent);

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

                PlayerManager.Instance.Player.GetCompo<PlayerSkill>().PassiveSkillExecution(SkillExecutionType.Hit);

                PoolingManager.Instance.Push(this);
            }
        }
    }
}
