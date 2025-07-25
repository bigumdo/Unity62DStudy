using UnityEngine;

namespace YUI
{
    public class TorchArea : MonoBehaviour
    {
        [SerializeField] private GameObject _leftTorch;
        [SerializeField] private GameObject _rightTorch;
        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _leftTorch.SetActive(false);
            _rightTorch.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _leftTorch.SetActive(true);
            _rightTorch.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _leftTorch.SetActive(false);
            _rightTorch.SetActive(false);
        }
    }
}
