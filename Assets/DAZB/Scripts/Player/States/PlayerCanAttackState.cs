using YUI.Animators;
using YUI.Cores;
using YUI.FSM;

namespace YUI.Agents.players {
    public class PlayerCanAttackState : AgentState
    {
        private Player player;
        private PlayerSkill playerSkill;
        private InputReader inputReader;
        private PlayerMover playerMover;

        public PlayerCanAttackState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            playerSkill = player.GetCompo<PlayerSkill>(true);
            inputReader = player.InputReader;
            playerMover = player.GetCompo<PlayerMover>(true);
        }

        public override void Enter()
        {
            base.Enter();

            inputReader.MainSkillEvent += HandleMainSkillEvent;
            //inputReader.CounterEvent += HandleCounterEvent;

            inputReader.DashEvent += HandleDashEvent;
            inputReader.AttackEvent += HandleAttackEvent;
        }

        public override void Exit()
        {
            inputReader.MainSkillEvent -= HandleMainSkillEvent;
            //inputReader.CounterEvent -= HandleCounterEvent;

            inputReader.DashEvent -= HandleDashEvent;
            inputReader.AttackEvent -= HandleAttackEvent;

            base.Exit();
        }

        private void HandleMainSkillEvent()
        {
            if (player.PlayerMode == PlayerMode.OVERLOAD) return;
            if (playerSkill.GetMainSkillSO() == null) return;
            if (playerSkill.GetMainSkillSO().CanExecuteSkill(player) == false) return;

            player.ChangeState("MAIN_SKILL");
        }

        /* private void HandleCounterEvent()
        {
            if (player.PlayerMode == PlayerMode.OVERLOAD || player.PlayerMode == PlayerMode.NORMAL) return;
            if (!player.CanCounter()) return;

            player.ChangeState("COUNTER");
        } */
        
        
        private void HandleDashEvent(bool value) {
            if (player.PlayerMode == PlayerMode.OVERLOAD) return;
            if (playerMover.moveType == PlayerMoveType.NORMAL) return;
            if (player.CanDash() == false) return;

            player.ChangeState("DASH");
        }

        private void HandleAttackEvent(bool value) {
            if (player.PlayerMode == PlayerMode.OVERLOAD) return;
            if (GameManager.Instance.IsBattle == false) return;
            if (!player.CanAttack()) return;

            player.ChangeState("ATTACK");
        }
    }
}
