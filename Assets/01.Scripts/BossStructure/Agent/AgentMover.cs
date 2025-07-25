using YUI.Animators;
using YUI.StatSystem;
using System;
using UnityEngine;

namespace YUI.Agents
{
    public class 
        AgentMover : MonoBehaviour, IAgentComponent, IAfterInit
    {
        public Vector3 Velocity => _rbCompo.linearVelocity;
        public bool CanMove { get; set; } = true;
        public bool IsGrounded { get; set; }
        public float LimitYSpeed { get; set; }

        [Header("MoveStat")]
        [SerializeField] protected StatSO _moveSpeedStat;
        protected float _moveSpeed;

        protected Rigidbody2D _rbCompo;
        protected Agent _agent;
        protected AgentRenderer _renderer;
        protected AgentStat _agentStat;

        protected Vector3 _Movement;

        protected Collider _collider;

        public virtual void Initialize(Agent agent)
        {
            _agent = agent;
            _rbCompo = agent.GetComponent<Rigidbody2D>();
            _renderer = agent.GetCompo<AgentRenderer>(true);
            _agentStat = agent.GetCompo<AgentStat>();
            _collider = agent.GetComponent<Collider>();
        }

        public virtual void AfterInit()
        {
            _agentStat.MoveSpeedStat.OnValueChange += HandleMoveSpeedChange;
            _moveSpeed = _agentStat.MoveSpeedStat.Value;
            LimitYSpeed = 40f;
        }

        protected virtual void OnDestroy()
        {
            _agentStat.MoveSpeedStat.OnValueChange -= HandleMoveSpeedChange;
        }

        protected virtual void HandleMoveSpeedChange(StatSO stat, float current, float previous)
        {
            _moveSpeed = current;
        }

        public void AddForce(Vector3 force, ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            _rbCompo.AddForce(force, forceMode);
        }

        public virtual void StopImmediately()
        {
            _rbCompo.linearVelocity = Vector3.zero;
        }

        public virtual void SetMovement(Vector3 Movement) => _Movement = Movement;

        public virtual void FixedUpdate()
        {
            CheckGround();
            MoveCharacter();

            Vector3 clampedVelocity = _rbCompo.linearVelocity;
            clampedVelocity.y = Mathf.Clamp(clampedVelocity.y, -LimitYSpeed, LimitYSpeed);
            _rbCompo.linearVelocity = clampedVelocity;
        }

        protected virtual void MoveCharacter()
        {
            if (CanMove)
            {
                Vector3 moveDirection = new Vector3(_Movement.x * _moveSpeed, _Movement.y * _moveSpeed);
                _rbCompo.linearVelocity = moveDirection;
            }

            _renderer.FlipController(_Movement.x);
        }

        protected virtual void CheckGround()
        {
        }
        
        protected virtual void IsWallDetected()
        {
        }
    }
}
