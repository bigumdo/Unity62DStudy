using System.Collections;
using UnityEngine;
using YUI.Cores;

namespace YUI.UI
{
    public class InteractPopup : LegacyBaseUI
    {
        [SerializeField] private RectTransform rootPanel;
        [SerializeField] private float startMinusYOffset = 50f;
        private CanvasGroup canvasGroup;
        private Vector3 initialPosition;
        private bool isOpen = false;

        private void Awake()
        {
            UIManager.Instance.AddUI(this);

            canvasGroup = rootPanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            initialPosition = rootPanel.anchoredPosition;

            rootPanel.anchoredPosition = new Vector2(initialPosition.x, initialPosition.y - startMinusYOffset);
        }

        public override void Open()
        {
            if (isOpen) return;

            rootPanel.gameObject.SetActive(true);
            isOpen = true;

            StartCoroutine(FadeInRoutine());
        }

        public override void Close()
        {
            isOpen = false;

            StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            float elapsedTime = 0f;
            float duration = 0.25f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
                rootPanel.anchoredPosition = new Vector2(initialPosition.x, Mathf.Lerp(initialPosition.y - startMinusYOffset, initialPosition.y, elapsedTime / duration));
                yield return null;
            }
        }

        private IEnumerator FadeOutRoutine()
        {
            float elapsedTime = 0f;
            float duration = 0.25f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
                rootPanel.anchoredPosition = new Vector2(initialPosition.x, Mathf.Lerp(initialPosition.y, initialPosition.y - startMinusYOffset, elapsedTime / duration));
                yield return null;
            }

            rootPanel.gameObject.SetActive(false);
        }
    }
}
