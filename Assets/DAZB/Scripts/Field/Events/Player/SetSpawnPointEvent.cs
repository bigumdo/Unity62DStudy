using UnityEngine;

namespace YUI.Fields {
    [CreateAssetMenu(fileName = "SetSpawnPointEvent", menuName = "Field/Events/SetSpawnPointEvent")]
    public class SetSpawnPointEvent : FieldEventSO {
        public override void Execute() {
            base.Execute();

            FieldManager.Instance.SetPlayerSpawnPoint(owner.transform.position);
        }
    }
}
