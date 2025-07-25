using System.Collections;
using UnityEngine;
using YUI.Agents.players;
using YUI.Effects;
using YUI.ObjPooling;

namespace YUI.Agents.AfterImages
{
    public class PlayerAfterImage : AgentAfterImage
    {
        [SerializeField] private AfterImageData adrenalineAfterImage;
        [SerializeField] private AfterImageData panicAfterImage;
        private Player player;
        private PlayerOverload playerOverload;

        private float currentAppliedStartScale;
        private float currentAppliedTargetScale;
        private float currentAppliedLifeTime;
        private float currentAppliedIntervalTime;
        private Color currentAppliedStartColor;
        private Color currentAppliedTargetColor;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);

            player = agent as Player;
            playerOverload = player.GetCompo<PlayerOverload>();
        }

        private void Update()
        {
            float inverseLerpValue;

            switch (player.PlayerMode)
            {
                case PlayerMode.NORMAL: {
                    inverseLerpValue = Mathf.InverseLerp(0, 50, playerOverload.Overload);

                    currentAppliedStartScale = Mathf.Lerp(normalAfterImage.startScale, adrenalineAfterImage.startScale, inverseLerpValue);
                    currentAppliedTargetScale = Mathf.Lerp(normalAfterImage.targetScale, adrenalineAfterImage.targetScale, inverseLerpValue);
                    currentAppliedLifeTime = Mathf.Lerp(normalAfterImage.lifeTime, adrenalineAfterImage.lifeTime, inverseLerpValue);
                    currentAppliedIntervalTime = Mathf.Lerp(normalAfterImage.intervalTime, adrenalineAfterImage.intervalTime, inverseLerpValue);
                    currentAppliedStartColor = Color.Lerp(normalAfterImage.startColor, adrenalineAfterImage.startColor, inverseLerpValue);
                    currentAppliedTargetColor = Color.Lerp(normalAfterImage.targetColor, adrenalineAfterImage.targetColor, inverseLerpValue);
                    break;
                }
                case PlayerMode.RELEASE: {
                    inverseLerpValue = Mathf.InverseLerp(50, 100, playerOverload.Overload);

                    currentAppliedStartScale = Mathf.Lerp(adrenalineAfterImage.startScale, panicAfterImage.startScale, inverseLerpValue);
                    currentAppliedTargetScale = Mathf.Lerp(adrenalineAfterImage.targetScale, panicAfterImage.targetScale, inverseLerpValue);
                    currentAppliedLifeTime = Mathf.Lerp(adrenalineAfterImage.lifeTime, panicAfterImage.lifeTime, inverseLerpValue);
                    currentAppliedIntervalTime = Mathf.Lerp(adrenalineAfterImage.intervalTime, panicAfterImage.intervalTime, inverseLerpValue);
                    currentAppliedStartColor = Color.Lerp(adrenalineAfterImage.startColor, panicAfterImage.startColor, inverseLerpValue);
                    currentAppliedTargetColor = Color.Lerp(adrenalineAfterImage.targetColor, panicAfterImage.targetColor, inverseLerpValue);
                    break;
                }
                case PlayerMode.OVERLOAD: {
                    inverseLerpValue = Mathf.InverseLerp(100, 0, playerOverload.Overload);

                    currentAppliedStartScale = Mathf.Lerp(panicAfterImage.startScale, normalAfterImage.startScale, inverseLerpValue);
                    currentAppliedTargetScale = Mathf.Lerp(panicAfterImage.targetScale, normalAfterImage.targetScale, inverseLerpValue);
                    currentAppliedLifeTime = Mathf.Lerp(panicAfterImage.lifeTime, normalAfterImage.lifeTime, inverseLerpValue);
                    currentAppliedIntervalTime = Mathf.Lerp(panicAfterImage.intervalTime, normalAfterImage.intervalTime, inverseLerpValue);
                    currentAppliedStartColor = Color.Lerp(panicAfterImage.startColor, normalAfterImage.startColor, inverseLerpValue);
                    currentAppliedTargetColor = Color.Lerp(panicAfterImage.targetColor, normalAfterImage.targetColor, inverseLerpValue);
                    break;
                }
            }
        }

        protected override IEnumerator Spawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(currentAppliedIntervalTime);
                AfterimageSpawn();
            }
        }

        protected override void AfterimageSpawn()
        {
            AfterImage obj = PoolingManager.Instance.Pop("AfterImage") as AfterImage;
            var sr = obj.GetComponent<SpriteRenderer>();
            sr.sprite = agentRenderer.Renderer.sprite;
            sr.flipX = agentRenderer.FacingDirection > 0 ? false : true;
            sr.color = new Vector4(1, 1, 1, 1);

            obj.transform.position = transform.position;
            obj.transform.localScale = currentAppliedStartScale * transform.localScale;

            StartCoroutine(AfterimageFadeAndScale(obj, sr));
        }

        protected override IEnumerator AfterimageFadeAndScale(AfterImage obj, SpriteRenderer sr)
        {
            float elapsed = 0f;
            Vector3 start = obj.transform.localScale;
            Vector3 target = transform.localScale * currentAppliedTargetScale;

            while (elapsed < currentAppliedLifeTime)
            {
                float t = elapsed / currentAppliedLifeTime;
                sr.color = Color.Lerp(currentAppliedStartColor, currentAppliedTargetColor, t);
                obj.transform.localScale = Vector3.Lerp(start, target, t);

                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            Color finalColor = sr.color;
            sr.color = finalColor;
            obj.transform.localScale = target;

            PoolingManager.Instance.Push(obj);
        }
    }
}
