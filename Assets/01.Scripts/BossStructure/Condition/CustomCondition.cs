using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YUI.Agents.players;

namespace YUI
{
    public class CustomCondition : TutorialCondition
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
                _onMet?.Invoke();
                StartCoroutine(LoadScene());
            }
        }

        private IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(2);

            SceneManager.LoadScene("Lobby");
        }
    }
}
