using UnityEngine;
using YUI.Agents.AfterImages;
using YUI.Animators;
using YUI.FSM;

namespace YUI.Agents.players {
    public class PlayerStayState : AgentState {
        private Player player;
        private PlayerMover mover;
        private AgentAfterImage agentAfterimage;

        public PlayerStayState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            mover = player.GetCompo<PlayerMover>(true);
            agentAfterimage = player.GetCompo<AgentAfterImage>(true);
        }

        public override void Enter()
        {
            base.Enter();

            agentAfterimage.Stop();

            mover.StopImmediately();
        }

        public override void Exit()
        {
            agentAfterimage.Play();

            base.Exit();
        }
    }
}
