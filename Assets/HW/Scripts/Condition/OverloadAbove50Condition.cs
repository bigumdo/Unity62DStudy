using System;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class OverloadAbove50Condition : TutorialCondition
    {
        private PlayerOverload _playerOverload;

        private bool _isTracking = false;
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _playerOverload = _player.GetCompo<PlayerOverload>(true);
            _maxCount = _playerOverload.Overload;
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

            _currentCount = _playerOverload.Overload;

            UpdateUI();

            if (_playerOverload.Overload >= 60)
            {
                _onMet?.Invoke();
            }
        }

    }
}
