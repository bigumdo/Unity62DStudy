using UnityEngine;

namespace YUI.Fields {
    public class RotateObject : MonoBehaviour {
        [SerializeField] private float rotationSpeed;

        private void Update() {
            transform.rotation = Quaternion.Euler(new Vector3(0, Time.time * rotationSpeed, 0));
        }
    }
}
