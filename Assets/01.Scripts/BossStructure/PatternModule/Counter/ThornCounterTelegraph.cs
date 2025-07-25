using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Bullets;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "ThornCounterTelegraph", menuName = "SO/Boss/Module/Counter/ThornCounterTelegraph")]
    public class ThornCounterTelegraph : PatternModule
    {
        [Header("TelegraphSetting")]
        [SerializeField] private float _bulletSpawnCnt;
        [SerializeField] private Vector2 _bulletScale;
        [SerializeField] private float _spawnDelay;
        [SerializeField] private float _spawnDistance;
        [SerializeField] private float _moveDuration;
        [SerializeField] private float _moveDelay;
        [SerializeField] private float _sefetyRange;

        private const string _bulletName = "ThornBullet";

        private Vector3 _sefetyDir;
        private List<BossBaseBullet> _bullets;
        public override IEnumerator Execute()
        {
            _bullets = new List<BossBaseBullet>();
            _sefetyDir = -BossManager.Instance.counterDir;
            yield return Spawn(true);
            yield return Spawn(false);
            CompleteActionExecute();
        }

        private IEnumerator Spawn(bool clockwise)
        {
            for (int i = 0; i < _bulletSpawnCnt; ++i)
            {
                BossBaseBullet bullet = PoolingManager.Instance.Pop(_bulletName) as BossBaseBullet;
                float angle = Angle(clockwise, _bulletSpawnCnt, i);
                _bullets.Add(bullet);
                bullet.transform.position = _boss.transform.position;
                bullet.transform.Rotate(new Vector3(0, 0, angle));
                bullet.SetObj(0, 0, _bulletScale);
                bullet.SetAlpha(1);
                SoundManager.Instance.PlaySound("SFX_Boss_ThornCounterWarning");
                _boss.StartCoroutine(bullet.DOMove(_spawnDistance, _moveDuration, _moveDelay));
                yield return new WaitForSeconds(_spawnDelay);
            }
            yield return new WaitForSeconds(_moveDuration + _moveDelay);
        }

        protected override void CompleteActionExecute()
        {
            _bullets.ForEach(x => PoolingManager.Instance.Push(x));
            _bullets.Clear();
            base.CompleteActionExecute();
        }

        private float Angle(bool clockwise, float spawnCnt, float currentCnt)
        {
            float bossDir = Mathf.Atan2(_sefetyDir.y, _sefetyDir.x) * Mathf.Rad2Deg;
            float clockwiseStartAngle = bossDir + _sefetyRange * 0.5f;
            float counterclockwiseStartAngle = bossDir - _sefetyRange * 0.5f;
            float angle = 0;

            if (clockwise)
                angle = Mathf.Lerp(clockwiseStartAngle - 360, counterclockwiseStartAngle
                    , currentCnt / (spawnCnt - 1));
            else
                angle = Mathf.Lerp(counterclockwiseStartAngle, clockwiseStartAngle - 360
                    , currentCnt / (spawnCnt - 1));

            return angle;
        }
    }
}
