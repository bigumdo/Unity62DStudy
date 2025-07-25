using System.Collections;
using UnityEngine;
using YUI.Animators;
using YUI.FSM;
using YUI.StatSystem;

namespace YUI.Agents.players {
    public class PlayerStunState : AgentState {
        private Player player;
        private AgentStat stat;
        private PlayerMover mover;

        public PlayerStunState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;

            stat = player.GetCompo<AgentStat>(true);
            mover = player.GetCompo<PlayerMover>(true);
        }

        public override void Enter() {
            base.Enter();

            player.InputReader.SetSlowMode(true);
            player.InputReader.Enable(false);

            mover.StopImmediately();

            stat.AddModifier(player.OverloadDecreaseStat, "STUN", -player.OverloadDecreaseStat.Value);

            player.StartCoroutine(Routine());
        }

        private IEnumerator Routine() {
            yield return new WaitForSeconds(2f);

            player.ChangeState("RUN");
        }

        public override void Exit() {
            player.InputReader.SetSlowMode(false);
            player.InputReader.Enable(true);

            stat.RemoveModifier(player.OverloadDecreaseStat, "STUN");

            base.Exit();
        }
    }
}
