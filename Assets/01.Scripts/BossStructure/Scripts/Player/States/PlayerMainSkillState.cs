using YUI.Animators;
using YUI.FSM;
using YUI.Skills;

namespace YUI.Agents.players {
    public class PlayerMainSkillState : AgentState {
        private Player player;
        private PlayerSkill playerSkill;

        public PlayerMainSkillState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            playerSkill = player.GetCompo<PlayerSkill>(true);
        }

        private ActiveSkill mainSkill;

        public override void Enter()
        {
            base.Enter();

            mainSkill = playerSkill.GetMainSkillSO();
            mainSkill.ExecuteSkill(player);

            player.ChangeState("RUN");
        }
    }
}
