using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.Agents.Bosses;
using YUI.PatternModules;
using System.Collections;
using YUI.Cores;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossRandomPattern", story: "[boss] [currentState] RandomPattern", category: "Action", id: "cc179344cb30d07e102416167cba7a4e")]
public partial class BossRandomPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<BossStateEnum> CurrentState;

    private bool _isEnd;
    private PatternSO _pattern;

    protected override Status OnStart()
    {   
        _pattern = PatternManager.Instance.GetRandomPattern(CurrentState.Value);
        Debug.Assert(_pattern != null, $"{CurrentState.Value.ToString()}°¡ ¾øÀ½");
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

    private IEnumerator PatternExecute()
    {
        yield return new WaitUntil(() => !GameManager.Instance.IsGmaeStop);
        yield return _pattern.Execute(Boss.Value);
        if(CurrentState.Value == BossStateEnum.FinalPhase)
            yield return Boss.Value.GetCompo<BossRenderer>().Cracked();
        _isEnd = true;
    }
}

