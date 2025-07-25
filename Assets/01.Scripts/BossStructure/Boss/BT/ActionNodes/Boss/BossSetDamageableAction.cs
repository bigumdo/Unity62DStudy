using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossSetDamageable ", story: "[Boss] SetDamageable [active]", category: "Action", id: "74b046d6d8a75dcbf9ddf74ecc0a9443")]
public partial class BossSetDamageableAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<bool> Active;

    protected override Status OnStart()
    {
        Boss.Value.SetDamageable(Active.Value);
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

