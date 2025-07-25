using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.Agents.Bosses;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossDeadAction", story: "BossDeadAction", category: "Action", id: "bb9d78113f449541db38b55f89e7b585")]
public partial class BossDeadAction : Action
{

    protected override Status OnStart()
    {
        BossManager.Instance.BossDeadPlay();
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

