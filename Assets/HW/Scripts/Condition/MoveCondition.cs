using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class MoveCondition : TutorialCondition
    {
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 5;
            _currentCount = 0;
            inputReader.MovementEvent += HandleMovement;
        }

        public override void Dispose()
        {
            base.Dispose();
            inputReader.MovementEvent -= HandleMovement;
        }

        private void HandleMovement(PlayerMoveDir dir)
        {
            _currentCount++;

            UpdateUI();

            if (_currentCount >= _maxCount)
            {
                _onMet?.Invoke();
                _currentCount = 0;
            }
        }
    }
}
