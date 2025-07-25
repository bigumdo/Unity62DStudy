using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.Agents.Bosses
{
    public class Griv : Boss 
    {
        private void Start()
        {
            BossManager.Instance.SetBoss(this);
        }

        public override IEnumerator DeadEffect()
        {
            CameraManager.Instance.ShakeCamera(1, 1, 1f);
            yield return new WaitForSeconds(1);
            //GetCompo<BossRenderer>
        }
    }
}
