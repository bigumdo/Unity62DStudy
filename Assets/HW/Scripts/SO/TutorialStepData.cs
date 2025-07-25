using UnityEngine;
using UnityEngine.Events;
namespace YUI
{
    public enum TutorialConditionType
    {
        None,
        Move,
        SlowMode,
        Dash,
        Attack,
        AttackHold,
        OverloadAbove50,
        Overload100,
        EatApple,
        CounterReady,
        Custom,
        Start,
        Adrenaline,
        GaugeManage,
        Shield,
        AvoidAttack,
    }

    [CreateAssetMenu(menuName = "Tutorial/Step")]
    public class TutorialStepData : ScriptableObject
    {
        [TextArea] public string tooltipText;

        [Space(20)]
        public TutorialConditionType conditionType;

        [Space(20)]
        public UnityEvent onEnter;
        public UnityEvent onComplete;

        public bool hasDialog;
        public string dialogKey;

        [Space(20)]
        public TutorialStepData nextStep;
    }

}
