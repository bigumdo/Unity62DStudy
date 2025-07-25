using YUI.Agents;
using YUI.Animators;

namespace YUI.FSM
{
    public abstract class AgentState
    {
        protected Agent _agent;
        protected AnimParamSO _animParam;
        protected bool _isEndTrigger;

        protected AgentRenderer _renderer;
        protected AgentAnimationTrigger _animTrigger;

        protected string stateName;

        public AgentState(Agent agent, AnimParamSO animParam)
        {
            _agent = agent;
            _animParam = animParam;
            /* _renderer = agent.GetCompo<AgentRenderer>(true); */
            _animTrigger = _agent.GetCompo<AgentAnimationTrigger>(true);
        }

        public void SetStateName(string stateName) => this.stateName = stateName;

        public virtual void Enter()
        {
            _isEndTrigger = false;
            /* _renderer.SetParam(_animParam, true); */
            _animTrigger.OnAnimationEndTrigger += AnimationEndTrigger;

            _agent.CurrentState = stateName;
        }

        public virtual void Exit()
        {
            /* _renderer.SetParam(_animParam, false); */
            _animTrigger.OnAnimationEndTrigger -= AnimationEndTrigger;
        }

        public virtual void Update()
        {

        }

        public virtual void AnimationEndTrigger()
        {
            _isEndTrigger = true;
        }
    }
}
