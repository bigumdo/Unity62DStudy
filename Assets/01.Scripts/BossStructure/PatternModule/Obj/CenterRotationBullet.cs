using System.Collections;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using YUI.Agents;
using YUI.Agents.players;
using YUI.Bullets;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    public class CenterRotationBullet : BossBaseBullet
    {
        private Vector3 _playerPos;
        private float _radius;
        private float _changRotationSpeed;
        private float _angle;
        private float _changeTime;
        private bool _canAttack;

        public void SetObj(float speed, float damage, Vector2 scale, float startRotationSpeed, float changeRotationSpeed,float changeTime, float radius,Vector3 playerPos)
        {
            _speed = speed;
            _damage = damage;
            _scale = scale;
            transform.localScale = new Vector3(scale.x, scale.y, 1);

            _playerPos = playerPos;
            _radius = radius;
            _rotationSpeed = startRotationSpeed;
            _changRotationSpeed = changeRotationSpeed;
            _changeTime = changeTime;
            _angle = transform.localRotation.eulerAngles.z;
        }

        public override void SetCanMove(bool canMove = false)
        {
            base.SetCanMove(canMove);
            ChangeRotationSpeed(_changRotationSpeed, _changeTime);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void Move()
        {
            transform.right = (_playerPos - transform.position).normalized;
            _angle += (_rotationSpeed) * Time.deltaTime;
            float rad = _angle * Mathf.Deg2Rad;
            float x = _playerPos.x + Mathf.Cos(rad) * _radius;
            float y = _playerPos.y + Mathf.Sin(rad) * _radius;
            transform.position = new Vector3(x, y, 0);
            _radius -= _speed * Time.deltaTime;
            if (_radius <= 0)
                PoolingManager.Instance.Push(this, true);
        }

        private void ChangeRotationSpeed(float changeSpeed, float time)
        {
            StartCoroutine(ChangeRotationSpeedCoroutine(changeSpeed, time));
        }

        private IEnumerator ChangeRotationSpeedCoroutine(float targetSpeed, float duration)
        {
            float startSpeed = _rotationSpeed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                _rotationSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _rotationSpeed = targetSpeed;
        }

        public override void ResetItem()
        {
            Color color = _renderer.color;
            color.a = 0;
            _renderer.color = color;
            _canAttack = true;
            base.ResetItem();
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            //base.OnTriggerEnter2D(collision);
            if (collision.TryGetComponent(out Player player) && _canAttack)
            {
                _canAttack = false;
                CameraManager.Instance.ShakeCamera(2f, 2, 0.15f);
                player.GetCompo<AgentHealth>(true).ApplyDamage(_damage);
            }
        }
    }
}
