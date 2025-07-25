using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Rooms;

namespace YUI
{
    public class PauseUI : ToolkitUI
    {
        private VisualElement _root;

        private VisualElement _frame;

        private Label _continue;
        private Label _setting;
        private Label _exit;

        [SerializeField] private InputReader _inputReader;

        protected override void Awake()
        {
            base.Awake();
            UIManager.Instance.AddUI(this);

            if (visualTreeAsset != null)
            {
                _root = visualTreeAsset.CloneTree();
                //_root.style.flexGrow = 1;
                _root.style.position = Position.Absolute;

                _root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                _root.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
                _root.pickingMode = PickingMode.Ignore;

            }
            else
                Debug.LogError("Pause Asset이 설정되지 않았습니다.");
        }

        public override void Close()
        {
            _frame.RemoveFromClassList("appear");
        }

        public override void Open()
        {
            if (_root != null)
            {
                root.Q("pause-container").Add(_root);
                _frame = _root.Q("frame");
                _continue = _root.Q<Label>("continue-label");
                //_setting = _root.Q<Label>("setting-label");
                _exit = _root.Q<Label>("exit-label");

                _continue?.RegisterCallback<ClickEvent>(evt =>
                {
                    Close();
                    _inputReader.SetAllInput(true);
                    Time.timeScale = 1;
                });

                //_setting?.RegisterCallback<ClickEvent>(evt =>
                //{

                //});

                _exit?.RegisterCallback<ClickEvent>(evt =>
                {
                    Application.Quit();
                });
            }

            Close();
        }

        private void PlayOpenAnimation()
        {
            if(_frame.ClassListContains("appear"))
            {
                _frame.RemoveFromClassList("appear");
                _inputReader.SetAllInput(true);
                Time.timeScale = 1;
            }
            else
            {
                _frame.AddToClassList("appear");
                _inputReader.SetAllInput(false);
                Time.timeScale = 0;
            }
        }

        private void OnEnable()
        {
            Open();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                PlayOpenAnimation();
            }
        }
    }
}
