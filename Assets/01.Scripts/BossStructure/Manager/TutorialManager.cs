using UnityEngine;
using YUI;
using YUI.Agents.players;
using YUI.Cores;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using YUI.Apples;
using YUI.UI.DialogSystem;
using System.Collections.Generic;
using YUI.Dialogs;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialStepData startStep;

    private TutorialStepData currentStep;
    private TutorialCondition currentCondition;

    [SerializeField]
    private Light2D _globalLight;

    [SerializeField]
    private AppleSpawner _spawner;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private InputReader _inputReader; // For Input Setting

    private TutorialUI _tutorialUI;

    [SerializeField]
    private bool _isTutorial;

    private bool _isTextTyping;

    private void Awake()
    {
        if (!_isTutorial) return;

        _globalLight.intensity = 0;

        _spawner.enabled = false;

        _player.AttackStat.BaseValue = 0;
    }

    private void Start()
    {
        if (!_isTutorial) return;

        SoundManager.Instance.PlaySound("SFX_TutorialStart");

        _inputReader.SetAllInput(false);
        _tutorialUI = UIManager.Instance.GetUI<TutorialUI>();
        _tutorialUI.OnCompleteTextTyping += EndTextTypeRoutine;

        PlayerManager.Instance.StopPlayerInput();

        StartCoroutine(DialogRoutine());
        
    }

    private IEnumerator DialogRoutine()
    {
        yield return new WaitForSeconds(2);

        List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_Start");

        var dialogCanvas = UIManager.Instance.GetUI<DialogCanvas>();

        UIManager.Instance.ShowUI<DialogCanvas>();
        dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, () => {
            PlayerManager.Instance.StartPlayerInput();
            StartStep(startStep);
        }
        ));
    }

    private void StartStep(TutorialStepData step)
    {
        if (step == null) return;
        _isTextTyping = true;

        currentStep = step;

        //step.onEnter?.Invoke();

        currentCondition = CreateCondition(step.conditionType);

        //UI ǥ��
        _tutorialUI.Open();
        _tutorialUI.SetTutorialText(currentStep.tooltipText);

        StartCoroutine(WaitRoutine());
    }

    private IEnumerator WaitRoutine()
    {
        yield return new WaitUntil(() => !_isTextTyping);
        currentCondition?.Initialize(CompleteStep, PlayerManager.Instance.Player);
    }

    private void EndTextTypeRoutine()
    {
        _isTextTyping = false;
    }

    private void CompleteStep()
    {
        //currentStep.onComplete?.Invoke();
        currentCondition?.Dispose();

        StartCoroutine(NextStepCorutine());
    }

    private IEnumerator NextStepCorutine()
    {
        _tutorialUI.PlayCompleteEffect();
        yield return new WaitForSeconds(3f);

        if (currentStep.nextStep == null)
        {
            UIManager.Instance.Fade("Lobby");
        }
        else
        {
            var nextStep = currentStep.nextStep;

            if (nextStep.hasDialog)
            {
                List<DialogData> dialogDataList = DialogManager.Instance.GetLines(nextStep.dialogKey);
                var dialogCanvas = UIManager.Instance.GetUI<DialogCanvas>();

                UIManager.Instance.ShowUI<DialogCanvas>();
                dialogCanvas.StartDialogOpenRoutine(dialogDataList, () => dialogCanvas.StartDialogRoutine(dialogDataList, () =>
                {
                    StartStep(nextStep);
                    _tutorialUI.SetGauge(0);
                }));
            }
            else
            {
                StartStep(nextStep);
                _tutorialUI.SetGauge(0);
            }
        }
    }


    private void OnDestroy()
    {
        currentCondition?.Dispose();
    }

    private TutorialCondition CreateCondition(TutorialConditionType type)
    {
        return type switch
        {
            TutorialConditionType.Start => GetCondition<StartCondition>(type),
            TutorialConditionType.Move => GetCondition<MoveCondition>(type),
            TutorialConditionType.Dash => GetCondition<DashCondition>(type),
            TutorialConditionType.Attack => GetCondition<AttackCondition>(type),
            TutorialConditionType.SlowMode => GetCondition<SlowCondition>(type),
            TutorialConditionType.OverloadAbove50 => GetCondition<OverloadAbove50Condition>(type),
            TutorialConditionType.Overload100 => GetCondition<Overload100Condition>(type),
            TutorialConditionType.EatApple => GetCondition<EatAppleCondition>(type),
            TutorialConditionType.CounterReady => GetCondition<CounterCondition>(type),
            TutorialConditionType.Custom => GetCondition<CustomCondition>(type),
            TutorialConditionType.Adrenaline => GetCondition<AdrenalineCondition>(type),
            TutorialConditionType.GaugeManage => GetCondition<GaugeManageCondition>(type),
            TutorialConditionType.Shield => GetCondition<ShieldCondition>(type),
            TutorialConditionType.AvoidAttack => GetCondition<AvoidAttackCondition>(type),
            _ => null
        };
    }

    private TutorialCondition GetCondition<T>(TutorialConditionType type) where T : TutorialCondition
    {
        var comp = GetComponent<T>();

        return comp;
    }
}
