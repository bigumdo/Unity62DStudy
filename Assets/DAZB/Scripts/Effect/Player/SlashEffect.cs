using System.Collections;
using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Agents.players {
    public class SlashEffect : PoolableMono {
        [SerializeField] private Vector3 startScale;
        [SerializeField] private Vector3 endScale;
        [SerializeField] private float time;
        [SerializeField] private float moveDistance;
        private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void ResetItem() {
            transform.localScale = startScale;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }

        public void StartRoutine() {
            StartCoroutine(Routine());
        }

        private IEnumerator Routine() {
            float elapsedTime = 0;
            float targetTime = time;

            while (elapsedTime < targetTime) {
                transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / targetTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = endScale;

            elapsedTime = 0;

            yield return new WaitForSeconds(0.05f);

            while (elapsedTime < targetTime) {
                transform.position += transform.right * (moveDistance * Time.deltaTime / targetTime);
                spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, elapsedTime / targetTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            PoolingManager.Instance.Push(this);
        }
    }
}
