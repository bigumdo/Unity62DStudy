using YUI.StatSystem;
using System;
using UnityEngine;

namespace YUI.Agents
{
    public class AgentHealth : MonoBehaviour, IAgentComponent, IAfterInit
    {
        protected Agent _agent;
        protected AgentStat _agentStat;

        protected float _currentHealth;
        protected float _maxHealth;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            _agentStat = agent.GetCompo<AgentStat>();
        }

        public virtual void AfterInit()
        {
            _currentHealth = _maxHealth = _agentStat.HpStat.Value;
        }

        public virtual void ApplyDamage(float damage)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            if (_currentHealth <= 0)
                _agent.OnDeadEvent?.Invoke();
            else
                _agent.OnHitEvent?.Invoke();
        }

        public float GetCurrentHp()
        {
            return _currentHealth;
        }

        public float GetMaxHp()
        {
            return _maxHealth;
        }
    }
}
