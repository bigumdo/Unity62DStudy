using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;
using YUI.Agents.players;
using YUI.FSM;
using YUI.PatternModules;

namespace YUI.Agents.Bosses
{
    public class Boss : Agent
    {
        protected BehaviorGraphAgent _btAgent;
        public BossStateEnum CurrentPage { get; set; } = BossStateEnum.Phase1;
        public bool IsDamageable { get; private set; } = false;
        public Vector3 centerPos;
        public BossPatternDataSO patternDataSO;

        protected override void Awake()
        {
            base.Awake();
            _btAgent = GetComponent<BehaviorGraphAgent>();
            _btAgent.enabled = false;
        }

        public void StartBT()
        {
            _btAgent.enabled = true;
        }

        public void StopBT()
        {
            _btAgent.enabled = false;
        }

        public BlackboardVariable<T> GetVariable<T>(string variableName)
        {
            if (_btAgent.GetVariable(variableName, out BlackboardVariable<T> variable))
                return variable;
            return null;
        }

        public void SetVariable<T>(string variableName, T value)
        {
            BlackboardVariable<T> variable = GetVariable<T>(variableName);
            Debug.Assert(variable != null, $"Variable {variableName} not found");
            variable.Value = value;
        }

        public virtual IEnumerator DeadEffect()
        {
            yield return null;
        }

        public void SetDamageable(bool value) => IsDamageable = value;
    }
}
