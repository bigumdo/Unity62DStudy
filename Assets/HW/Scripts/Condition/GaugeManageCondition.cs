using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Apples;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class GaugeManageCondition : TutorialCondition
    {
        [SerializeField]
        private int _maxCnt;

        [SerializeField]
        private AppleSpawner _spawner;
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = _maxCnt;
            _currentCount = 0;

            player.InputReader.UIAttackEvent += OnPlayerAttack;
            player.InputReader.SetInput(InputType.Attack, true);
            _spawner.enabled = true;

        }

        private void OnPlayerAttack()
        {
            var overload = _player.GetCompo<PlayerOverload>();
            if (overload != null && overload.Overload >= 50f && overload.Overload < 85f)
            {
                _currentCount++;
                UpdateUI();

                if (_currentCount >= _maxCount)
                {
                    StartCoroutine(HandleDashCompletedRoutine());
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _player.InputReader.UIAttackEvent -= OnPlayerAttack;
        }

        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_Guage");

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
                    TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.Attack1);
                    _spawner.enabled = false;
                });
            });
        }

    }
}
