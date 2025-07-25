using UnityEngine;
using UnityEngine.UI;

namespace YUI.UI.DialogSystem
{
    public class DialogChoiceButton : MonoBehaviour
    {
        [SerializeField] private Outline outline;
        private bool isSelected;

        public void Select()
        {
            if (isSelected) return;

            outline.enabled = true;
            isSelected = true;
        }
        
        public void Deselect()
        {
            outline.enabled = false;
            isSelected = false;
        }
    }
}
