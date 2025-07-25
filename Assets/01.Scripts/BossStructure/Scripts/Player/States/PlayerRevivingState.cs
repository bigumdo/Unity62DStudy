using System.Collections;
using UnityEngine;
using YUI.Animators;
using YUI.Fields;
using YUI.FSM;

namespace YUI.Agents.players  {
    public class PlayerRevivingState : AgentState {
        private Player player;
        private PlayerMover mover;

        public PlayerRevivingState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            mover = (agent as Player).GetCompo<PlayerMover>(true);
        }

        public override void Enter()
        {
            base.Enter();

            mover.SetMovement(PlayerMoveDir.STAY);

            player.transform.Find("Visual").gameObject.SetActive(false);

            player.StartCoroutine(ReviveRoutine());
        }

        private IEnumerator ReviveRoutine()
        {
            player.transform.position = FieldManager.Instance.GetPlayerSpawnPoint();
            yield return new WaitForSeconds(1f);
            player.transform.Find("Visual").gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            mover.SetMovement(PlayerMoveDir.RIGHT);
            player.ChangeState("RUN");
        }
    }
}
