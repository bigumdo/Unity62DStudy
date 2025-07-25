using System.Collections;
using UnityEngine;
using YUI.Agents;

namespace YUI.Agents.Bosses
{
    public class BossCounterHead : MonoBehaviour, IAgentComponent
    {
        private float _distance;
        private Boss _boss;
        private SpriteRenderer _renderer;
        [SerializeField] private ParticleSystem _testParticle;

        public void Initialize(Agent agent)
        {
            _boss = agent as Boss;
            _renderer = GetComponentInChildren<SpriteRenderer>();
            Color color = _renderer.color;
            color.a = 0;
            _renderer.color = color;
        }

        public IEnumerator Fade(float alpha,float durationTime=0)
        {
            if (alpha == 1)
            {
                Instantiate(_testParticle, _boss.transform);
                yield return new WaitForSeconds(0.5f);
            }

            float time = 0;
            Color color = _renderer.color;
            float startAlpha = color.a;
            if(durationTime <= 0)
            {
                color.a = alpha;
                _renderer.color = color;
                yield break;
            }
            while(time < durationTime)
            {
                time += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, alpha, time / durationTime);
                _renderer.color = color;
                yield return null;
            }
            color.a = alpha;
            _renderer.color = color;
        }

        public void SetSpawnDistance(float distance)
        {
            _distance = distance;
        }
        public void SetPos(Vector3 dir)
        {
            if (dir == Vector3.right)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
            else if (dir == Vector3.down)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            else if (dir == Vector3.left)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            else if (dir == Vector3.up)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 360));

            transform.position = _boss.transform.position + dir * _distance;
        }
    }
}
