using YUI.Agents;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace YUI.FSM
{
    public class StateMachine
    {
        public AgentState currentState { get; private set; }

        private Dictionary<string, AgentState> _states;

        public StateMachine(Agent agent, AgentStateListSO stateList)
        {
            _states = new Dictionary<string, AgentState>();
            foreach (StateSO state in stateList.states)
            {
                try
                {
                    Type t = Type.GetType(state.className);
                    var entityState = Activator.CreateInstance(t, agent, state.animParam) as AgentState;
                    entityState.SetStateName(state.stateName);
                    _states.Add(state.stateName, entityState);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{state.stateName}?��? ???????? , Error.Message : {ex.Message}");
                }
            }
        }

        public void Initialize(string startState)
        {
            currentState = GetState(startState);
            currentState.Enter();
        }

        public void ChangeState(string changeState)
        {
            currentState.Exit();
            currentState = GetState(changeState);
            currentState.Enter();
        }

        public AgentState GetState(string name) => _states.GetValueOrDefault(name);
    }
}
