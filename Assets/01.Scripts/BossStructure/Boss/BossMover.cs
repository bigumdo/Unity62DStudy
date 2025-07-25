using System.Collections;
using UnityEngine;
using YUI.Agents.AfterImages;
using YUI.Cores;
using YUI.StatSystem;

namespace YUI.Agents.Bosses
{
    public class BossMover : AgentMover
    {
        public override void FixedUpdate()
        {
            MoveCharacter();
        }

        protected override void MoveCharacter()
        {
            if (CanMove)
            {
                Vector3 moveDirection = new Vector3(_Movement.x * _moveSpeed, _rbCompo.linearVelocity.y, _Movement.z * _moveSpeed);
                _rbCompo.linearVelocity = moveDirection;
            }
        }

        public IEnumerator DOMove(Vector3 endPos, float duration)
        {
            float time = 0;
            Vector3 startPos = _agent.transform.position;
            _agent.GetCompo<AgentAfterImage>().Play();
            SoundManager.Instance.PlaySound("SFX_Boss_Move");
            while (time < duration)
            {
                time += Time.deltaTime;
                Debug.DrawRay(_agent.transform.position, endPos - transform.position);
                _agent.transform.position = Vector3.Lerp(startPos, endPos, Mathf.Sin((time / duration * Mathf.PI) / 2));
                yield return null;
            }
            _agent.GetCompo<AgentAfterImage>().Stop();

            _agent.transform.position = endPos;
        }

        public IEnumerator DOFade(float alpha, float duration)
        {
            float startAlpha = _renderer.Renderer.color.a;
            float time = 0;
            Color color = _renderer.Renderer.color;

            while (time < duration)
            {
                time += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, alpha, time / duration);
                _renderer.Renderer.color = color;
                yield return null;
            }
            color.a = alpha;
            _renderer.Renderer.color = color;
        }
    }
}
