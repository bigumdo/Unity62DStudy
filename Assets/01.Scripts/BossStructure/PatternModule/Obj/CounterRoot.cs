using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.ObjPooling;
using static UnityEngine.Rendering.DebugUI;
using YUI.StatusEffects;
using YUI.Cores;

namespace YUI.PatternModules
{
    public class CounterRoot : PoolableMono
    {
        [SerializeField] private Color _baseColor;
        [SerializeField] private Color _delColor;
        [SerializeField] private Color _constColor;
        [SerializeField] private Color _rangeDisplayColor;

        private float _rangeDisplayTime;
        private float _attackDelayTime;
        private float _attackTime;
        private float _damage;
        private float _length;
        private bool _isLocked;
        private bool _canAttack;
        private bool _canBossCheck;

        private SpriteRenderer _renderer;

        private const float c1 = 1.70158f;
        private const float c3 = c1 + 1;

        private void Awake()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable()
        {
            BossManager.Instance.OnCounterSuccessEvent += CounterSuccess;
            BossManager.Instance.OnCounterFailEvent += CounterFail;
        }

        private void OnDisable()
        {
            ActionClose();
        }

        private void ActionClose()
        {
            BossManager.Instance.OnCounterSuccessEvent -= CounterSuccess;
            BossManager.Instance.OnCounterFailEvent -= CounterFail;
        }

        public void SetData(Vector3 pos,float rangeDisplayTime, float attackDelayTime,float attackTime, float length, float damage)
        {
            transform.position = pos;
            _rangeDisplayTime = rangeDisplayTime;
            _attackDelayTime = attackDelayTime;
            _attackTime = attackTime;
            _length = length;
            _damage = damage;
            _renderer.color = _rangeDisplayColor;
            transform.localScale = new Vector3(_length, 1, 1);
        }

        public override void ResetItem()
        {
            _canAttack = false;
            _renderer.color = _rangeDisplayColor;
            transform.localScale = new Vector3(_length, 1, 1);
            _canAttack = false;
            _canBossCheck = false;
        }

        public IEnumerator Excute()
        {
            SoundManager.Instance.PlaySound("SFX_Boss_RootWarning");
            float rotationTime = 0;

            while(rotationTime <= _rangeDisplayTime)
            {
                rotationTime += Time.deltaTime;
                transform.right = (PlayerManager.Instance.Player.transform.position - transform.position).normalized;
                yield return null;
            }

            yield return new WaitForSeconds(_attackDelayTime);
            transform.localScale = new Vector3(0, 1, 1);
            _renderer.color = _baseColor;

            _canAttack = true;
            _canBossCheck = true;

            float time = 0;
            SoundManager.Instance.PlaySound("SFX_Boss_RootStart");
            while (time <= _attackTime)
            {
                time += Time.deltaTime;
                float t = time / _attackTime;
                float length = Mathf.Lerp(transform.localScale.x, _length, c3 * t * t * t - c1 * t * t);
                transform.localScale = new Vector3(length,1,1);

                yield return null;
            }
            _canAttack = false;
            SoundManager.Instance.PlaySound("SFX_Boss_RootEnd");
        }

        private void SetCounterDir()
        {
            if (transform.position.x > BossManager.Instance.bossTrm.position.x)
            {
                BossManager.Instance.counterDir = BossManager.Instance.bossTrm.right;
            }
            else
                BossManager.Instance.counterDir = -BossManager.Instance.bossTrm.right;
        }
            
        private void CounterSuccess() => StartCoroutine(CounterSuccessAnimation());

        private IEnumerator CounterSuccessAnimation()
        {
            _renderer.color = _delColor;
            yield return SetFade(0, 1, 0.1f);
            yield return SetFade(1, 0, 0.3f);
            BossManager.Instance.counterObjList.Remove(this);
            PoolingManager.Instance.Push(this);
        }

        private void CounterFail() => StartCoroutine(CounterFailAnimation());

        private IEnumerator CounterFailAnimation()
        {
            CounterRoot obj = Instantiate(this,null);
            obj.transform.position = transform.position;
            obj.ActionClose();
            obj._renderer.sortingOrder = 10;
            _isLocked = true;
            _renderer.color = _constColor;
            obj._renderer.color = Color.white;
            SoundManager.Instance.PlaySound("SFX_Boss_RootFixed");
            StartCoroutine(obj.SetFade(0, 1, 0.1f));
            yield return obj.SetSize(1.2f, 0.2f);
            yield return obj.SetFade(1, 0, 0.3f);
            Destroy(obj);
            yield return null;
            ActionClose();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_canAttack)
            {
                if (collision.gameObject.TryGetComponent(out Player player))
                {
                    _canAttack = false;
                    player.GetCompo<PlayerHealth>().ApplyDamage(_damage);
                }
            }
            if (_canBossCheck)
            {
                if (collision.gameObject.TryGetComponent(out Boss boss))
                {
                    SetCounterDir();
                    _canBossCheck = false;
                }
            }
            if (_isLocked)
            {
                StatusEffectManager.Instance.AddStatusEffect(StatusEffectType.Slow, 2, 0.5f);
            }
        }

        private IEnumerator SetFade(float startAlpha,float setAlpha, float duration)
        {
            float time = 0;
            float currentAlpha = _renderer.color.a;
            Color color = _renderer.color;
            color.a = startAlpha;
            _renderer.color = color;

            while(time <= duration)
            {
                time += Time.deltaTime;

                color.a = Mathf.Lerp(currentAlpha, setAlpha, time / duration);
                _renderer.color = color;
                yield return null;
            }

            _renderer.color = color;
        }

        private IEnumerator SetSize(float setSize, float duration)
        {
            float startSize = transform.localScale.y;
            float targetSize = transform.localScale.y * setSize;
            float time = 0;

            while(time <= duration)
            {
                time += Time.deltaTime;
                transform.localScale =  new Vector3(transform.localScale.x,Mathf.Lerp(startSize, targetSize, time / duration),transform.localScale.z);
                yield return null;
            }

            transform.localScale = new Vector3(transform.localScale.x, targetSize, transform.localScale.z);
        }
    }
}
