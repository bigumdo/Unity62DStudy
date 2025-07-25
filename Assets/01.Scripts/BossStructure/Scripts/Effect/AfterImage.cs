using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Effects {
    public class AfterImage : PoolableMono {
        public override void ResetItem()
        {
            transform.localScale = Vector3.zero;
        }
    }
}
