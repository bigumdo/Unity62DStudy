using System;
using Unity.Behavior;
using UnityEngine;
using YUI.Agents.Bosses;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections.Generic;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;
using System.Collections;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossPhaseChangeDialog", story: "[Boss] PhaseChange Dialog [CurrentPhase]", category: "Action", id: "e096a73f6813c893abbe4c986486eec6")]
public partial class BossPhaseChangeDialogAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<BossStateEnum> CurrentPhase;

    private bool _isFinished;

    protected override Status OnStart()
    {
        _isFinished = false;
        Boss.Value.StartCoroutine(Dialog());
        PlayerManager.Instance.Player.InputReader.Enable(false);
        PlayerManager.Instance.Player.InputReader.SetSlowMode(true);
        return Status.Running;
    }

    private IEnumerator Dialog()
    {
        List<DialogData> dialogDatas;
        List<string> dialogKeyList = PatternManager.Instance.GetDialog(CurrentPhase.Value);

        for (int i = 0; i < dialogKeyList.Count; ++i)
        {
            dialogDatas = DialogManager.Instance.GetLines(dialogKeyList[i]);

            UIManager.Instance.ShowUI<DialogCanvas>();
            UIManager.Instance.GetUI<DialogCanvas>().StartDialogOpenRoutine(dialogDatas, () =>
            {
                UIManager.Instance.GetUI<DialogCanvas>().StartDialogRoutine(dialogDatas);
            });

            yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>().isFinished);
        }

        _isFinished = true;
    }

    protected override Status OnUpdate()
    {
        if (_isFinished)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        PlayerManager.Instance.Player.InputReader.Enable(true);
        PlayerManager.Instance.Player.InputReader.SetSlowMode(false);
    }

}

