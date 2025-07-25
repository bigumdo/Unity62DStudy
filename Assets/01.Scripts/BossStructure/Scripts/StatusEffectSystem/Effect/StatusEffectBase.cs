using UnityEngine;
using YUI.Agents.players;

namespace YUI.StatusEffects {
    public abstract class StatusEffectBase {
        protected float duration;
        protected float value;
        protected Player player;
        protected float startTime;
        public StatusEffectType type;

        public StatusEffectBase(float duration, Player player, StatusEffectType type) {
            this.duration = duration;
            this.player = player;
            this.type = type;
        }

        public abstract void Start();
        public virtual void Update() { }
        public abstract void End();

        public void ResetTime() {
            startTime = Time.time;
        }

        public virtual void SetValue(float value) {
            this.value = value;
        }

        public virtual bool IsEnd() {
            return startTime + duration < Time.time;
        }
    }
}
