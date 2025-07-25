using UnityEngine;
using YUI.StatusEffects;

namespace YUI.Fields {
    [CreateAssetMenu(fileName = "ForceSlowModeEvent", menuName = "Field/Events/ForceSlowModeEvent")]
    public class ForceSlowModeEvent : FieldEventSO {
        [SerializeField] private float slowModeDuration;

        public override void Execute()
        {
            base.Execute();

            StatusEffectManager.Instance.AddStatusEffect(StatusEffectType.ForceSlowMode, slowModeDuration);
        }
    }
}
