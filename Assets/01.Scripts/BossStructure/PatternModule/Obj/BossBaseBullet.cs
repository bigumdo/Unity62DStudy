using Unity.VisualScripting;
using UnityEngine;
using YUI.Agents.players;
using YUI.Agents;
using YUI.Bullets;
using YUI.Cores;
using YUI.ObjPooling;
using System.Collections;
using UnityEngine.UIElements;
using System.Drawing;
using Color = UnityEngine.Color;
using YUI.Agents.Bosses;

namespace YUI.PatternModules
{
    public abstract class BossBaseBullet : Bullet
    {

        protected Transform _visual;
        protected Collider2D _collider;
        protected Vector2 _scale;
        protected float _rotationSpeed;
        protected Rigidbody2D _rbCompo;

        protected SpriteRenderer _renderer;

        protected bool _canMove = false;

        protected virtual void Awake()
        {
            _visual = transform.Find("Visual");
            _collider = GetComponent<Collider2D>();
            _rbCompo = GetComponent<Rigidbody2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        public virtual void SetObj(float speed, float damage, Vector2 scale)
        {
            _speed = speed;
            _damage = damage;
            _scale = scale;
            transform.localScale = new Vector3(scale.x, scale.y);
        }

        public virtual void SetCanMove(bool canMove = false)
        {
            transform.parent = null;
            _canMove = canMove;
            _collider.enabled = true;
        }

        protected virtual void FixedUpdate()
        {
            if (!GameManager.Instance.IsGmaeStop && !_canMove)
                return;
            Move();
        }

        protected virtual void Move()
        {
            _rbCompo.linearVelocity = transform.right * _speed;
        }

        #region Base
        public IEnumerator DOMove(float distance, float duration)
        {
            float time = 0;
            Vector3 startValue = transform.position;
            Vector3 endValue = startValue + transform.right * distance;

            while (time < duration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(startValue, endValue, time / duration);
                yield return null;
            }

            transform.position = endValue;
        }
        public IEnumerator DOMove(float distance, float duration, float waitTime)
        {
            float time = 0;
            Vector3 startValue = transform.position;
            Vector3 endValue = startValue + transform.right * distance;

            while (time < duration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(startValue, endValue, time / duration);
                yield return null;
            }

            transform.position = endValue;
            yield return new WaitForSeconds(waitTime);
            time = 0;
            startValue = transform.position;
            endValue = startValue + -transform.right * distance;

            while (time < duration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(startValue, endValue, time / duration);
                yield return null;
            }
        }

        public IEnumerator DOFade(float alpha, float duration)
        {
            float startAlpha = _renderer.color.a;
            float time = 0;
            Color color = _renderer.color;

            while (time < duration)
            {
                time += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, alpha, time / duration);
                _renderer.color = color;
                yield return null;
            }
            color.a = alpha;
            _renderer.color = color;
        }

        public void SetAlpha(float alpha)
        {
            Color color = _renderer.color;
            color.a = alpha;
            _renderer.color = color;
        }
        #endregion
        public override void ResetItem()
        {
            SetAlpha(0);
            _collider.enabled = false;
            _canMove = false;
            _rbCompo.linearVelocity = Vector3.zero;
            transform.rotation = Quaternion.Euler( Vector3.zero);
            base.ResetItem();
        }

        //protected Vector3 TestBezierCurve(Vector3 startPos, Vector3 endPos, float t, float rotation,float distance, float interval)
        //{
        //    Vector3 p1 = startPos

        //    Vector3 M0 = Vector3.Lerp(startPos, P1, t);
        //    Vector3 M1 = Vector3.Lerp(P1, P2, t);
        //    Vector3 M2 = Vector3.Lerp(P2, endPos, t);

        //    Vector3 B0 = Vector3.Lerp(M0, M1, t);
        //    Vector3 B1 = Vector3.Lerp(M1, M2, t);

        //    return Vector3.Lerp(B0, B1, t);
        //}

        //protected Vector3 TestBezierCurve(Vector3 startPos, Vector3 endPos, float t, float rotation, float startInterval, float endInterval)
        //{
        //    yield return Vector3.zero;
        //}

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player player))
            {
                CameraManager.Instance.ShakeCamera(2f, 2, 0.15f);
                player.GetCompo<AgentHealth>(true).ApplyDamage(_damage);
                PoolingManager.Instance.Push(this, true);
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("BulletWall"))
                PoolingManager.Instance.Push(this, true);
        }
    }
}
