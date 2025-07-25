using UnityEngine;
using YUI.Agents.players;

namespace YUI.StatusEffects {
    public class ReversalEffect : StatusEffectBase {
        private PlayerMover playerMover;

        public ReversalEffect(float duration, Player player, StatusEffectType type) : base(duration, player, type)
        {
            playerMover = player.GetCompo<PlayerMover>(true);
        }

        public override void Start() {
            playerMover.SetReversal(true);
        }

        public override void End() {
            playerMover.SetReversal(false);
        }
    }
}
