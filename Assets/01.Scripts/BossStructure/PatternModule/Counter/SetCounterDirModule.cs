using System.Collections;
using UnityEngine;
using YUI.Agents.Bosses;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "SetCounterDirModule", menuName = "SO/Boss/Module/Counter/SetCounterDirModule")]
    public class SetCounterDirModule : PatternModule
    {
        public override IEnumerator Execute()
        {
            int dirCnt = Random.Range(0, 4);
            Vector3 randomDir = Vector3.right;
            switch (dirCnt)
            {
                case 0:
                    randomDir = _boss.transform.right;
                    break;
                case 1:
                    randomDir = -_boss.transform.right;
                    break;
                case 2:
                    randomDir = _boss.transform.up;
                    break;
                case 3:
                    randomDir = -_boss.transform.up;
                    break;
            }

            BossManager.Instance.counterDir = randomDir;
            CompleteActionExecute();
            yield break;
        }
    }
}
