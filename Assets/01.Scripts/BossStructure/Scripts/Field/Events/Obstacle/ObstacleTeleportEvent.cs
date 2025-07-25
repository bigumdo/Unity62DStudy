using UnityEngine;

namespace YUI.Fields {
    [CreateAssetMenu(fileName = "ObstacleTeleportEvent", menuName = "Field/Events/ObstacleTeleportEvent")]
    public class ObstacleTeleportEvent : FieldEventSO {
        public override void Execute() {
            base.Execute();

            currentCollidedObject.transform.position = (owner.GetVariable("ExitPos").value as GameObject).transform.position;
        }
    }
}
