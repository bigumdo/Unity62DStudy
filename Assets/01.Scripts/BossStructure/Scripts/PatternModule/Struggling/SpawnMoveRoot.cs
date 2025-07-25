using System;
using System.Collections;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "SpawnMoveRoot", menuName = "SO/Boss/Module/Struggling/SpawnMoveRoot")]
    public class SpawnMoveRoot : PatternModule
    {
        private enum DirectionEnum
        {
            Left, Right, Up, Down
        }

        [SerializeField] private DirectionEnum direction;
        [SerializeField] private float rangeMoveTime;
        [SerializeField] private float rangeMoveSpeed;
        [SerializeField] private float rangeStopTime;
        [SerializeField] private float attackDelay;
        [SerializeField] private float rootRemainTime;
        [SerializeField] private float rootDisappearTime;
        [SerializeField] private float damage;

        public override IEnumerator Execute()
        {
            SoundManager.Instance.PlaySound("SFX_Boss_RootWarning");

            TreeRootWarningEffect effect = PoolingManager.Instance.Pop("TreeRootWarningEffect") as TreeRootWarningEffect;
            CompleteActionExecute();

            float x = 0f;
            float y = 0f;

            switch (direction)
            {
                case DirectionEnum.Left: x = 0f; break;
                case DirectionEnum.Right: x = 1f; break;
                case DirectionEnum.Up: y = 1f; break;
                case DirectionEnum.Down: y = 0f; break;
            }

            float viewZ = 10f;
            Vector3 viewPos = new Vector3(x, y, viewZ);
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewPos);

            effect.transform.position = new Vector3(worldPos.x, worldPos.y);

            float angle = 0;

            Vector3 pos = effect.transform.position;
            Vector3 playerPos = PlayerManager.Instance.Player.transform.position;

            switch (direction)
            {
                case DirectionEnum.Left:
                case DirectionEnum.Right:
                    pos.y = playerPos.y;
                    break;
                case DirectionEnum.Up:
                case DirectionEnum.Down:
                    pos.x = playerPos.x;
                    break;
            }

            effect.transform.position = pos;

            float elapsedTime = 0f;
            while (elapsedTime < rangeMoveTime)
            {
                playerPos = PlayerManager.Instance.Player.transform.position;
                
                switch (direction)
                {
                    case DirectionEnum.Left:
                        pos.y = Mathf.Lerp(pos.y, playerPos.y, rangeMoveSpeed * Time.deltaTime);
                        effect.transform.rotation = Quaternion.Euler(0, 0, 0f);
                        angle = 0f;
                        break;
                    case DirectionEnum.Right:
                        pos.y = Mathf.Lerp(pos.y, playerPos.y, rangeMoveSpeed * Time.deltaTime);
                        effect.transform.rotation = Quaternion.Euler(0, 0, 180f);
                        angle = 180f;
                        break;
                    case DirectionEnum.Up:
                        pos.x = Mathf.Lerp(pos.x, playerPos.x, rangeMoveSpeed * Time.deltaTime);
                        effect.transform.rotation = Quaternion.Euler(0, 0, 90f);
                        angle = 90f;
                        break;
                    case DirectionEnum.Down:
                        pos.x = Mathf.Lerp(pos.x, playerPos.x, rangeMoveSpeed * Time.deltaTime);
                        effect.transform.rotation = Quaternion.Euler(0, 0, 270f);
                        angle = 270f;
                        break;
                }

                effect.transform.position = pos;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(rangeStopTime);

            SpriteRenderer effectSr = effect.GetComponent<SpriteRenderer>();

            Color color = effectSr.color;

            SoundManager.Instance.PlaySound("SFX_Boss_RootStart");

            elapsedTime = 0;
            while (elapsedTime < 0.1f)
            {
                color.a = Mathf.Lerp(1, 0, elapsedTime / 0.1f);
                effectSr.color = color;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            color.a = 0;
            effectSr.color = color;

            SoundManager.Instance.PlaySound("SFX_Boss_RootEnd");

            PoolingManager.Instance.Push(effect);
            
            yield return new WaitForSeconds(attackDelay);

            TreeRootObject treeRootObject = PoolingManager.Instance.Pop("TreeRootObject") as TreeRootObject;

            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector3 endPos = pos - dir * treeRootObject.transform.localScale.x / 4f;
            Vector3 startPos = endPos - dir * treeRootObject.transform.localScale.x;

            treeRootObject.transform.rotation = Quaternion.Euler(0, 0, angle);
            treeRootObject.transform.position = startPos;

            elapsedTime = 0;

            treeRootObject.SetDamage(damage);
            treeRootObject.SetDamaged(false);

            while (elapsedTime < 0.5f)
            {
                treeRootObject.transform.position = Vector3.Lerp(startPos, endPos, EaseInBack(elapsedTime / 0.5f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            treeRootObject.SetDamaged(true);

            treeRootObject.transform.position = endPos;

            yield return new WaitForSeconds(rootRemainTime);

            SpriteRenderer attackObjectSr = treeRootObject.GetComponent<SpriteRenderer>();

            color = attackObjectSr.color;

            elapsedTime = 0;
            while (elapsedTime < rootDisappearTime)
            {
                color.a = Mathf.Lerp(1, 0, elapsedTime / rootDisappearTime);
                attackObjectSr.color = color;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            color.a = 0;
            attackObjectSr.color = color;

            PoolingManager.Instance.Push(treeRootObject);
            yield return null;
        }
        
        private float EaseInBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * x * x * x - c1 * x * x;
        }
    }
}
