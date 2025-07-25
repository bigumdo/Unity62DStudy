using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class CounterCondition : TutorialCondition
    {
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);

            BossManager.Instance.OnCounterSuccessEvent += HandleCounterSuccessEvent;
            TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.Counter);
        }

        private void Start()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            BossManager.Instance.OnCounterSuccessEvent -= HandleCounterSuccessEvent;

        }

        private void HandleCounterSuccessEvent()
        {
            _currentCount = 1f;
            _maxCount = 1f;

            UpdateUI();

            StartCoroutine(HandleDashCompletedRoutine());
        }

        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.None);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_CounterSuccess");

            if (dialogDatas == null || dialogDatas.Count == 0)
            {
                _onMet?.Invoke();
                yield break;
            }


            var dialogCanvas = UIManager.Instance.GetUI<DialogCanvas>();
            UIManager.Instance.ShowUI<DialogCanvas>();


            dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => {
                dialogCanvas.StartDialogRoutine(dialogDatas, () => {
                    _onMet?.Invoke();

                    _player.InputReader.SetInput(InputType.Attack, true);
                    TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.Dead);
                });
            });
        }

    }
}
