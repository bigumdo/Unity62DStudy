using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Skills;
using YUI.UI;

namespace YUI.Rewards {
    public class RewardObject : MonoBehaviour, IInteractable
    {
        private PassiveSkill passiveNode;
        private Reward reward;

        public void Interact(Player player)
        {
            if (passiveNode != null)
            {
                player.GetCompo<PlayerSkill>().AddPassive(passiveNode);
                reward.SelectedObject();
            }
        }

        public void Interactable(bool active)
        {

            if (active)
            {
                UIManager.Instance.GetUI<RewardPopup>().Open();
            }
            else
            {
                UIManager.Instance.GetUI<RewardPopup>().Close();
            }

            if (this == null) return;

            if (gameObject == null) return;

            if (passiveNode != null)
                UIManager.Instance.GetUI<RewardPopup>().SetNameAndDescription(passiveNode);

            if (this != null && gameObject != null)
                UIManager.Instance.GetUI<RewardPopup>().SetPosition(transform.position);
        }

        public void SetPassiveNode(PassiveSkill node)
        {
            passiveNode = node;
        }

        public void SetOwner(Reward owner)
        {
            reward = owner;
        }
        
    }
}
