using YUI.Animators;
using UnityEngine;

namespace YUI.Agents
{
    public class AgentRenderer : MonoBehaviour, IAgentComponent
    {
        public float FacingDirection { get; protected set; } = 1;
        public SpriteRenderer Renderer { get; protected set; }

        protected Agent _agent;
        protected Animator _animator;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            _animator = GetComponent<Animator>();
            Renderer = GetComponent<SpriteRenderer>();
        }


        public void SetParam(AnimParamSO paramSO, bool value) => _animator.SetBool(paramSO.hashValue, value);

        public void SetParam(AnimParamSO paramSO, float value) => _animator.SetFloat(paramSO.hashValue, value);
        public void SetParam(AnimParamSO paramSO, int value) => _animator.SetInteger(paramSO.hashValue, value);
        public void SetParam(AnimParamSO paramSO) => _animator.SetTrigger(paramSO.hashValue);

        public virtual void FlipController(float xMove)
        {
            if (Mathf.Abs(FacingDirection + xMove) < 0.5f)
                Flip();
        }

        public virtual void Flip()
        {
            FacingDirection *= -1;
            //_agent.transform.Rotate(0, 180, 0);
            Renderer.flipX = FacingDirection < 0 ? true : false;
            //_agent.transform.localScale = new Vector3(FacingDirection,1,1);
        }

    }
}
