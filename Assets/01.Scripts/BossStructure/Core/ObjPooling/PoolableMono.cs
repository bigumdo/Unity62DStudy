using UnityEngine;

namespace YUI.ObjPooling
{
    public abstract class PoolableMono : MonoBehaviour
    {
        public string type;
        public abstract void ResetItem();
    }
}
