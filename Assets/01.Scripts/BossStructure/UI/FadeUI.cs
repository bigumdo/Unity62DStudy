using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using YUI.Cores;

namespace YUI
{
    public class FadeUI : ToolkitUI
    {
        private VisualElement _root;

        private VisualElement _fade;
        protected override void Awake()
        {
            base.Awake();

            UIManager.Instance.AddUI(this);

            if (visualTreeAsset != null)
            {
                _root = visualTreeAsset.CloneTree();
                _root.style.flexGrow = 1;
                _root.style.position = Position.Absolute;
            }
            else
                Debug.LogError("Main Asset�� �������� �ʾҽ��ϴ�.");

        }
        public override void Close()
        {

        }

        public override void Open()
        {
            if (_root != null)
            {
                root.Q("fade-container").Add(_root);

                _fade = _root.Q("fade");
            }
        }



        public void StartFade(string sceneName)
        {
            StartCoroutine(StartFadeCoroutine(sceneName));
        }

        private IEnumerator StartFadeCoroutine(string sceneName)
        {
            if (sceneName == "")
            {
                yield return null;
                Debug.LogError("�̵��� �� �̸��� �Լ����� ���� �� �ּ���.");
            }
            else
            {
                yield return null;

                _fade.AddToClassList("fade");

                yield return new WaitForSeconds(1f);

                SceneManager.LoadScene(sceneName);
            }


        }
    }
}
