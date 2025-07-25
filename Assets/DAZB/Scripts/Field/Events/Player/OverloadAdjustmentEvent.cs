using UnityEngine;
using YUI.Agents.players;

namespace YUI.Fields {
    [CreateAssetMenu(fileName = "OverloadAdjustmentEvent", menuName = "Field/Events/OverloadAdjustmentEvent ")]
    public class OverloadAdjustmentEvent : FieldEventSO {
        [Header("증가/감소할 값")]
        [SerializeField] private float adjustValue;
        [SerializeField] private bool isApple;

        public override void Execute() {
            base.Execute();

            player.GetCompo<PlayerOverload>().AddOverload(-adjustValue);

            if (isApple) {
                PlayerSkill playerSkill = player.GetCompo<PlayerSkill>();
                playerSkill.PassiveSkillExecution(Skills.SkillExecutionType.Eat);

                if (player.PlayerMode == PlayerMode.RELEASE && playerSkill.GetVariable("AteCount") != null) {
                    playerSkill.AddVariable("AteCount", (int)playerSkill.GetVariable("AteCount") + 1);
                }
                else {
                    playerSkill.AddVariable("AteCount", 1);
                }
            }

            Destroy(owner.gameObject);
        }
    }
}
