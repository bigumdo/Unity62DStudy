using UnityEngine;
using YUI.Animators;
using YUI.Cores;
using YUI.FSM;
using YUI.ObjPooling;

namespace YUI.Agents.players {
    public class PlayerCounterState : PlayerCanAttackState {
        private Player player;
        private InputReader inputReader;
        private PlayerMover mover;

        private CounterEffect counterEffect;

        public PlayerCounterState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            inputReader = (agent as Player).InputReader;
            mover = (agent as Player).GetCompo<PlayerMover>(true);
        }

        public override void Enter()
        {
            base.Enter();

            player.IsCoating = true;
            player.LastCounterTime = Time.time;

            player.StartDelayCallback(7f, () =>
            {
                player.IsCoating = false;

                player.ChangeState("RUN");
            });

            SoundManager.Instance.PlaySound("SFX_Player_SkillCounter");

            counterEffect = PoolingManager.Instance.Pop("CounterEffect") as CounterEffect;
        }

        public override void Update() {
            base.Update();

            counterEffect.transform.position = player.transform.position;

            Move();
        }

        public override void Exit() {
            PoolingManager.Instance.Push(counterEffect);

            base.Exit();
        }

        private void Move() {
            if (player.PlayerMode == PlayerMode.NORMAL) {
                inputReader.SetSlowMode(true);
            }

            if (inputReader.SlowMode) {
                mover.SetMovement(inputReader.MovementVector);
            }
            else {
                mover.SetMovement(inputReader.Movement);
            }
        }

    }
}
