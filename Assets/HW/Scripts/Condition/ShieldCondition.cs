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
    public class ShieldCondition : TutorialCondition
    {
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 1;
            _currentCount = 0;
            _player.InputReader.MainSkillEvent += OnMainSkillEvent;
            _player.InputReader.SetInput(InputType.Skill, true);
        }

        private void OnMainSkillEvent()
        {
            _currentCount++;

            UpdateUI();

            if (_currentCount >= _maxCount)
            {
                StartCoroutine(HandleDashCompletedRoutine());
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _player.InputReader.MainSkillEvent -= OnMainSkillEvent;
            _player.InputReader.SetInput(InputType.Skill, false);

        }

        private IEnumerator HandleDashCompletedRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            TutorialBossManger.Instance.BossChangeState(TutorialBossStateEnum.None);

            List<DialogData> dialogDatas = DialogManager.Instance.GetLines("Tutorial_Shield2");

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
