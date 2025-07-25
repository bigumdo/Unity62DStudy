using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace YUI.UI
{
    public class DeadCanvasButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent callback;
        public TextMeshProUGUI text;
        public string defaultText;

        public void Init()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();

            defaultText = text.text;
        }

        public void Execute()
        {
            callback?.Invoke();
        }
    }
}
