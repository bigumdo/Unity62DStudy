using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "CounterModule", menuName = "SO/Boss/Module/Counter/CounterModule")]
    public class CounterModule : PatternModule
    {
        [SerializeField] private float _counterDuration;
        [SerializeField] private float _counterAngle;
        [SerializeField] private float _stunDuration;
        [SerializeField] private float _distance;
        [SerializeField] private PatternSO _failModule;

        private Vector3 _counterDir;
        private bool _isStunned;
        private BossCounterHead _counterHead;

        public override void Init(Boss boss)
        {
            base.Init(boss);
            if (_failModule != null)
                _failModule.Init(boss);
            _isStunned = false;
            _counterHead = boss.GetCompo<BossCounterHead>();
        }

        public override IEnumerator Execute()
        {
            BossManager.Instance.OnCounterAttackEvent += CounterCheck;
            BossManager.Instance.OnCounterSuccessEvent += Success;
            _counterDir = BossManager.Instance.counterDir;
            _counterHead.SetSpawnDistance(_distance);
            if (_counterDir == Vector3.zero)
            {
                yield return Fail();
                yield break;
            }
            _counterHead.SetPos(_counterDir);
            SoundManager.Instance.PlaySound("SFX_Boss_CounterHeadAppear");
            yield return _counterHead.Fade(1, 0.05f);
            yield return _boss.GetCompo<BossRenderer>().CounterShader(0.5f);
            BossManager.Instance.canCounter = true;
            yield return new WaitForSeconds(_counterDuration);
            yield return _counterHead.Fade(0, 0.05f);
            if(BossManager.Instance.canCounter)
                yield return Fail();
        }

        private void CounterCheck()
        {
            if (_isStunned)
                return;
            Vector3 playerPos = (PlayerManager.Instance.Player.transform.position - _boss.transform.position).normalized;
            float distance = Vector3.Distance(PlayerManager.Instance.Player.transform.position, _boss.transform.position);
            float dot = Vector3.Dot(_counterDir, playerPos);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle <= _counterAngle * 0.5f && distance <= _distance * 3f)
            {
                _isStunned = true;
                BossManager.Instance.Success();
            }
        }

        protected override void CompleteActionExecute()
        {
            _isStunned = false;
            BossManager.Instance.OnCounterAttackEvent -= CounterCheck;
            BossManager.Instance.OnCounterSuccessEvent -= Success;
            base.CompleteActionExecute();
            BossManager.Instance.counterDir = Vector3.zero;
        }

        #region CounterResult

        public void Success()
        {
            BossManager.Instance.canCounter = false;
            _boss.StartCoroutine(AttackableTime());
            CameraManager.Instance.ShakeCamera(10, 10, 0.1f);
            CameraManager.Instance.ShakeCamera(1, 2, 0.2f);
            _boss.GetCompo<BossRenderer>().CounterHitEffect();
        }

        private IEnumerator AttackableTime()
        {
            yield return _counterHead.Fade(0);
            _boss.StartCoroutine(_boss.GetCompo<BossRenderer>().CounterOff(BossColor.Confusion, 1.3f, _stunDuration));
            float dissolveValue = _boss.GetCompo<BossRenderer>().Renderer.material.GetFloat("_PhaseDissolveValue");
            _boss.StartCoroutine(_boss.GetCompo<BossRenderer>().PhaseDissolve(0));
            if (_stunDuration > 0)
                yield return new WaitForSeconds(_stunDuration);
            yield return _boss.GetCompo<BossRenderer>().SetColor(BossColor.Base);
            _boss.StartCoroutine(_boss.GetCompo<BossRenderer>().PhaseDissolve(dissolveValue));
            CompleteActionExecute();
        }

        private IEnumerator Fail()
        {
            BossManager.Instance.canCounter = false;
            BossManager.Instance.Fail();
            if(_failModule != null)
            {
                yield return _failModule.Execute(_boss);
            }
            yield return _boss.GetCompo<BossRenderer>().CounterOff(BossColor.Base);
            CompleteActionExecute();
        }
        #endregion
    }
}
