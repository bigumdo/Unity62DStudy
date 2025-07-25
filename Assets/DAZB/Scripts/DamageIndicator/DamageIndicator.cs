using System.Collections;
using TMPro;
using UnityEngine;
using YUI.ObjPooling;

namespace YUI
{
    public class DamageIndicator : PoolableMono
    {
        [SerializeField] private float xVelocityRange;
        [SerializeField] private float yVelocity;
        [SerializeField] private float lifetime;
        [SerializeField] private Color textColor;
        [SerializeField] private float fadeTime;
        [SerializeField] private Color criticalTextColor;
        private Rigidbody2D rigidCompo;
        private TextMeshProUGUI text;
        private Color DefaultTextColor;

        private void Awake()
        {
            rigidCompo = GetComponent<Rigidbody2D>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            text.color = textColor;

            DefaultTextColor = text.color;
        }

        public override void ResetItem()
        {
            text.color = DefaultTextColor;

            StartCoroutine(Routine());
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetText(string damageText)
        {
            text.text = damageText;
        }

        public void SetTextColor(Color color)
        {
            text.color = color;
            textColor = color;
        }

        public void StartRoutine(bool isCritical = false)
        {
            StartCoroutine(Routine(isCritical));
        }

        private IEnumerator Routine(bool isCritical = false)
        {
            text.text += isCritical ? "!" : "";

            if (isCritical)
            {
                text.color = criticalTextColor;
                textColor = criticalTextColor;
            }

            text.rectTransform.localScale *= isCritical ? 1.2f : 1f;
            rigidCompo.linearVelocity = new Vector2(Random.Range(-xVelocityRange, xVelocityRange), yVelocity);

            yield return new WaitForSeconds(lifetime);

            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                text.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
                yield return null;
            }

            rigidCompo.linearVelocity = Vector2.zero;
            PoolingManager.Instance.Push(this);
        }
    }
}
