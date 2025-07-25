using System;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public abstract class TutorialCondition : MonoBehaviour, ITutorialCondition
    {
        protected Action _onMet;
        protected Player _player;
        protected InputReader inputReader;

        [Range(0f, 1f)]
        protected float Progress;

        protected float _currentCount;
        protected float _maxCount;


        public virtual void Initialize(Action onMet, Player player)
        {
            _onMet = onMet;
            _player = player;
            inputReader = _player.InputReader;
            Progress = 0f;
            
        }
        public virtual void Dispose()
        {
            Progress = 0;
        }

        protected void UpdateUI()
        {
            Progress = (_maxCount > 0) ? Mathf.Clamp01((float)_currentCount / _maxCount) : 0f;
            UIManager.Instance?.UpdateProgressBar(Progress);
        }

        protected void StartDialog(Action onDialogEnd, string key)
        {
            List<DialogData> dialogDatas = DialogManager.Instance.GetLines(key);

            var dialogCanvas = UIManager.Instance.GetUI<DialogCanvas>();

            UIManager.Instance.ShowUI<DialogCanvas>();
            dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, onDialogEnd));
        }

    }
}
