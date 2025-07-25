using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;

namespace YUI.UI.DialogSystem
{
    public class DialogCanvas : LegacyBaseUI
    {
        [SerializeField] private Image rootPanel;
        [SerializeField] private RectTransform dialogPanel;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private TextMeshProUGUI speecherNameText;
        [SerializeField] private Image twinkleObject;

        [Space, Space]

        [SerializeField] private DialogChoiceButton choiceButton_1;
        [SerializeField] private DialogChoiceButton choiceButton_2;

        private Vector2 defaultPosition;
        private float defaultAlpha;
        
        private bool isShaking = false;
        
        private float twinkleElapsedTime = 0f;
        private float blinkInterval = 0.35f;
        private float twinkleHoldTime = 0.2f;
        private float twinkleHoldElapsed = 0f;
        private bool isTwinkling = false;

        public bool isFinished = false;

        private void Awake()
        {
            UIManager.Instance.AddUI(this);

            defaultAlpha = rootPanel.color.a;
            defaultPosition = dialogPanel.anchoredPosition;

            //rootPanel.color = new Color(rootPanel.color.r, rootPanel.color.g, rootPanel.color.b, 0);
            dialogText.text = "";
            speecherNameText.text = "";

            //dialogPanel.anchoredPosition = new Vector2(dialogPanel.anchoredPosition.x, -800f);

            rootPanel.gameObject.SetActive(false);

            choiceButton_1.gameObject.SetActive(false);
            choiceButton_2.gameObject.SetActive(false);
        }

        private void Start() {
            StartCoroutine(TwinkleRoutine());
        }

        private void Update()
        {
            if (isShaking)
            {
                float shakeAmountX = 5 * Mathf.Sin(Time.time * 50f);
                float shakeAmountY = 5 * Mathf.Cos(Time.time * 40f);
                dialogPanel.anchoredPosition = defaultPosition + new Vector2(shakeAmountX, shakeAmountY);
            }
        }

