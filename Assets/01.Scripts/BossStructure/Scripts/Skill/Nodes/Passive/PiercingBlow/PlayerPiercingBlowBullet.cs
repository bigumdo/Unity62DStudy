using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;
using YUI.Skills;

namespace YUI.Bullets {

    public class PlayerPiercingBlowBullet : Bullet {
        [SerializeField] private LayerMask whatIsBoss;
        [SerializeField] private float maxScaleMultiplier;
        [SerializeField] private float maxIncreaseDamage;
        private bool chargingComplete = false;
        private float chargePercent;
        private Coroutine chargingRoutine;
        

        private Vector3 defaultScale;
        private float defaultDamage;

        private void Awake() {
            defaultScale = transform.localScale;

            transform.localScale = Vector3.zero;

            defaultDamage = _damage;
        }

        public override void ResetItem()
        {
            base.ResetItem();

            transform.localScale = Vector3.zero;

            chargingComplete = false;
            chargingRoutine = null;
            chargePercent = 0;

            SetDamage(defaultDamage);
        }

        public bool IsChargingComplete() => chargingComplete;

        public void StartChargingRoutine(float chargingTime, Player player, Transform followTrm) {
            chargingRoutine = StartCoroutine(ChargingRoutine(chargingTime, player, followTrm));
        }

        public void StartShootRoutine() {
            StartCoroutine(ShootRoutine());
        }

        private IEnumerator ChargingRoutine(float chargingTime, Player player, Transform followTrm) {
            float elapsedTime = 0f;

            while (true) {
                yield return null;

                elapsedTime += Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, followTrm.position, 10 * Time.deltaTime);
                transform.rotation = followTrm.rotation;
                
                chargePercent = elapsedTime / chargingTime;

                transform.localScale = defaultScale * Mathf.Lerp(0, maxScaleMultiplier, chargePercent);

                if (elapsedTime >= chargingTime) {
                    chargingComplete = true;
                }
            }
        }

        private IEnumerator ShootRoutine() {
            if (chargingRoutine != null) {
                StopCoroutine(chargingRoutine);
            }

            SoundManager.Instance.PlaySound("SFX_Player_ShotSubBullet");

            SetDamage(defaultDamage);
            AddDamage(maxIncreaseDamage * chargePercent);

            Vector3 moveDir = Vector3.zero;

            switch (PlayerManager.Instance.Player.GetCompo<PlayerOrbiter>().playerMoveDir) {
                case PlayerMoveDir.UP: {
                    moveDir = new Vector3(0, 0, 1);
                    break;
                }
                case PlayerMoveDir.DOWN: {
                    moveDir = new Vector3(0, 0, -1);
                    break;
                }
                case PlayerMoveDir.RIGHT: {
                    moveDir = new Vector3(1, 0, 0);
                    break;
                }
                case PlayerMoveDir.LEFT: {
                    moveDir = new Vector3(-1, 0, 0);
                    break;
                }
            }

            while (true) {
                transform.position += moveDir * _speed * Time.deltaTime;

                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)  {
            if ((whatIsBoss & (1 << other.gameObject.layer)) != 0) {
                float appliedDamage = PlayerManager.Instance.Player.AttackStat.Value * _damage;

                if (other.gameObject.TryGetComponent(out Agent agent))
                {
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
            }
        }
    }
}
