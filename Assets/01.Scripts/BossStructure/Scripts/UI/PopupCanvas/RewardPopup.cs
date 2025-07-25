using TMPro;
using UnityEngine;
using YUI.Cores;
using YUI.Skills;
using YUI.UI;

namespace YUI.UI
{
    public class RewardPopup : LegacyBaseUI
    {
        [SerializeField] private RectTransform rootPanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private float yOffset = 50f;

        private void Awake()
        {
            UIManager.Instance.AddUI(this);
        }

        public void SetPosition(Vector3 position)
        {
            rootPanel.position = Camera.main.WorldToScreenPoint(position) + new Vector3(0, yOffset, 0);
        }

        public void SetNameAndDescription(PassiveSkill skill) 
        {
            nameText.text = skill.NodeName;
            descriptionText.text = skill.NodeDescription;
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
