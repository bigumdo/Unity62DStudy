using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class AvoidAttackCondition : TutorialCondition
    {
        private bool _isTracking = false;
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 5;
            _currentCount = 0;
            _isTracking = true;

        }
        public override void Dispose()
        {
            base.Dispose();
            _isTracking = false;
        }


        private void Update()
        {
            if (!_isTracking) return;

            _currentCount += Time.deltaTime;

            UpdateUI();

            if (_currentCount >= _maxCount)
            {
                _isTracking = false;
                StartCoroutine(HandleDashCompletedRoutine());
            }
        }

        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.None);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_Avoid2");

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
                    TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.Attack2);
                });
            });
        }
    }
}