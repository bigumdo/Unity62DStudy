using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossHpResetAction", story: "[Boss] HpReset", category: "Action", id: "db442ec1a0c627c78c7c63ef3e0fb1d3")]
public partial class BossHpResetAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;

    protected override Status OnStart()
    {
        Boss.Value.GetCompo<BossHealth>().ResetHp();
        return Status.Success;
    }
}

