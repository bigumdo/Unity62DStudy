using System;
using System.Collections;
using UnityEngine;
using YUI.Agents.AfterImages;
using YUI.Agents.Bosses;
using YUI.Animators;
using YUI.Cores;
using YUI.FSM;
using YUI.ObjPooling;
using YUI.StatusEffects;

namespace YUI.Agents.players {
    public class PlayerDashState : AgentState
    {
        private Player player;
        private InputReader inputReader;
        private PlayerMover mover;
        private PlayerSkill playerSkill;
        private PlayerHealth playerHealth;
        private AgentAfterImage agentAfterimage;

        public PlayerDashState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            mover = player.GetCompo<PlayerMover>();
            inputReader = (agent as Player).InputReader;
            playerSkill = player.GetCompo<PlayerSkill>();
            playerHealth = player.GetCompo<PlayerHealth>(true);

            agentAfterimage = player.GetCompo<AgentAfterImage>(true);
        }

        private Vector2 dashDirection;
        private float chargingPercent;
        private bool isPressed;
        private bool isDashEnd = false;

        public override void Enter()
        {
            base.Enter();
            inputReader.DashEvent += HandleDashEvent;

            isDashEnd = false;
            agentAfterimage.Stop();

            BossManager.Instance.OnCounterSuccessEvent += HandleCounterSuccess;
            chargingPercent = 0;
        }

        public override void Update()
        {
            base.Update();

            if (isDashEnd)
            {
                player.ChangeState("RUN");
            }

            Move();
        }

        private void Move()
        {
            if (!player.CanMoveAtDash) return;

            if (inputReader.SlowMode)
            {
                mover.SetMovement(inputReader.MovementVector);
            }
            else
            {
                mover.SetMovement(inputReader.Movement);
            }
        }

        public override void Exit()
        {
            inputReader.DashEvent -= HandleDashEvent;
            BossManager.Instance.OnCounterSuccessEvent -= HandleCounterSuccess;

            agentAfterimage.Play();

            playerHealth.SetInvincible(false);
            player.LastDashTime = Time.time;

            base.Exit();
        }

        private void HandleCounterSuccess()
        {
            StatusEffectManager.Instance.AddStatusEffect(StatusEffectType.Counter, 5);
            player.GetCompo<PlayerOverload>().SetOverload(50);
            SoundManager.Instance.PlaySound("SFX_CounterSuccess");

            player.StartCoroutine(HitStopRoutine());
        }

        private IEnumerator HitStopRoutine()
        {
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(0.1f);

            Time.timeScale = 1;
        }

        private IEnumerator DashChargingRoutine()
        {
            float elapsedTime = 0;

            mover.StopImmediately();

            while (isPressed)
            {
                if (elapsedTime > player.DashChargingStartTime.Value)
                {
                    chargingPercent = Mathf.Clamp01(elapsedTime / player.DashChargingEndTime.Value);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            player.StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            //mover.StopImmediately();
            playerHealth.SetInvincible(true);
            player.UseDash();

            dashDirection = new Vector3(inputReader.MovementVector.x, inputReader.MovementVector.y).normalized;

            if (dashDirection == Vector2.zero)
            {
                switch (inputReader.Movement)
                {
                    case PlayerMoveDir.UP:
                        dashDirection = new Vector2(0, 1);
                        break;
                    case PlayerMoveDir.DOWN:
                        dashDirection = new Vector2(0, -1);
                        break;
                    case PlayerMoveDir.RIGHT:
                        dashDirection = new Vector2(1, 0);
                        break;
                    case PlayerMoveDir.LEFT:
                        dashDirection = new Vector2(-1, 0);
                        break;
                }
            }

            SoundManager.Instance.PlaySound("SFX_Player_StartDash");

            float targetDistance = player.DashDistanceStat.Value * (chargingPercent == 1 ? player.ChargingDashDistanceMultiplier.Value : 1f);
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = startPos + (Vector3)(dashDirection * targetDistance);

            RaycastHit2D wallChecker = Physics2D.Raycast(startPos, dashDirection, targetDistance, LayerMask.GetMask("Wall"));
            RaycastHit2D bossChecker = Physics2D.Raycast(startPos, dashDirection, targetDistance, LayerMask.GetMask("Boss"));

            if (wallChecker.collider != null)
            {
                targetPos = wallChecker.point;
                targetPos.z = player.transform.position.z;
            }

            if (bossChecker.collider != null)
            {
                if (bossChecker.collider.gameObject.layer == LayerMask.NameToLayer("Boss"))
                {

                    if (playerSkill.GetVariable("CriticalCatch") != null)
                    {
                        playerSkill.AddVariable("CriticalCatch", true);
                    }

                    if (chargingPercent == 1)
                    {
                        BossManager.Instance.Counter();

                        if (bossChecker.collider.gameObject.TryGetComponent(out Agent agent))
                        {
                            CameraManager.Instance.ShakeCamera(3f, 4f, 0.1f);

                            agent.GetCompo<AgentHealth>(true).ApplyDamage(player.AttackStat.Value * player.ChargingDashDamageMultiplier.Value);

                            GameObject go = PoolingManager.Instance.Pop("HitEffect").gameObject;

                            Vector3 direction = agent.transform.position - player.transform.position;
                            direction.Normalize();

                            go.transform.position = BossManager.Instance.Boss.transform.position;

                            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                            go.transform.localRotation = Quaternion.Euler(0, 0, -angle);
                        }
                    }
                }
            }

            float dashEffectTime = 0.1f;

            DashEffect dashEffect = PoolingManager.Instance.Pop("DashEffect") as DashEffect;
            dashEffect.SetStartPos(player.transform.position);
            dashEffect.SetEndPos(targetPos);
            dashEffect.StartRoutine(dashEffectTime);

            yield return new WaitForSeconds(dashEffectTime);

            float dashSpeed = 50f;

            while (Vector3.Distance(player.transform.position, targetPos) > 0.05f)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, dashSpeed * Time.deltaTime);
                yield return null;
            }

            isDashEnd = true;
            dashCoroutine = null;

            player.transform.position = targetPos;
            player.ChangeState("RUN");

            yield return null;
        }

        private Coroutine dashCoroutine;

        private void HandleDashEvent(bool value)
        {
            isPressed = value;

            if (value)
            {
                if (dashCoroutine != null) return;

                dashCoroutine = player.StartCoroutine(DashChargingRoutine());
            }
        }
    }
}
