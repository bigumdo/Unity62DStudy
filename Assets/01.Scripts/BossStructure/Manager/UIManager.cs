using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace YUI.Cores
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private Dictionary<Type, IUI> _uiDict;

        protected override void OnCreateInstance()
        {
            base.OnCreateInstance();
            _uiDict = new Dictionary<Type, IUI>();
        }

        public T GetUI<T>() where T : class, IUI
        {
            if (_uiDict.TryGetValue(typeof(T), out IUI ui))
            {
                return ui as T;
            }

            Debug.LogWarning($"UIManager: {typeof(T).Name} UI가 등록되지 않았습니다.");

            return null;
        }

        // UI 등록
        public void AddUI<T>(T ui) where T : IUI
        {
            Type type = typeof(T);

            if (_uiDict.ContainsKey(type)) return;

            _uiDict.Add(type, ui);
        }

        // 이 함수들은 다른 스크립트에서 열거나 닫아야 할때 사용합니다.
        public void ShowUI<T>() where T : class, IUI => GetUI<T>()?.Open();
        public void HideUI<T>() where T : class, IUI => GetUI<T>()?.Close();

        // 모든 UI 닫기
        public void HideAllUI()
        {
            foreach (var ui in _uiDict.Values)
            {
                ui.Close();
            }
        }

        // Only Toolkit
        public VisualTreeAsset GetVTAsset<T>() where T : ToolkitUI
        {
            var toolkitUI = GetUI<T>();
            return toolkitUI?.visualTreeAsset;
        }

        public void SetBossGaugeVisibility(bool istrue) => GetUI<CombatUI>().SetBossGaugeVisibility(istrue);
        public void UpdateBossHp() => GetUI<CombatUI>()?.UpdateBossHpUI();
        public void Fade(string sceneName = "") => GetUI<FadeUI>()?.StartFade(sceneName);
        public void UpdateProgressBar(float progress, float duration = 0.3f) => GetUI<TutorialUI>()?.SetGauge(progress, duration);

    }
}
