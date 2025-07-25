using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YUI.Cores;

namespace YUI
{
    public class TooltipUI : ToolkitUI
    {
        private VisualElement _root;
        private VisualElement _window;
        private List<Label> _labels;

        protected override void Awake()
        {
            base.Awake();
            UIManager.Instance.AddUI(this);

            if (visualTreeAsset != null)
            {
                _root = visualTreeAsset.CloneTree();
                _root.style.flexGrow = 1;
            }
            else
                Debug.LogError("Main Asset이 설정되지 않았습니다.");
        }
        public override void Close()
        {

        }

        public override void Open()
        {
            if (_root != null)
            {
                root.Q("container").Add(_root);

                _window = _root.Q("window-frame");

                _labels = _root.Query<Label>().ToList();
            }
        }

        private void OnEnable()
        {
            Open();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                StartCoroutine(PlayOpenAnimation());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(PlayeCloseAnim());
            }



        }

        private IEnumerator PlayOpenAnimation()
        {
            _window.AddToClassList("window-width-appear");

            yield return new WaitForSeconds(0.4f);

            _window.AddToClassList("window-height-appear");

            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < _labels.Count; i++)
            {
                _labels[i].AddToClassList("text-appear");
            }
        }

        private IEnumerator PlayeCloseAnim()
        {

            for (int i = 0; i < _labels.Count; i++)
            {
                _labels[i].RemoveFromClassList("text-appear");
            }

            yield return new WaitForSeconds(0.4f);

            _window.RemoveFromClassList("window-height-appear");

            yield return new WaitForSeconds(0.4f);

            _window.RemoveFromClassList("window-width-appear");



        }
    }
}
