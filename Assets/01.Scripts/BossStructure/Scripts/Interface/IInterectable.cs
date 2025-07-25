using UnityEngine;
using YUI.Agents.players;

namespace YUI {
    public interface IInteractable {
        public void Interact(Player player);
        public void Interactable(bool active);
    }
}
