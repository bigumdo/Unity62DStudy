using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules {
    [System.Serializable]
    public class TreeRootData
    {
        [Range(0, 1)] public float startX;
        [Range(0, 1)] public float endX;

        [Space, Space]

        [Range(0, 1)] public float startY;
        [Range(0, 1)] public float endY;

        [Space, Space]

        public float rotateTime;
        public float donRotateTime;

        public float nextDelayTime;

        public float fadeTime;

        public int damage;
    }

    [CreateAssetMenu(fileName = "TreeRootModule", menuName = "SO/Boss/Module/NextPhase/TreeRootModule")]
    public class TreeRootModule : PatternModule
    {
        [SerializeField] private List<TreeRootData> treeRootDataList;

        public override IEnumerator Execute()
        {
            List<TreeRootObject> spawnedObject = new List<TreeRootObject>();

            for (int i = 0; i < treeRootDataList.Count; ++i)
            {
                TreeRootWarningEffect effect = PoolingManager.Instance.Pop("TreeRootWarningEffect") as TreeRootWarningEffect;

                float rand = (float)Random.Range(0, 100) / 100;

                float x = Mathf.Lerp(treeRootDataList[i].startX, treeRootDataList[i].endX, rand);
                float y = Mathf.Lerp(treeRootDataList[i].startY, treeRootDataList[i].endY, rand);

                float viewZ = 10f;
                Vector3 viewPos = new Vector3(x, y, viewZ);
                Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewPos);

                effect.transform.position = new Vector3(worldPos.x, worldPos.y);

                float elapsedTime = 0;
                float targetTime = treeRootDataList[i].rotateTime;
                float angle = 0;

                Vector3 dir;

                SoundManager.Instance.PlaySound("SFX_Boss_RootWarning");
                while (elapsedTime < targetTime)
                {
                    dir = PlayerManager.Instance.Player.transform.position - effect.transform.position;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    effect.transform.rotation = Quaternion.Euler(0, 0, angle);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                yield return new WaitForSeconds(treeRootDataList[i].donRotateTime);
                effect.Push();

                TreeRootObject treeRootObject = PoolingManager.Instance.Pop("TreeRootObject") as TreeRootObject;

                treeRootObject.attackObject.GetComponent<SpriteRenderer>().sortingOrder = 20 + i;

                spawnedObject.Add(treeRootObject);

                dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                float scale = treeRootObject.transform.localScale.x;
                Vector3 endPos = new Vector3(worldPos.x, worldPos.y) - dir * (scale / 5f);
                Vector3 startPos = new Vector3(worldPos.x, worldPos.y) - dir * (scale + scale / 3f);

                treeRootObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                treeRootObject.transform.position = startPos;

                elapsedTime = 0;
                targetTime = 0.5f;

                SoundManager.Instance.PlaySound("SFX_Boss_RootStart");
                treeRootObject.SetDamage(treeRootDataList[i].damage);
                treeRootObject.SetDamaged(false);

                CameraManager.Instance.ShakeCamera(4, 2, 0.1f);

                while (elapsedTime < targetTime)
                {
                    treeRootObject.transform.position = Vector3.Lerp(startPos, endPos, EaseInBack(elapsedTime / targetTime));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                treeRootObject.SetDamaged(true);
                SoundManager.Instance.PlaySound("SFX_Boss_RootEnd");
                CameraManager.Instance.ShakeCamera(10, 5, 0.1f);

                rand = Random.Range(0, 2);

                if (rand > 0)
                {
                    treeRootObject.attackObject.transform.localPosition = new Vector3(0, -24.5f, 0);
                }
                else
                {
                    treeRootObject.attackObject.transform.localPosition = new Vector3(0, 24.5f, 0);
                }

                treeRootObject.transform.position = endPos;

                SpriteRenderer attackObjectSr = treeRootObject.attackObject.GetComponent<SpriteRenderer>();

                targetTime = treeRootDataList[i].fadeTime;

                if (treeRootObject.attackObject.transform.localPosition.y < 0)
                {
                    attackObjectSr.material.SetFloat("_IsMinus", 1);
                }
                else
                {
                    attackObjectSr.material.SetFloat("_IsMinus", 0);
                }

                _boss.StartCoroutine(FadeRoutine(attackObjectSr, targetTime));
                yield return new WaitForSeconds(treeRootDataList[i].nextDelayTime);
            }

            foreach (var iter in spawnedObject)
            {
                //iter.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
            }
            yield return new WaitForSeconds(1);

            foreach (var iter in spawnedObject)
            {
                yield return Effect(iter.attackObject.GetComponent<SpriteRenderer>(), 1);
            }

            yield return new WaitForSeconds(1);

            //foreach (var iter in spawnedObject)
            //{
            //    iter.attackObject.GetComponent<SpriteRenderer>().material.SetFloat("_IsAttack", 1);
            //    iter.attackObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0f);
            //    iter.attackObject.GetComponent<SpriteRenderer>().DOFade(1, 0.2f);
            //}
            CameraManager.Instance.ShakeCamera(30, 1, 0.1f);
            CameraManager.Instance.SetFadeColor(Color.white);
            CameraManager.Instance.FadeOut(0.1f);
            foreach (var iter in spawnedObject)
            {
                iter.attackObject.SetCanAttack(true);
            }
            SoundManager.Instance.PlaySound("SFX_Boss_TreeRootExplosion");
            yield return new WaitForSeconds(0.1f);
            CameraManager.Instance.FadeIn(0.2000000001f);

            yield return new WaitForSeconds(0.1f);


            foreach (var iter in spawnedObject)
            {
                iter.attackObject.GetComponent<SpriteRenderer>().material.SetFloat("_IsAttack", 0);
            }

            spawnedObject.ForEach(x => PoolingManager.Instance.Push(x));

            yield return new WaitForSeconds(0.1f);
            CameraManager.Instance.SetFadeColor(Color.black);
            //foreach (var iter in spawnedObject)
            //{
            //    iter.attackObject.GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            //}

            //yield return new WaitForSeconds(0.05f);

            //foreach (var iter in spawnedObject)
            //{
            //    iter.GetComponent<SpriteRenderer>().DOFade(0, 0.1f);
            //}

            yield return new WaitForSeconds(0.1f);
            
            CompleteActionExecute();
        }

        private IEnumerator Effect(SpriteRenderer objRenderer ,float duration)
        {
            float time = 0;
            float stopTime = duration * 0.5f;
            CameraManager.Instance.ShakeCamera(10, 1, 0.15f);
            SoundManager.Instance.PlaySound("SFX_Boss_TreeRootBreak");
            while (time <= duration)
            {
                time += Time.deltaTime;
                objRenderer.material.SetFloat("_BrokenAmount", time / duration);

                if (time >= stopTime)
                {
                    stopTime = duration + 1;
                    yield return new WaitForSeconds(0.1f);
                    CameraManager.Instance.ShakeCamera(13, 1, 0.1f);
                    SoundManager.Instance.PlaySound("SFX_Boss_TreeRootBreak");
                }
                yield return null;
            }

            objRenderer.material.SetFloat("_BrokenAmount", 1);
        }

        private IEnumerator FadeRoutine(SpriteRenderer attackObjectSr, float targetTime)
        {
            float fadeElapsed = 0f;
            while (fadeElapsed < targetTime)
            {
                float t = fadeElapsed / targetTime;
                float value = Mathf.Lerp(1f, 0f, t);
                attackObjectSr.material.SetFloat("_Float", value);
                fadeElapsed += Time.deltaTime;
                yield return null;
            }
            attackObjectSr.material.SetFloat("_Float", 0f);
        }
        
        private float EaseInBack(float x)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1f;

            return c3 * x * x * x - c1 * x * x;
        }
    }
}
