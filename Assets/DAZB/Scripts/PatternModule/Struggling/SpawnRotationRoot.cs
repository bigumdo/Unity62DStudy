using System.Collections;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "SpawnRotationRoot", menuName = "SO/Boss/Module/Struggling/SpawnRotationRoot")]
    public class SpawnRotationRoot : PatternModule
    {
        [Range(0, 1), SerializeField] private float startX;
        [Range(0, 1), SerializeField] private float endX;
        [Range(0, 1), SerializeField] private float startY;
        [Range(0, 1), SerializeField] private float endY;
        [SerializeField] private float rangeRotationTime;
        [SerializeField] private float rangeRotationSpeed;
        [SerializeField] private float rangeStopTime;
        [SerializeField] private float attackDelay;
        [SerializeField] private float rootRemainTime;
        [SerializeField] private float rootDisappearTime;
        [SerializeField] private int damage;

        public override IEnumerator Execute()
        {
            SoundManager.Instance.PlaySound("SFX_Boss_RootWarning");

            TreeRootWarningEffect effect = PoolingManager.Instance.Pop("TreeRootWarningEffect") as TreeRootWarningEffect;
            CompleteActionExecute();

            float rand = (float)Random.Range(0, 100) / 100;

            float x = Mathf.Lerp(startX, endX, rand);
            float y = Mathf.Lerp(startY, endY, rand);

            float viewZ = 10f;
            Vector3 viewPos = new Vector3(x, y, viewZ);
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewPos);

            effect.transform.position = new Vector3(worldPos.x, worldPos.y);

            float elapsedTime = 0;
            float targetTime = rangeRotationTime;
            float angle = 0;

            Vector3 dir;

            dir = PlayerManager.Instance.Player.transform.position - effect.transform.position;
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            effect.transform.rotation = Quaternion.Euler(0, 0, angle);

            while (elapsedTime < targetTime)
            {
                dir = PlayerManager.Instance.Player.transform.position - effect.transform.position;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                effect.transform.rotation = Quaternion.RotateTowards(
                    effect.transform.rotation,
                    targetRotation,
                    rangeRotationSpeed * Time.deltaTime
                );

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(rangeStopTime);

            SpriteRenderer effectSr = effect.GetComponent<SpriteRenderer>();

            Color c = effectSr.color;

            SoundManager.Instance.PlaySound("SFX_Boss_RootStart");

            elapsedTime = 0;
            while (elapsedTime < 0.1f)
            {
                c.a = Mathf.Lerp(1, 0, elapsedTime / 0.1f);
                effectSr.color = c;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            c.a = 0;
            effectSr.color = c;

            SoundManager.Instance.PlaySound("SFX_Boss_RootEnd");

            PoolingManager.Instance.Push(effect);

            yield return new WaitForSeconds(attackDelay);

            TreeRootObject treeRootObject = PoolingManager.Instance.Pop("TreeRootObject") as TreeRootObject;

            dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector3 endPos = new Vector3(worldPos.x, worldPos.y) - dir * treeRootObject.transform.localScale.x / 3;
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

            Color color = attackObjectSr.color;

            elapsedTime = 0;
            targetTime = rootDisappearTime;
            while (elapsedTime < targetTime)
            {
                color.a = Mathf.Lerp(1, 0, elapsedTime / targetTime);
                attackObjectSr.color = color;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            color.a = 0;
            attackObjectSr.color = color;

            PoolingManager.Instance.Push(treeRootObject);
        }
        
        private float EaseInBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * x * x * x - c1 * x * x;
        }
    }
}
