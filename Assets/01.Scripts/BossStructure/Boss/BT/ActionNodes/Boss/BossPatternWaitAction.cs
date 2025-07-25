using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;
using YUI.Agents.Bosses;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossPatternWaitAction", story: "[Boss] CurretnPatternWait", category: "Action", id: "34510fe2686049aa035860f548589469")]
public partial class BossPatternWaitAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    private bool _canNext;

    protected override Status OnStart()
    {
        Boss.Value.StartCoroutine(Test());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_canNext)
            return Status.Success;
        else
            return Status.Running;
    }

    protected override void OnEnd()
    {
        _canNext = false;
    }

    private IEnumerator Test()
    {
        yield return new WaitUntil(() => !PatternManager.Instance.isExecutingPattern);
        _canNext = true;
    }
}

