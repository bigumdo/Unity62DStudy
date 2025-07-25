using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.Agents.Bosses;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossPhaseDissolve", story: "[Boss] PhaseDissolve [Value] [Duration]", category: "Action", id: "d15778d1289a48298ec60b85a67e0c12")]
public partial class BossPhaseDissolveAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<float> Value;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private bool _isFinished;

    protected override Status OnStart()
    {
        Boss.Value.StartCoroutine(Dissolve());
        return Status.Running;
    }

    private IEnumerator Dissolve()
    {
        _isFinished = false;
        yield return Boss.Value.GetCompo<BossRenderer>().PhaseDissolve(Value.Value,Duration.Value);
        _isFinished = true;
    }

    protected override Status OnUpdate()
    {
        if(_isFinished)
            return Status.Success;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

