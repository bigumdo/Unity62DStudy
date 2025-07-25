using UnityEngine;
using YUI.Cores;
using YUI.Fields;

namespace YUI.Field {
    [CreateAssetMenu(fileName = "SetConfinerEvent", menuName = "Field/Events/SetConfinerEvent ")]
    public class SetConfinerEvent : FieldEventSO {
        public override void Execute() {
            CameraManager.Instance.SetConfiner((owner.GetVariable("Confiner").value as GameObject).GetComponent<Collider2D>());
        }
    }
}
