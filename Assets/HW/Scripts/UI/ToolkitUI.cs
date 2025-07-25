using UnityEngine;
using UnityEngine.UIElements;

namespace YUI
{
    public abstract class ToolkitUI : MonoBehaviour, IUI
    {
        [SerializeField] public VisualTreeAsset visualTreeAsset;

        protected UIDocument _uiDocument;
        protected VisualElement root;

        protected virtual void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            root = _uiDocument.rootVisualElement;
        }

        public abstract void Open();
        public abstract void Close();
    }
}
