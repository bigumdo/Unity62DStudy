using System.Collections.Generic;
using UnityEngine;
using YUI.Skills;

namespace YUI.Agents.players {

    public class PlayerSkill : MonoBehaviour, IAgentComponent, IAfterInit {
        public Player Player { get; private set; }
        [SerializeField] private ActiveSkill mainSkillSO;
        [SerializeField] private List<PassiveSkill> passiveNodeList = new List<PassiveSkill>();

        private Dictionary<string, object> skillData = new Dictionary<string, object>();
        
        public void Initialize(Agent agent) {
            Player = agent as Player;
        }

        public void AfterInit() {
            if (passiveNodeList.Count > 0) {
                passiveNodeList.ForEach(x => x.ApplyStatModifier(Player));
            }
        }

        public bool CheckPassiveSkill(string skillName) {
            foreach (PassiveSkill node in passiveNodeList) {
                if (node.name == skillName) {
                    return true;
                }
            }
            return false;
        }

        public void AddVariable(string key, object value) {
            if (skillData.ContainsKey(key)) {
                skillData[key] = value;
            } else {
                skillData.Add(key, value);
            }
        }

        public object GetVariable(string key) {
            if (skillData.ContainsKey(key)) {
                return skillData[key];
            }
            else {
                return null;
            }
        }

        private void Update() {
            foreach (PassiveSkill node in passiveNodeList)
            {
                if (node.ExecutionType == SkillExecutionType.Update && node.CanExecuteSkill(Player) && node.OnlyStat == false)
                {
                    node.ExecuteSkill(Player);
                }
                
                foreach (var subPassive in node.SubPassiveSkills)
                {
                    if (subPassive.ExecutionType == SkillExecutionType.Update && subPassive.CanExecuteSkill(Player) && subPassive.OnlyStat == false)
                    {
                        subPassive.ExecuteSkill(Player);
                    }
                }
            }
        }

        public void PassiveSkillExecution(SkillExecutionType type) {
            foreach (PassiveSkill node in passiveNodeList)
            {
                if (node.ExecutionType == type && node.CanExecuteSkill(Player))
                {
                    node.ExecuteSkill(Player);
                }
                
                foreach (var subPassive in node.SubPassiveSkills)
                {
                    if (subPassive.ExecutionType == type && subPassive.CanExecuteSkill(Player))
                    {
                        subPassive.ExecuteSkill(Player);
                    }
                }
            }
        }

        public void AddPassive(PassiveSkill passive) {
            passive.ApplyStatModifier(Player);

            passiveNodeList.Add(passive);
        }

        public void RemovePassive(PassiveSkill passive) {
            passive.RemoveStatModifier(Player);

            passiveNodeList.Remove(passive);
        }

        public void ChangeMainSkill(ActiveSkill newSkill) => mainSkillSO = newSkill;
        public ActiveSkill GetMainSkillSO() => mainSkillSO;
        public bool CheckPassiveNode(PassiveSkill passiveNode) => passiveNodeList.Contains(passiveNode);
    }
}
