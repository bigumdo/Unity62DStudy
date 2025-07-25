using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using YUI.PatternModules;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
using YUI.Cores;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "TutorialBossRandomPatternAction", story: "[Boss] [CurrentState]", category: "Action", id: "e814e51be6c3f4d3c7d32532b0a7ea92")]
public partial class TutorialBossRandomPatternAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<TutorialBossStateEnum> CurrentState;

    private bool _isEnd;
    private PatternSO _pattern;

    protected override Status OnStart()
    {
        _pattern = TutorialBossManger.Instance.GetRandomPattern(CurrentState.Value);
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

    protected override void OnEnd()
    {
    }

    private IEnumerator PatternExecute()
    {
        yield return new WaitUntil(() => !GameManager.Instance.IsGmaeStop);
        yield return _pattern.Execute(Boss.Value);
        _isEnd = true;
    }
}

