using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.ObjPooling;

namespace YUI.PatternModules {
    public class TreeRootWarningEffect : PoolableMono
    {
        public override void ResetItem()
        {

        }

        public void Push()
        {
            PoolingManager.Instance.Push(this);
        }
    }
}
