using System;
using UnityEngine;
using YUI.Agents.AfterImages;

namespace YUI.Agents.players {
    public enum PlayerMoveDir {
        UP, DOWN, RIGHT, LEFT, STAY
    }

    public enum PlayerMoveType
    {
        NORMAL, BATTLE
    }

    public class PlayerMover : AgentMover
    {
        public PlayerMoveDir moveDirection { get; private set; } = PlayerMoveDir.RIGHT;
        [field: SerializeField] public PlayerMoveType moveType { get; private set; } = PlayerMoveType.NORMAL;
        [SerializeField] private GameObject OverloadCanvas;
       // [SerializeField] private GameObject HurtBeat;

        Vector3 moveDir = Vector3.zero;
        float lerpSpeed = 10f;

        private bool isReversal = false;

        private void Start() {
            SetMoveType(moveType);
        }

        public void SetReversal(bool isReversal)
        {
            this.isReversal = isReversal;
        }

        public void SetMovement(PlayerMoveDir dir)
        {
            moveDirection = dir;
        }

        public override void StopImmediately()
        {
            moveDirection = PlayerMoveDir.STAY;
        }

        public void SetMoveType(PlayerMoveType type)
        {
            moveType = type;

            if (moveType == PlayerMoveType.BATTLE)
            {
                OverloadCanvas.SetActive(true);
                //HurtBeat.SetActive(true);
                if (_agent is Player player)
                {
                    player.GetCompo<AgentRenderer>().SetParam(player.BattleParam, true);
                    player.GetCompo<AgentRenderer>().SetParam(player.RunParam, false);
                    player.GetCompo<AgentAfterImage>(true).Play();
                }
            }
            else
            {
                OverloadCanvas.SetActive(false);
                //HurtBeat.SetActive(false);
                if (_agent is Player player)
                {
                    player.GetCompo<AgentRenderer>().SetParam(player.BattleParam, false);
                    player.GetCompo<AgentRenderer>().SetParam(player.RunParam, true);
                    player.GetCompo<AgentAfterImage>(true).Stop();
                }
            }
        }

        protected override void MoveCharacter()
        {
            switch (moveType)
            {
                case PlayerMoveType.NORMAL:
                    base.MoveCharacter();
                    break;
                case PlayerMoveType.BATTLE:
                    MoveBattle();
                    break;
            }
        }

        private void MoveBattle()
        {
            if (_agent.CurrentState == "STUN")
            {
                _rbCompo.linearVelocity = Vector3.Lerp(_rbCompo.linearVelocity, Vector3.zero, Time.deltaTime * lerpSpeed / 1.5f);
                return;
            }

            if ((_agent as Player).InputReader.SlowMode)
            {
                float moveSpeed = _moveSpeed;
                Vector3 moveDirection = new Vector3(_Movement.x * moveSpeed / 2, _Movement.y * moveSpeed / 2) * (isReversal ? -1 : 1);
                _rbCompo.linearVelocity = Vector3.Lerp(_rbCompo.linearVelocity, moveDirection, Time.deltaTime * lerpSpeed / 1.5f);
            }
            else
            {

                switch (moveDirection)
                {
                    case PlayerMoveDir.UP:
                        {
                            moveDir = new Vector3(0, 1, 0);
                            break;
                        }
                    case PlayerMoveDir.DOWN:
                        {
                            moveDir = new Vector3(0, -1, 0);
                            break;
                        }
                    case PlayerMoveDir.RIGHT:
                        {
                            moveDir = new Vector3(1, 0, 0);
                            break;
                        }
                    case PlayerMoveDir.LEFT:
                        {
                            moveDir = new Vector3(-1, 0, 0);
                            break;
                        }
                    case PlayerMoveDir.STAY:
                        {
                            moveDir = Vector3.zero;
                            break;
                        }

                }

                float moveSpeed = _moveSpeed;

                _rbCompo.linearVelocity = moveDir * (isReversal ? -1 : 1) * moveSpeed;
            }
        }
    }
}
