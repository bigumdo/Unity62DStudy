using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Agents
{
    public class CounterEffect : PoolableMono
    {
        [SerializeField] private float _speed;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_renderer.material.GetFloat("_Value") >= 1)
                _renderer.material.SetFloat("_Value", 0);
            _renderer.material.SetFloat("_Value", _renderer.material.GetFloat("_Value") + Time.deltaTime * _speed);
        }

        public override void ResetItem()
        {
            _renderer.material.SetFloat("_Value",0);
        }
    }
}
