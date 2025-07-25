using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using YUI.Agents.players;

namespace YUI.Skills {
    public class PassiveSkill : Skill {
        public List<PassiveSkill> SubPassiveSkills;
        public bool OnlyStat;

        public virtual void ApplyStatModifier(Player player) { }
        public virtual void RemoveStatModifier(Player player) { }
    }
}
