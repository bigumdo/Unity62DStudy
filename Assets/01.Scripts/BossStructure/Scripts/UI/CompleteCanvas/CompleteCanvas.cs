using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Cores;

namespace YUI.UI.CompleteUI
{
    public class CompleteCanvas : LegacyBaseUI
    {
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private CompleteCanvasButton choiceButton_1;
        [SerializeField] private CompleteCanvasButton choiceButton_2;
        [SerializeField] private bool isPlayerDead = false;
        int currentChoiceIndex = 0;

        private void Awake()
        {
            UIManager.Instance.AddUI(this);

            rootPanel.SetActive(false);
        }

        private void Update() {
            if (isPlayerDead == false) return;

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                currentChoiceIndex = 0;
            }
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                currentChoiceIndex = 1;
            }

            if (currentChoiceIndex == 0)
            {
                choiceButton_1.Select();
                choiceButton_2.Deselect();
            }
            else if (currentChoiceIndex == 1)
            {
                choiceButton_2.Select();
                choiceButton_1.Deselect();
            }
            
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if (currentChoiceIndex == 0)
                {
                    GameManager.Instance.LoadScene("Lobby", true);
                }
                else if (currentChoiceIndex == 1)
                {
                    GameManager.Instance.Quit();
                }
            }
        }

        public override void Open()
        {
            isPlayerDead = true;
            rootPanel.SetActive(true);
        }

        public override void Close()
        {
            isPlayerDead = false;
            rootPanel.SetActive(false);
        }
    }
}
