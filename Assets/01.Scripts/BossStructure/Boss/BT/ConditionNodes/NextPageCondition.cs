using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NextPageCondition", story: "[Boss] [CurrentPhase]", category: "Conditions", id: "a37da13f834ab298001f0fa0994591a0")]
public partial class NextPageCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BossStateEnum> CurrentPhase;
    [SerializeReference] public BlackboardVariable<Boss> Boss;

    public override bool IsTrue()
    {
        return Boss.Value.CurrentPage != CurrentPhase.Value;
    }


    public override void OnStart()
    {
    }


    public override void OnEnd()
    {
    }
}
