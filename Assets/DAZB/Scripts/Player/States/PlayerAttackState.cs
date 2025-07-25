using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Animators;
using YUI.Bullets;
using YUI.Cores;
using YUI.FSM;
using YUI.ObjPooling;
using YUI.Skills;

namespace YUI.Agents.players {
    public class PlayerAttackState : AgentState
    {
        private Player player;
        private InputReader inputReader;
        private PlayerMover mover;
        private PlayerOrbiter orbiter;
        private PlayerSkill playerSkill;

        public PlayerAttackState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            inputReader = player.InputReader;
            mover = player.GetCompo<PlayerMover>(true);
            orbiter = player.GetCompo<PlayerOrbiter>(true);
            playerSkill = player.GetCompo<PlayerSkill>(true);
        }

        private bool isPressed = false;
        private bool isSlowPressed = false;
        private bool isAttackEnd = false;

        public override void Enter()
        {
            base.Enter();
            inputReader.AttackEvent += HandleAttackEvent;
        }

        public override void Update()
        {
            base.Update();
            isAttackEnd = false;
            isSlowPressed = Keyboard.current.spaceKey.isPressed;

            if (isAttackEnd)
            {
                player.ChangeState("RUN");
            }

            Move();
        }

        public override void Exit()
        {
            if (this == null)
            {
                player.ChangeState("RUN");
                return;
            }

            if (!isSlowPressed)
            {
                inputReader.SetSlowMode(false);
            }

            player.LastAttackTime = Time.time;

            /* if (isFirst) {
                isFirst = false;
            } */

            inputReader.AttackEvent -= HandleAttackEvent;

            base.Exit();
        }

        private void Move()
        {
            if (inputReader.SlowMode)
            {
                mover.SetMovement(inputReader.MovementVector);
            }
            else
            {
                mover.SetMovement(inputReader.Movement);
            }
        }

        private IEnumerator PiercingBlowChargeRoutine()
        {
            int currentCount = 0;

            float individualChargingCompleteTime = player.IndividualChargingCompleteTimeStat.Value;
            float TotalChargingCompleteTime = player.TotalChargingCompleteTimeStat.Value;

            PlayerPiercingBlowBullet spawnedBullet = null;

            while (isPressed)
            {
                inputReader.SetSlowMode(true);

                orbiter.SetOrbiterCount(1);
                orbiter.SetFixed(true);
                orbiter.SetPlayerMoveDir(inputReader.PrevMovement);

                float targetTime = TotalChargingCompleteTime + individualChargingCompleteTime;

                yield return null;

                if (spawnedBullet != null)
                {
                    if (!spawnedBullet.IsChargingComplete()) continue;
                }

                if (currentCount >= 1)
                {
                    continue;
                }

                PlayerPiercingBlowBullet bullet = PoolingManager.Instance.Pop("PlayerPiercingBlowBullet") as PlayerPiercingBlowBullet;

                if (playerSkill.GetVariable("CriticalCatch") != null)
                {

                    if ((bool)playerSkill.GetVariable("CriticalCatch"))
                    {
                        bullet.SetDamage(15);
                    }
                }

                playerSkill.PassiveSkillExecution(SkillExecutionType.Attack);

                bullet.transform.position = player.transform.position;
                bullet.StartChargingRoutine(targetTime, player, orbiter.GetOrbiters()[0]);
                spawnedBullet = bullet;

                currentCount++;
            }

            if (spawnedBullet != null)
            {
                spawnedBullet.StartShootRoutine();
            }

            player.GetCompo<PlayerOverload>().AddOverload(player.OverloadIncreaseStat.Value);


            if (player.PlayerMode != PlayerMode.OVERLOAD)
            {
                player.ChangeState("RUN");
            }

            yield return null;
        }

        private IEnumerator ChargeRoutine()
        {
            bool isFirst = true;

            int currentCount = 0;

            float shootDelay = player.chargingAttackDelayStat.Value;
            float chargingCount = player.ChargingCountStat.Value;
            float individualChargingCompleteTime = player.IndividualChargingCompleteTimeStat.Value;
            float TotalChargingCompleteTime = player.TotalChargingCompleteTimeStat.Value;

            SoundManager.Instance.PlaySound("SFX_Player_StartCharging");

            List<PlayerNormalModeBullet> spawnedBulletList = new List<PlayerNormalModeBullet>();

            while (isPressed)
            {
                inputReader.SetSlowMode(true);
                orbiter.SetOrbiterCount(currentCount);
                float targetTime = isFirst ? individualChargingCompleteTime : (TotalChargingCompleteTime - individualChargingCompleteTime) / chargingCount;

                yield return null;

                if (spawnedBulletList.Count != 0 && !spawnedBulletList[currentCount - 1].IsChargingComplete())
                {
                    continue;
                }

                if (currentCount >= chargingCount + 1)
                {
                    continue;
                }

                PlayerNormalModeBullet bullet = PoolingManager.Instance.Pop("PlayerNormalModeBullet") as PlayerNormalModeBullet;
                SoundManager.Instance.PlaySound("SFX_Player_SpawnBullet");

                if (playerSkill.GetVariable("CriticalCatch") != null)
                {

                    if ((bool)playerSkill.GetVariable("CriticalCatch"))
                    {
                        bullet.SetDamage(15);
                    }
                }

                playerSkill.PassiveSkillExecution(SkillExecutionType.Attack);

                bullet.SetPosition(player.transform.position);
                bullet.StartChargingRoutine(targetTime, player, orbiter.GetOrbiters()[currentCount]);
                spawnedBulletList.Add(bullet);

                currentCount++;

                isFirst = false;
            }

            foreach (var bullet in spawnedBulletList)
            {
                bullet.StartShootRoutine();

                yield return new WaitForSeconds(shootDelay);
            }

            player.GetCompo<PlayerOverload>().AddOverload(player.OverloadIncreaseStat.Value);

            attackCoroutine = null;
            isAttackEnd = true;

            if (player.PlayerMode != PlayerMode.OVERLOAD)
            {
                player.ChangeState("RUN");
            }

            yield return null;
        }
        
        private Coroutine attackCoroutine;
        
        private void HandleAttackEvent(bool value)
        {
            isPressed = value;
            if (value)
            {
                if (attackCoroutine != null) return;

                if (player.PlayerMode != PlayerMode.OVERLOAD && !playerSkill.CheckPassiveSkill("PiercingBlow"))
                {
                    attackCoroutine = player.StartCoroutine(ChargeRoutine());
                }
                else if (player.PlayerMode != PlayerMode.OVERLOAD && playerSkill.CheckPassiveSkill("PiercingBlow"))
                {
                    attackCoroutine = player.StartCoroutine(PiercingBlowChargeRoutine());
                }
            }
        }
    }
}
