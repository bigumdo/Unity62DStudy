using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.Agents.Bosses;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetWordBulletSpaner", story: "Set WordSpawner [Value]", category: "Action", id: "7a2908a1414baeef44e5cec63a890ccb")]
public partial class SetWordBulletSpanerAction : Action
{
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        PatternManager.Instance.SetWordSpawner(Value.Value);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

