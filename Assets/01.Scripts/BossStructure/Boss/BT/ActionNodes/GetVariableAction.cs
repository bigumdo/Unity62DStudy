using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.Agents.Bosses;
using YUI.Agents;
using YUI.Agents.players;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetVariable", story: "get variable from [boss]", category: "Action", id: "8c5c19b1b99569209560cacf484de622")]
public partial class GetVariableAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;

    protected override Status OnStart()
    {

        Boss enemy = Boss.Value;

        enemy.SetVariable("Renderer", enemy.GetCompo<AgentRenderer>());
        enemy.SetVariable("AnimTrigger", enemy.GetCompo<AgentAnimationTrigger>());
        enemy.SetVariable("BossHealth", enemy.GetCompo<BossHealth>());
        enemy.SetVariable("Player", PlayerManager.Instance.Player);
        return Status.Running;
    }
}

