using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;

namespace YUI.Agents.players {
    public class PlayerHeartBeat : MonoBehaviour, IAgentComponent {
        [SerializeField] private List<float> heartBeatIntervals;
        [SerializeField] private float scaledTime;
        [SerializeField] private float targetScale;

        private Player player;
        private SpriteRenderer spriteRenderer;

        public void Initialize(Agent agent) {
            player = agent as Player;

            spriteRenderer = GetComponent<SpriteRenderer>();

            StartCoroutine(Routine());
        }

        private IEnumerator Routine() {
            while (true) {
                yield return new WaitForSeconds(heartBeatIntervals[(int)player.PlayerMode]);

                float duration = 0.1f;
                float elapsed = 0f;
                Color startColor = spriteRenderer.color;
                Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
                Vector3 startScale = transform.localScale;
                Vector3 endScale = Vector3.one * targetScale;

                while (elapsed < duration) {
                    float t = elapsed / duration;
                    spriteRenderer.color = Color.Lerp(startColor, endColor, t);
                    transform.localScale = Vector3.Lerp(startScale, endScale, t);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                spriteRenderer.color = endColor;
                transform.localScale = endScale;

                yield return new WaitForSeconds(scaledTime);

                duration = 0f;
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
                transform.localScale = Vector3.one;
            }
        }
    }
}
