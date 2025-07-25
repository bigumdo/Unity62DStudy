using System.Collections;
using UnityEngine;
using YUI.Agents.Bosses;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BossCenterMoveModule", menuName = "SO/Boss/Module/Move/BossCenterMoveModule")]
    public class BossCenterMoveModule : PatternModule
    {
        [Tooltip("������ ��ġ���� �̵� �ð� ����")]
        [SerializeField] private float _moveDrutation;

        public override IEnumerator Execute()
        {
            yield return _boss.GetCompo<BossMover>().DOMove(BossManager.Instance.startPos, _moveDrutation);
            CompleteActionExecute();
        }
    }
}