        public void SetSpeecherName(string name)
        {
            speecherNameText.text = "";

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] != '"')
                {
                    speecherNameText.text += name[i];
                }
            }
        }

        public void StartDialogOpenRoutine(List<DialogData> dialogDataList, Action callback = null)
        {
            isFinished = false;

            PlayerManager.Instance.StopPlayerInput();

            SetSpeecherName(dialogDataList[0].Speecher);

            StartCoroutine(DialogOpenRoutine(callback));
        }

        private IEnumerator TwinkleRoutine()
        {
            while (true)
            {
                if (isTwinkling)
                {
                    float t = Mathf.PingPong(twinkleElapsedTime, blinkInterval) / blinkInterval;
                    float alpha = Mathf.Lerp(0f, 1f, t);

                    if (Mathf.Approximately(alpha, 1f) || alpha > 0.99f)
                    {
                        twinkleHoldElapsed += Time.deltaTime;
                        alpha = 1f;
                        if (twinkleHoldElapsed >= twinkleHoldTime)
                        {
                            twinkleElapsedTime += Time.deltaTime;
                            twinkleHoldElapsed = 0f;
                        }
                    }
                    else
                    {
                        twinkleElapsedTime += Time.deltaTime;
                        twinkleHoldElapsed = 0f;
                    }

                    Color c = twinkleObject.color;
                    c.a = alpha;
                    twinkleObject.color = c;
                    twinkleObject.gameObject.SetActive(true);
                }
                else
                {
                    twinkleObject.gameObject.SetActive(false);
                    twinkleElapsedTime = 0f;
                    twinkleHoldElapsed = 0f;
                }

                yield return null;
            }
        }

        private IEnumerator DialogOpenRoutine(Action callback = null)
        {
            float elapsedTime = 0f;
            float time = 0.2f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / time);

                rootPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, t);
                yield return null;
            } 
            
            rootPanel.GetComponent<CanvasGroup>().alpha = 1;

            callback?.Invoke();

            yield return null;
        }

        public void StartDialogCloseRoutine(Action callback = null, bool isStartPlayerInput = true)
        {
            choiceButton_1.gameObject.SetActive(false);
            choiceButton_2.gameObject.SetActive(false);

            StartCoroutine(DialogCloseRoutine(callback, isStartPlayerInput));
        }

        private IEnumerator DialogCloseRoutine(Action callback, bool isStartPlayerInput = true)
        {
            float elapsedTime = 0f;
            float time = 0.2f;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / time);

                rootPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, t);
                yield return null;
            }

            isShaking = false;

            callback?.Invoke();

            Close();

            isFinished = true;

            dialogText.text = "";
            speecherNameText.text = "";

            if (isStartPlayerInput)
            {
                PlayerManager.Instance.StartPlayerInput();
            }

            yield return null;
        }

        public void StartDialogRoutine(List<DialogData> dialogDataList, Action callback = null, bool isStartPlayerInput = true)
        {
            StartCoroutine(DialogRoutine(dialogDataList, callback, isStartPlayerInput));
        }


        private IEnumerator DialogRoutine(List<DialogData> dialogDataList, Action callback = null, bool isStartPlayerInput = true)
        {
            int currentIndex = 0;
            StringBuilder sb = new StringBuilder();
            bool skip = false;

            foreach (DialogData dialogData in dialogDataList)
            {
                isShaking = false;
                isTwinkling = false;
                dialogPanel.anchoredPosition = defaultPosition;

                SetSpeecherName(dialogData.Speecher);

                yield return new WaitForSecondsRealtime(0.05f);

                if (skip == false && (dialogData.LineType == 1 || dialogData.LineType == 2))
                {
                    yield return new WaitForSecondsRealtime(dialogData.LineDelay);
                }
                else
                {
                    skip = false;
                }

                switch (dialogData.LineType)
                {
                    case 0: sb.Clear(); break;
                    case 1: sb.Append(" "); break;
                    case 2: sb.Append("\r\n"); break;
                }

                if (dialogData.EffectOnPlay == 1)
                {
                    isShaking = true;
                    SoundManager.Instance.PlaySound(dialogData.PlayEffectSound);
                }

                string colorTagStart = $"<color=#{ColorUtility.ToHtmlStringRGB(dialogData.TextColor)}>";
                string colorTagEnd = "</color>";
                sb.Append(colorTagStart);

                for (int i = 0; i < dialogData.Line.Length; i++)
                {
                    float elapsedTime = 0f;

                    if (skip)
                    {
                        for (int j = i; j < dialogData.Line.Length; j++)
                        {
                            if (dialogData.Line[j] != '"')
                                sb.Append(dialogData.Line[j]);
                        }
                        sb.Append(colorTagEnd);
                        dialogText.text = sb.ToString();
                        break;
                    }

                    if (dialogData.Line[i] == '"') continue;

                    sb.Append(dialogData.Line[i]);
                    dialogText.text = sb.ToString() + colorTagEnd;

                    SoundManager.Instance.PlaySound(dialogData.SpeecherVoice);

                    while (elapsedTime < dialogData.CharDelay)
                    {
                        elapsedTime += Time.deltaTime;

                        if (Keyboard.current.spaceKey.wasPressedThisFrame)
                        {
                            for (int j = i + 1; j < dialogData.Line.Length; j++)
                            {
                                if (dialogData.Line[j] != '"')
                                    sb.Append(dialogData.Line[j]);
                            }
                            sb.Append(colorTagEnd);
                            dialogText.text = sb.ToString();
                            skip = true;
                            break;
                        }

                        yield return null;
                    }

                    if (skip) break;
                }

                switch (dialogData.CallbackEventType)
                {
                    case "NONE": break;
                }

                currentIndex++;

                if (dialogData.LineType == 0)
                {
                    if (currentIndex < dialogDataList.Count && dialogDataList[currentIndex].LineType == 0)
                    {
                        isTwinkling = true;
                        yield return new WaitForSecondsRealtime(0.1f);
                        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                    }
                }

                if (dialogData.LineType != 0)
                {
                    if (currentIndex < dialogDataList.Count && dialogDataList[currentIndex].LineType == 0)
                    {
                        isTwinkling = true;
                        skip = false;
                        yield return new WaitForSecondsRealtime(0.1f);
                        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                    }
                }

                yield return new WaitForSecondsRealtime(0.1f);
            }

            isTwinkling = true;


            if (dialogDataList[dialogDataList.Count - 1].DialogType == 1)
            {
                int currentChoiceIndex = 0;

                choiceButton_1.gameObject.SetActive(true);
                choiceButton_2.gameObject.SetActive(true);

                isTwinkling = false;

                while (!Keyboard.current.enterKey.wasPressedThisFrame)
                {
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

                    yield return null;
                }

                if (currentChoiceIndex == 0)
                {
                    if (dialogDataList[dialogDataList.Count - 1].ChoiceEvent_1 == "LoadScene")
                    {
                        StartDialogCloseRoutine(() =>
                        {
                            callback?.Invoke();
                            PlayerManager.Instance.StopPlayerInput();
                            GameManager.Instance.LoadScene(dialogDataList[dialogDataList.Count - 1].ChoiceEventString_1);
                        }, false);
                    }
                    else if (dialogDataList[dialogDataList.Count - 1].ChoiceEvent_1 == "Dialog")
                    {
                        StartDialogRoutine(DialogManager.Instance.GetLines(dialogDataList[dialogDataList.Count - 1].ChoiceEventString_1), callback);
                    }
                    else if (dialogDataList[dialogDataList.Count - 1].ChoiceEvent_1 == "NONE")
                    {
                        StartDialogCloseRoutine(callback);
                    }
                }
                else if (currentChoiceIndex == 1)
                {
                    if (dialogDataList[dialogDataList.Count - 1].ChoiceEvent_2 == "LoadScene")
                    {
                        StartDialogCloseRoutine(() =>
                        {
                            callback?.Invoke();
                            PlayerManager.Instance.StopPlayerInput();
                            GameManager.Instance.LoadScene(dialogDataList[dialogDataList.Count - 1].ChoiceEventString_2);
                        }, false);
                    }
                    else if (dialogDataList[dialogDataList.Count - 1].ChoiceEvent_2 == "Dialog")
                    {
                        StartDialogRoutine(DialogManager.Instance.GetLines(dialogDataList[dialogDataList.Count - 1].ChoiceEventString_2), callback);
                    }
                    else if (dialogDataList[dialogDataList.Count - 1].ChoiceEvent_2 == "NONE")
                    {
                        StartDialogCloseRoutine(callback);
                    }
                }
            }
            else
            {
                yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
                StartDialogCloseRoutine(callback, isStartPlayerInput);
            }


            yield return null;
        }

        public override void Open()
        {
            rootPanel.gameObject.SetActive(true);
        }

        public override void Close()
        {
            rootPanel.gameObject.SetActive(false);
        }
    }
}
