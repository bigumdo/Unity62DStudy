using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace YUI.FSM
{
    [CreateAssetMenu(menuName = "SO/FSM/AgentStateListSO")]
    public class AgentStateListSO : ScriptableObject
    {
        public List<StateSO> states = new List<StateSO>();
    }
}
