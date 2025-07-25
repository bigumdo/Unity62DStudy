using UnityEngine;
using YUI.Agents.players;

namespace YUI.Fields {
    public class FieldEventSO : ScriptableObject {
        protected FieldEventTrigger owner;
        protected Player player;

        protected bool isActive = true;

        protected GameObject currentCollidedObject;

        public void SetActive(bool isActive) => this.isActive = isActive;

        public virtual void SetOwner(FieldEventTrigger owner) { 
            this.owner = owner;

            player = PlayerManager.Instance.Player;
        }

        public virtual void Execute() { }
        public virtual void SetCurrentCollidedObject(GameObject obj) => currentCollidedObject = obj;
    }
}
