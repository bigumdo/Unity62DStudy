using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using YUI.Cores;

namespace YUI
{
    public class TutorialUI : ToolkitUI
    {
        private VisualElement _root;

        private Label _sign;
        private VisualElement _labelvisual;
        private List<Label> _labels;
        private Label _label;

        private VisualElement _gauge;

        private Coroutine _gaugeCoroutine;
        private float _currentGaugeValue = 0f;

        private Coroutine _completeEffectCoroutine;
        private VisualElement _completePanel;

        private Coroutine _typingCoroutine;

        public event Action OnCompleteTextTyping;


        protected override void Awake()
        {
            base.Awake();
            UIManager.Instance.AddUI(this);

            if (visualTreeAsset != null)
            {
                _root = visualTreeAsset.CloneTree();
                _root.style.flexGrow = 1;
                _root.style.position = Position.Absolute;
                _root.style.width = new Length(100, LengthUnit.Percent);
                _root.style.height = new Length(100, LengthUnit.Percent);

                _root.pickingMode = PickingMode.Ignore;
            }
            else
            {
                Debug.LogError("Main Asset이 설정되지 않았습니다.");
            }
        }

        public override void Close()
        {

        }

        public override void Open()
        {
            if (_root != null)
            {
                root.Q("tutorial-container").Add(_root);

                _sign = _root.Q<Label>("triangle-sign");
                _labelvisual = _root.Q("top-content");
                _labels = _labelvisual.Query<Label>().ToList();
                _label = _root.Q<Label>("tutorial-label");
                _gauge = _root.Q("tutorial-gauge");
                _completePanel = _root.Q("clear");

                PlayOpenAnimation();
            }
        }

        private void PlayOpenAnimation()
        {
            _labelvisual.RemoveFromClassList("label-disappear");

            foreach (var lbl in _labels)
            {
                lbl.AddToClassList("tutorial-label-appear");
            }
        }

        public void SetTutorialText(string richText, float typingSpeed = 0.05f)
        {
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);

            _typingCoroutine = StartCoroutine(TypeTextWithRichText(richText, typingSpeed));
        }



        private IEnumerator TypeTextWithRichText(string fullText, float typingSpeed)
        {
            List<string> plainChars = new List<string>();
            Stack<string> tagStack = new Stack<string>();

            int i = 0;
            while (i < fullText.Length)
            {
                if (fullText[i] == '<')
                {
                    int tagEnd = fullText.IndexOf('>', i);
                    if (tagEnd == -1) break;

                    string tag = fullText.Substring(i, tagEnd - i + 1);
                    i = tagEnd + 1;

                    if (!tag.StartsWith("</"))
                        tagStack.Push(tag);
                    else if (tagStack.Count > 0)
                        tagStack.Pop();
                }
                else
                {
                    plainChars.Add(fullText[i].ToString());
                    i++;
                }
            }

            for (int j = 1; j <= plainChars.Count; j++)
            {
                int plainIndex = 0;
                i = 0;
                StringBuilder builder = new StringBuilder();
                Stack<string> openTags = new Stack<string>();

                while (i < fullText.Length && plainIndex < j)
                {
                    if (fullText[i] == '<')
                    {
                        int tagEnd = fullText.IndexOf('>', i);
                        if (tagEnd == -1) break;

                        string tag = fullText.Substring(i, tagEnd - i + 1);
                        builder.Append(tag);

                        if (!tag.StartsWith("</"))
                            openTags.Push(tag);
                        else if (openTags.Count > 0)
                            openTags.Pop();

                        i = tagEnd + 1;
                    }
                    else
                    {
                        builder.Append(fullText[i]);
                        i++;
                        plainIndex++;
                    }
                }

                // 닫는 태그 붙이기
                foreach (var tag in openTags)
                {
                    builder.Append("</" + tag.Substring(1));
                }

                _label.text = builder.ToString();
                yield return new WaitForSeconds(typingSpeed);
            }

            _label.text = fullText;
            OnCompleteTextTyping?.Invoke();
        }


        public void SetGauge(float target, float duration = 0.3f)
        {
            // 이전 코루틴이 실행 중이면
            if (_gaugeCoroutine != null)
                StopCoroutine(_gaugeCoroutine);

            _gaugeCoroutine = StartCoroutine(AnimateGauge(target, duration));
        }

        private IEnumerator AnimateGauge(float target, float duration)
        {
            float elapsed = 0f;
            float startValue = _currentGaugeValue;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = EaseOutCubic(t);
                _currentGaugeValue = Mathf.Lerp(startValue, target, easedT);
                _gauge.style.width = new Length(_currentGaugeValue * 100f, LengthUnit.Percent);
                yield return null;
            }

            _currentGaugeValue = target;
            _gauge.style.width = new Length(_currentGaugeValue * 100f, LengthUnit.Percent);
            _gaugeCoroutine = null;
        }

        public void PlayCompleteEffect()
        {
            if (_completeEffectCoroutine != null)
                StopCoroutine(_completeEffectCoroutine);

            _completeEffectCoroutine = StartCoroutine(AnimateCompleteEffect());
        }

        private IEnumerator AnimateCompleteEffect()
        {
            float fadeInDuration = 0.25f;
            float holdTime = 1f;
            float fadeOutDuration = 0.25f;
            float maxAlpha = 0.2f;

            yield return new WaitForSeconds(0.6f);

            // 점점 나타나기 - EaseOut
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeInDuration);
                float eased = EaseOutCubic(t);
                float alpha = Mathf.Lerp(0f, maxAlpha, eased);
                _completePanel.style.backgroundColor = new StyleColor(new Color(0f, 1f, 0f, alpha));
                yield return null;
            }

            yield return new WaitForSeconds(holdTime);

            // 점점 사라지기 - EaseIn
            elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                float eased = EaseInCubic(t);
                float alpha = Mathf.Lerp(maxAlpha, 0f, eased);
                _completePanel.style.backgroundColor = new StyleColor(new Color(0f, 1f, 0f, alpha));
                yield return null;
            }

            _completePanel.style.backgroundColor = new StyleColor(new Color(0f, 1f, 0f, 0f));
            _completeEffectCoroutine = null;
        }

        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3);
        }

        private float EaseInCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return t * t * t;
        }
    }
}
