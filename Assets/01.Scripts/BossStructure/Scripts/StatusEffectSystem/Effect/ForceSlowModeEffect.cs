using UnityEngine;
using YUI.Agents.players;

namespace YUI.StatusEffects {
    public class ForceSlowModeEffect : StatusEffectBase {

        public ForceSlowModeEffect(float duration, Player player, StatusEffectType type) : base(duration, player, type)
        {
        }

        public override void Start()
        {
            player.InputReader.SetSlowMode(true);
        }

        public override void End()
        {
            player.InputReader.SetSlowMode(false);
        }
    }
}
