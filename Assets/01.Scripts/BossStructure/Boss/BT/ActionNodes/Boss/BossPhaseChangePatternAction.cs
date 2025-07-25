using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.PatternModules;
using System.Collections;
using YUI.Agents.players;
using YUI.Agents;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossPhaseChangePatternAction", story: "[Boss] PhaseChangePattern", category: "Action", id: "59fb6092ed7d6c13dd3ad96f524a8307")]
public partial class BossPhaseChangePatternAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<string> Pattern;

    private bool _isEnd;
    private PatternSO _pattern;

    protected override Status OnStart()
    {
        _pattern = PatternManager.Instance.GetPhaseChagnePattern();
        Debug.Assert(_pattern != null, $"{Pattern.Value}°¡ ¾øÀ½");
        Boss.Value.StartCoroutine(PatternExecute());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isEnd)
        {
            _isEnd = false;
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        base.OnEnd();
    }

    private IEnumerator PatternExecute()
    {
        yield return _pattern.Execute(Boss.Value);
        _isEnd = true;
    }
}

