using UnityEngine;

namespace YUI.Fields {
    public class AxisMoveObject : MonoBehaviour {
        [SerializeField] private bool isHorizontal;
        [SerializeField] private float speed;

        private void Update() {
            transform.position += (isHorizontal ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1)) * speed * Time.deltaTime;
        }
    }
}
