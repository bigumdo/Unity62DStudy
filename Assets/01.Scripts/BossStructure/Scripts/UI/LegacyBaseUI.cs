using UnityEngine;
using YUI.Cores;

namespace YUI.UI
{
    public abstract class LegacyBaseUI : MonoBehaviour, IUI
    {
        public abstract void Open();

        public abstract void Close();
    }
}
