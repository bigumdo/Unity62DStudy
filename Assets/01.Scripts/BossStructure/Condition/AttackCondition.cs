using System;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class AttackCondition : TutorialCondition
    {
        [SerializeField] public Boss boss;
        public override void Initialize(Action onMet, Player player)
        {
            base.Initialize(onMet, player);
            _maxCount = 4;
            _currentCount = 0;
            inputReader.SetInput(InputType.Attack, true);
            inputReader.UIAttackEvent += HandleAttack;
        }

        public override void Dispose()
        {
            base.Dispose();
            inputReader.UIAttackEvent -= HandleAttack;
            inputReader.SetInput(InputType.Attack, false);
        }

        private void HandleAttack()
        {
            _currentCount++;

            UpdateUI();

            if (_currentCount >= _maxCount)
            {
                _onMet?.Invoke();
            }
        }


    }
}
