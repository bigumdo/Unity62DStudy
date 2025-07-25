using System;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class SlowCondition : TutorialCondition
    {
        private bool _isTracking = false;

        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 5;
            _currentCount = 0f;
            inputReader.SetInput(InputType.SlowMode, true);
            _isTracking = true;
            _currentCount = 0f;
        }

        public override void Dispose()
        {
            base.Dispose();
            _isTracking = false;
        }

        private void Update()
        {
            if (!_isTracking) return;

            if (inputReader.SlowMode)
            {
                _currentCount += Time.deltaTime;

                UpdateUI();

                if (_currentCount >= _maxCount)
                {
                    _isTracking = false;
                    _onMet?.Invoke();
                }
            }
        }
    }
}
