using YUI.ObjPooling;
using System.Collections;
using UnityEngine;
using YUI.Effects;

namespace YUI.Agents.AfterImages
{
    [System.Serializable]
    public class AfterImageData
    {
        public float intervalTime;
        public float lifeTime;
        public float startScale;
        public float targetScale;
        public Color startColor = new Color(1, 1, 1, 1);
        public Color targetColor = new Color(1, 1, 1, 1);
    }
    
    public class AgentAfterImage : MonoBehaviour, IAgentComponent, IAfterInit
    {
        [SerializeField] protected bool playOnAwake = false;
        [SerializeField] protected AfterImageData normalAfterImage;
        public bool IsPlay { get; protected set; } = false;

        //private Player _player;
        protected AgentRenderer agentRenderer;

        protected Coroutine spawnCoroutine;

        public virtual void Initialize(Agent agent)
        {
            //_player = agent as Player;
            agentRenderer = agent.GetCompo<AgentRenderer>(true);

            if (playOnAwake)
            {
                Play();
            }
        }
        public void AfterInit()
        {

        }

        protected void OnEnable()
        {
            if (IsPlay)
            {
                Play();
            }
        }

        public void Play()
        {
            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);
            IsPlay = true;
            spawnCoroutine = StartCoroutine(Spawn());
        }

        public void Stop()
        {
            IsPlay = false;

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }

        }

        protected virtual IEnumerator Spawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(normalAfterImage.intervalTime);
                AfterimageSpawn();
            }

        }

        protected virtual void AfterimageSpawn()
        {
            AfterImage obj = PoolingManager.Instance.Pop("AfterImage") as AfterImage;
            var sr = obj.GetComponent<SpriteRenderer>();
            sr.sprite = agentRenderer.Renderer.sprite;
            sr.flipX = agentRenderer.FacingDirection > 0 ? false : true;
            sr.color = new Vector4(1, 1, 1, 1);

            obj.transform.position = transform.position;
            obj.transform.localScale = normalAfterImage.startScale * transform.localScale;

            StartCoroutine(AfterimageFadeAndScale(obj, sr));
        }

        protected virtual IEnumerator AfterimageFadeAndScale(AfterImage obj, SpriteRenderer sr)
        {
            float elapsed = 0f;
            Vector3 start = obj.transform.localScale;
            Vector3 target = transform.localScale * normalAfterImage.targetScale;

            while (elapsed < normalAfterImage.lifeTime)
            {
                float t = elapsed / normalAfterImage.lifeTime;
                sr.color = Color.Lerp(normalAfterImage.startColor, normalAfterImage.targetColor, t);
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
    