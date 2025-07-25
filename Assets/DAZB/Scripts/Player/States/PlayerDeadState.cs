using System.Collections;
using UnityEngine;
using YUI.Agents.AfterImages;
using YUI.Animators;
using YUI.Cores;
using YUI.Effects;
using YUI.FSM;
using YUI.ObjPooling;
using YUI.UI;

namespace YUI.Agents.players
{
    public class PlayerDeadState : AgentState
    {
        private Player player;
        private PlayerMover mover;
        private AgentAfterImage afterimage;

        public PlayerDeadState(Agent agent, AnimParamSO animParam) : base(agent, animParam)
        {
            player = agent as Player;
            afterimage = player.GetCompo<AgentAfterImage>();

            mover = player.GetCompo<PlayerMover>();
        }

        public override void Enter()
        {
            base.Enter();

            afterimage.Stop();

            GameManager.Instance.AllObjectDisable();

            if (!player.InputReader.SlowMode)
            {
                player.InputReader.SetSlowMode(true);
            }
            player.InputReader.Enable(false);

            mover.StopImmediately();

            player.StartCoroutine(DeadRoutine());
        }

        private IEnumerator DeadRoutine()
        {
            yield return new WaitForSeconds(0.2f);

            float elapsedTime = 0f;
            float time = 2f;
            float shakeAmount = 0.01f;
            float maxShakeSpeed = 100;

            Vector3 originalPos = player.transform.position;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                float offsetX = Mathf.Sin(elapsedTime * maxShakeSpeed) * shakeAmount;
                float offsetY = Mathf.Cos(elapsedTime * maxShakeSpeed) * shakeAmount;
                player.transform.position = originalPos + new Vector3(offsetX, 0, offsetY);
                yield return null;
            }

            player.transform.position = originalPos;

            yield return new WaitForSeconds(0.5f);

            EffectPlayer effect = PoolingManager.Instance.Pop("PlayerDeadEffect") as EffectPlayer;

            effect.transform.position = player.transform.position;

            player.transform.Find("Visual").GetComponent<SpriteRenderer>().enabled = false;

            yield return new WaitForSeconds(1f);

            UIManager.Instance.ShowUI<PlayerDeadCanvas>();
            UIManager.Instance.GetUI<PlayerDeadCanvas>().StartDeadRoutine();
        }
    }
}
