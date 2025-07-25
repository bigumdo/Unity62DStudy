using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Cores;


namespace YUI.UI
{
    public class PlayerDeadCanvas : LegacyBaseUI
    {
        [SerializeField] private GameObject rootPanel;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI subTitle;
        [SerializeField] private RectTransform line;
        [SerializeField] private List<DeadCanvasButton> buttonList;

        private bool isRunning = false;
        private int currentSelectedButtonIndex = 0;

        private float defaultLineWidth;
        private string defaultTitleText;
        private string defaultSubTitleText;

        private bool isPlayerDead = false;

        private void Awake()
        {
            UIManager.Instance.AddUI(this);

            defaultLineWidth = line.rect.width;
            defaultTitleText = title.text;
            defaultSubTitleText = subTitle.text;

            buttonList.ForEach(x => x.Init());
            buttonList.ForEach(x => x.gameObject.SetActive(false));

            title.text = "";
            subTitle.text = "";
            line.sizeDelta = new Vector2(0, line.sizeDelta.y);

            rootPanel.SetActive(false);
        }

        private void Update()
        {
            if (!isRunning && isPlayerDead)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                {
                    currentSelectedButtonIndex--;
                    currentSelectedButtonIndex = Mathf.Clamp(currentSelectedButtonIndex, 0, buttonList.Count - 1);
                }
                else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                {
                    currentSelectedButtonIndex++;
                    currentSelectedButtonIndex = Mathf.Clamp(currentSelectedButtonIndex, 0, buttonList.Count - 1);
                }

                if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
                {
                    buttonList[currentSelectedButtonIndex].Execute();
                }

                for (int i = 0; i < buttonList.Count; ++i)
                {
                    if (i == currentSelectedButtonIndex)
                    {
                        buttonList[i].text.text = ">  " + buttonList[i].defaultText.Trim() + "  <";
                    }
                    else
                    {
                        buttonList[i].text.text = buttonList[i].defaultText;
                    }
                }
            }
        }

        public void StartDeadRoutine()
        {
            StartCoroutine(DeadRoutine());
        }

        private IEnumerator DeadRoutine()
        {
            isRunning = true;
            isPlayerDead = true;

            yield return new WaitForSeconds(1f);

            float time = 1f;
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                line.sizeDelta = new Vector2(Mathf.Lerp(0, defaultLineWidth, elapsedTime / time), line.sizeDelta.y);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < defaultTitleText.Length; ++i)
            {
                title.text += defaultTitleText[i];

                yield return new WaitForSeconds(0.1f);
            }

            for (int i = 0; i < defaultSubTitleText.Length; ++i)
            {
                subTitle.text += defaultSubTitleText[i];

                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(0.5f);

            elapsedTime = 0;
            time = 0.2f;

            for (int i = 0; i < buttonList.Count; ++i)
            {
                buttonList[i].gameObject.SetActive(true);
            }

            while (elapsedTime < time)
            {
                for (int i = 0; i < buttonList.Count; ++i)
                {
                    buttonList[i].text.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, elapsedTime / time));
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            isRunning = false;
        }

        public override void Open()
        {
            rootPanel.SetActive(true);
        }

        public override void Close()
        {
            rootPanel.SetActive(false);
        }
    }
}
