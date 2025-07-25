using System;
using Unity.Behavior;
using UnityEngine;
using YUI.PatternModules;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using YUI.Agents.Bosses;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatternModule", story: "[PatternName] [Boss]", category: "Action", id: "2ccad78a447e321ca1ea90f2b796607b")]
public partial class PatternModuleAction : Action
{
    [SerializeReference] public BlackboardVariable<string> PatternName;
    [SerializeReference] public BlackboardVariable<Boss> Boss;

    private bool _isEnd;
    private PatternSO _pattern;

    protected override Status OnStart()
    {
        if (_pattern == null)
            _pattern = PatternManager.Instance.GetPhase1Pattern(PatternName.Value);
        Debug.Assert(_pattern != null, $"{PatternName.Value}°¡ ¾øÀ½");
        Boss.Value.StartCoroutine(PatternExecute());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(_isEnd)
        {
            _isEnd = false;
            return Status.Success;
        }
        return Status.Running;
    }


    private IEnumerator PatternExecute()
    {
        yield return _pattern.Execute(Boss.Value);
        _isEnd = true;
    }
}

