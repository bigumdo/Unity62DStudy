using YUI.Agents;
using System;
using UnityEngine;

namespace YUI.Agents
{
    public class AgentAnimationTrigger : MonoBehaviour, IAgentComponent
    {
        public event Action OnAnimationEndTrigger;
        public event Action OnAttackTrigger;

        protected Agent _agent;
        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
        }

        protected virtual void AniamtionEnd() => OnAnimationEndTrigger?.Invoke();   
        protected virtual void AttackTrigger() => OnAttackTrigger?.Invoke();
    }
}
