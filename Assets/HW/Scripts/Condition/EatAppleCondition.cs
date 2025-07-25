using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using YUI.Agents.players;
using YUI.Apples;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class EatAppleCondition : TutorialCondition
    {
        [SerializeField]
        private AppleSpawner _appleSpawner;
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 1;
            _currentCount = 0;

            player.OnAppleEat += AppleEat;
            _appleSpawner.enabled = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            _player.OnAppleEat -= AppleEat;

            _appleSpawner.enabled = false;
        }

        private void AppleEat()
        {
            _currentCount++;

            UpdateUI();

            if (_currentCount >= _maxCount)
            {
                StartCoroutine(HandleDashCompletedRoutine());
            }
        }

        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_Apple2");

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