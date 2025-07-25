using System.Collections.Generic;
using UnityEngine;

namespace YUI.Agents.players {
    public class PlayerOrbiter : MonoBehaviour, IAgentComponent {
        [SerializeField] private float maxOrbiterCount;
        [SerializeField] private float orbitSpeed;
        [SerializeField] private float radius;

        private Player player;
        private List<Transform> orbiters = new List<Transform>();

        private float currentOrbiterCount = 0;

        private bool isFixed;

        public PlayerMoveDir playerMoveDir;

        public void Initialize(Agent agent) {
            player = agent as Player;

            for (int i = 0; i < maxOrbiterCount; ++i) {
                GameObject go = new GameObject();
                orbiters.Add(go.transform);
            }
        }

        private void Update() {
            if (isFixed) {
                SetAngle(playerMoveDir);
                return;
            }

            float time = Time.time;
            for (int i = 0; i < orbiters.Count; ++i)
            {
                if (i < currentOrbiterCount)
                {
                    float angle = time * orbitSpeed + i * Mathf.PI * 2 / currentOrbiterCount;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                    Vector3 targetPos = player.transform.position + offset;
                    orbiters[i].position = Vector3.Lerp(orbiters[i].position, targetPos, Time.deltaTime * 8f);
                }
                else
                {
                    orbiters[i].position = player.transform.position;
                }
            }
        }

        public List<Transform> GetOrbiters() => orbiters;

        public void SetOrbiterCount(float count) {
            currentOrbiterCount = Mathf.Clamp(count, 0, maxOrbiterCount);
        }

        private void SetAngle(PlayerMoveDir playerMoveDir) {
            float angle = 0;

            switch (playerMoveDir) {
                case PlayerMoveDir.UP:
                    angle = 90;
                    break;
                case PlayerMoveDir.DOWN:
                    angle = -90;
                    break;
                case PlayerMoveDir.RIGHT:
                    angle = 0;
                    break;
                case PlayerMoveDir.LEFT:
                    angle = -180;
                    break;
            }

            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;

            orbiters[0].position = player.transform.position + offset;

            orbiters[0].rotation = Quaternion.Euler(-90, 0, -angle - 90);
        }

        public void SetPlayerMoveDir(PlayerMoveDir playerMoveDir) => this.playerMoveDir = playerMoveDir;

        public void SetFixed(bool value) => isFixed = value;
    }
}
