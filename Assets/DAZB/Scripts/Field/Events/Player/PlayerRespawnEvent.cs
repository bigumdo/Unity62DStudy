using UnityEngine;
using YUI.Agents.players;

namespace YUI.Fields {
    [CreateAssetMenu(fileName = "PlayerRespawnEvent", menuName = "Field/Events/PlayerRespawnEvent")]
    public class PlayerRespawnEvent : FieldEventSO {
        public override void Execute() {
            player.ChangeState("REVIVING");
        }
    }
}
