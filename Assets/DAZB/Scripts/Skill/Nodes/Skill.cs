using UnityEngine;
using YUI.Agents.players;

namespace YUI.Skills {
    public enum SkillExecutionType {
        Update, // 매 프레임마다
        Attack, // 공격시
        Hit, // 상대 피격시
        Hurt, // 자신 피격시
        Change, // 상태 변화시
        Eat, // 사과 먹을 시
        ShieldSkillStart, // 쉴드 스킬 시작
        ShieldSkillEnd, // 쉴드 스킬 종료시
        Heal, // 체력 회복시
        GaugeIncrease, // 게이지 상승시
        GaugeDecrease, // 게이지 감소시
    }
    public abstract class Skill : ScriptableObject {
        public float Cooldown;

        [Space]

        public SkillExecutionType ExecutionType;
        public string NodeName;
        [TextArea] public string NodeDescription;

        protected float lastExecutionTime = -Mathf.Infinity;
        
        public float LastExecutionTime => lastExecutionTime;

        public virtual bool CanExecuteSkill(Player player)
        {
            return Time.time >= lastExecutionTime + Cooldown;
        }

        protected virtual void OnEnable() {
            lastExecutionTime = -Mathf.Infinity;
        }

        public virtual void ExecuteSkill(Player player) {
            lastExecutionTime = Time.time;
        }

        public virtual void RestoreCooldownByPercent(float percent)
        {
            lastExecutionTime = Time.time - (Cooldown * percent);
        }

        public virtual void RestoreCooldownByTime(float time) {
            lastExecutionTime = Time.time - time;
        }
    }
}
