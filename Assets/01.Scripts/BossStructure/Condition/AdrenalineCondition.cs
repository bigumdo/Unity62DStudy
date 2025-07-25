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
    public class AdrenalineCondition : TutorialCondition
    {
        [SerializeField]
        private int _maxCnt;

        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = _maxCnt;
            _currentCount = 0;
            player.InputReader.SetInput(InputType.Attack, true);
            player.InputReader.UIAttackEvent += OnPlayerAttack;

        }
        public override void Dispose()
        {
            base.Dispose();
            _player.InputReader.UIAttackEvent -= OnPlayerAttack;
        }

        private void OnPlayerAttack()
        {
            var overload = _player.GetCompo<PlayerOverload>();
            if (overload != null && overload.Overload == 60f)
            {
                _currentCount++;
                UpdateUI();

                if (_currentCount >= _maxCount)
                {
                    _player.InputReader.SetInput(InputType.Attack, false);
                    StartCoroutine(HandleDashCompletedRoutine());
                }
            }
        }


        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_AdAttack");

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
                    
                });
            });
        }
    }
}
