using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Animators;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeAnimParamAction", story: "set [CurrentAnimParam] to [NextAnimParam]", category: "Action", id: "e1df30cda1c8fe50d818879c7babab06")]
public partial class ChangeAnimParamAction : Action
{
    [SerializeReference] public BlackboardVariable CurrentAnimParam;
    [SerializeReference] public BlackboardVariable NextAnimParam;

    protected override Status OnStart()
    {
        if(CurrentAnimParam == null || NextAnimParam == null)
        {
            return Status.Failure;
        }

        CurrentAnimParam.ObjectValue = NextAnimParam.ObjectValue;
        return Status.Success;
    }
}

