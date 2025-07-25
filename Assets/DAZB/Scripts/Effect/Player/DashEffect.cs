using System.Collections;
using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Agents.players {
    public class DashEffect : PoolableMono {
        private Vector3 startPos;
        private Vector3 endPos;

        private LineRenderer lineRenderer;


        public override void ResetItem() {
            if (lineRenderer == null) {
                lineRenderer = GetComponent<LineRenderer>();
            }
        }

        public void StartRoutine(float time) {
            
            StartCoroutine(Routine(time));
        }

        public void SetStartPos(Vector3 startPos) {
            this.startPos = startPos;
        }

        public void SetEndPos(Vector3 endPos) {
            this.endPos = endPos;
        }

        private IEnumerator Routine(float time) {
            float elapsedTime = 0;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);

            while (elapsedTime < time) {
                float t = elapsedTime / time;
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);
                lineRenderer.SetPosition(1, pos);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            lineRenderer.SetPosition(1, endPos);

            elapsedTime = 0;

            yield return new WaitForSeconds(0.05f);

            time /= 2;

            while (elapsedTime < time) {
                float t = elapsedTime / time;
                Vector3 pos = Vector3.Lerp(startPos, endPos, t);
                lineRenderer.SetPosition(0, pos);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            lineRenderer.SetPosition(0, endPos);

            lineRenderer.positionCount = 0;

            PoolingManager.Instance.Push(this, true);
        }
    }
}
