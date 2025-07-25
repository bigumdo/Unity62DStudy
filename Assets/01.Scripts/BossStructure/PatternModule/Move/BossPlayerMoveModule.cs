using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BossPlayerMoveModule", menuName = "SO/Boss/Module/Move/BossPlayerMoveModule")]
    public class BossPlayerMoveModule : PatternModule
    {
        [Tooltip("플레이어와 얼마나 떯어져있는지")]
        [SerializeField] private float _distance;
        [Tooltip("도착까지 걸리는 시간")]
        [SerializeField] private float _moveDrutation;

        public override IEnumerator Execute()
        {
            Vector2 circle;
            Vector3 randomDir;
            Vector3 playerPos;
            while (true)
            {
                circle = Random.insideUnitCircle.normalized;
                randomDir = new Vector3(circle.x, circle.y).normalized;
                playerPos = PlayerManager.Instance.Player.transform.position;
                LayerMask layer = 1 << LayerMask.NameToLayer("Wall");
                if (!Physics2D.Raycast(playerPos, randomDir, _distance, layer))
                {
                    if(!Physics2D.BoxCast(playerPos + randomDir * _distance, _boss.GetComponent<BoxCollider2D>().size, 0, Vector2.zero,0,layer))
                    {
                        Vector3 targetPos = playerPos + randomDir * _distance;
                        yield return _boss.GetCompo<BossMover>().DOMove(targetPos, _moveDrutation);
                        CompleteActionExecute();
                        yield break;
                    }
                }
                yield return null;
            }
        }
    }
}
