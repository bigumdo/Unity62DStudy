using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using YUI.Agents.players;

namespace YUI.PatternModules
{
    public class RotationBullet : BossBaseBullet
    {
        private float _dirDurationTime;
        private bool _canRotation;

        public void SetObj(float speed, float damage, Vector2 scale, float rotationSpeed, float dirDurationTime)
        {
            _speed = speed;
            _damage = damage;
            _scale = scale;
            transform.localScale = new Vector3(scale.x, scale.y);
            _rotationSpeed = rotationSpeed;
            _dirDurationTime = dirDurationTime;
        }

        public override void SetCanMove(bool canMove = false)
        {
            base.SetCanMove(canMove);
            _canRotation = true;
            if (_dirDurationTime > 0)
                StartCoroutine(SetMoveDir(_dirDurationTime));
        }

        protected override void Move()
        {
            if(_canRotation)
            {
                Vector3 dir = (PlayerManager.Instance.Player.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg * -1;
                Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                    _rotationSpeed);
                _rbCompo.linearVelocity = transform.right * _speed;
            }
            else
                _rbCompo.linearVelocity = transform.right * _speed;
        }

        public IEnumerator SetMoveDir(float durationTime)
        {
            if (!gameObject.activeSelf) yield break;
            yield return new WaitForSeconds(durationTime);
            _canRotation = false;
        }
    }
}
