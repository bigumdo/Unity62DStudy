using System.Collections.Generic;
using System.Linq;
using YUI.Agents;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YUI.StatSystem
{
    public class AgentStat : MonoBehaviour, IAgentComponent
    {
        [Header("Frequently used Stat")]
        [SerializeField] private StatSO _hpStat;
        [SerializeField] private StatSO _moveSpeed;

        [Space]
        [SerializeField] private StatOverride[] _statOverrides;
        private StatSO[] _stats; //this is real stat
        public Agent Owner { get; private set; }
        public StatSO HpStat { get; private set; }
        public StatSO MoveSpeedStat { get; private set; }
        
        public void Initialize(Agent agent)
        {
            Owner = agent;

            //스텟 오버라이드 후 자주쓰는 스탯은 뽑아서 저장해둔다.
            _stats = _statOverrides.Select(x => x.CreateStat()).ToArray();
            HpStat = _hpStat ? GetStat(_hpStat) : null;
            MoveSpeedStat = _moveSpeed ? GetStat(_moveSpeed) : null;
        }

        public StatSO GetStat(StatSO stat)
        {
            //인자가 False면 에러메시지를 보여줌
            Debug.Assert(stat != null, $"Stats::Getstat- stat은 null일 수 없습니다.");
            return _stats.FirstOrDefault(x => x.statName == stat.statName);
        }

        public void SetBaseValue(StatSO stat, float value)
            => GetStat(stat).BaseValue = value;

        public float GetBaseValue(StatSO stat)
            => GetStat(stat).BaseValue;

        public void IncreaseBaseValue(StatSO stat, float value)
            => GetStat(stat).BaseValue += value;

        public void AddModifier(StatSO stat, string key, float value)
            => GetStat(stat).AddModifier(key, value);
        public void RemoveModifier(StatSO stat, string key)
            => GetStat(stat).RemoveModifier(key);
    }
}
