using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Rendering.Universal;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class DashCondition : TutorialCondition
    {
        [SerializeField]
        private Light2D _glbalLight;
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 3;
            _currentCount = 0;
            inputReader.SetInput(InputType.Dash, true);
            inputReader.DashEvent += HandleDash;
        }

        public override void Dispose()
        {
            base.Dispose();
            inputReader.DashEvent -= HandleDash;
        }

        private void HandleDash(bool value)
        {
            _currentCount++;

            UpdateUI();

            if (_currentCount >= _maxCount) 
            {
                _glbalLight.intensity = 1;
                StartCoroutine(HandleDashCompletedRoutine());
            }

           
        }

        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_AfterLight");

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
