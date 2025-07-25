using UnityEngine;
using YUI.Animators;
using YUI.FSM;

namespace YUI.Agents.players {
    public class PlayerRunState : PlayerCanAttackState {
        private PlayerMover mover;
        private InputReader inputReader;

        private Player player;
        private AgentRenderer agentRenderer;

        public PlayerRunState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;

            mover = agent.GetCompo<PlayerMover>();
            inputReader = (agent as Player).InputReader;
            agentRenderer = agent.GetCompo<AgentRenderer>();
        }

        public override void Update()
        {
            base.Update();

            Move();
        }

        private void Move() {
            if (inputReader.SlowMode && mover.moveType != PlayerMoveType.NORMAL)
            {
                mover.SetMovement(inputReader.MovementVector);
            }
            else if (mover.moveType == PlayerMoveType.NORMAL)
            {
                mover.SetMovement(inputReader.MovementVector);
                agentRenderer.SetParam(player.MovementX, inputReader.MovementVector.x);
                agentRenderer.SetParam(player.MovementY, inputReader.MovementVector.y);
            }
            else
            {
                mover.SetMovement(inputReader.Movement);

            }
        }
    }
}
