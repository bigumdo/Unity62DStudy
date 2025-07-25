using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "SymmetricBulletSpawnModule", menuName = "SO/Boss/Module/Spawn/SymmetricSpawnModule")]
    public class SymmetricCircularBulletSpawnModule : PatternModule
    {
        [Space]
        [Header("BulletSetting")]
        [SerializeField] private float _speed;
        [SerializeField] private float _damage;
        [SerializeField] private Vector2 _scale;

        [Space]
        [Header("SpawnSetting")]
        [SerializeField] private float _spawnCnt;

        [Space]
        [Header("ShootingSetting")]
        [Tooltip("발사 딜레이")]
        [SerializeField] private float _shootDelay;
        [Tooltip("모든 총알이 발사되는 시간")]
        [SerializeField] private float _shootDuration;
        [Tooltip("안전 거리를 몇도로 할 것인가")]
        [SerializeField] private float _sefetyRange;

        [Space]
        [Header("MoveSetting")]
        [Tooltip("이동 후 발사 할 것인가")]
        [SerializeField] protected bool _moveBeforeFire;
        [Tooltip("이동시간이 얼마나 되는가")]
        [SerializeField] protected float _moveDuration;

        private Vector3 _counterDir;
        private List<BossBaseBullet> _bullets;

        private const string _bulletName = "ThornBullet";

        public override IEnumerator Execute()
        {
            _bullets = new List<BossBaseBullet>();
            _counterDir = -BossManager.Instance.counterDir;
            yield return _boss.StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            float fireDelay = _shootDuration / _spawnCnt;
            for (int i = 0; i < _spawnCnt; ++i)
            {
                BossBaseBullet leftBullet = PoolingManager.Instance.Pop(_bulletName) as BossBaseBullet;
                BossBaseBullet rightBullet = PoolingManager.Instance.Pop(_bulletName) as BossBaseBullet;
                leftBullet.transform.parent = _boss.transform;
                leftBullet.transform.position = _boss.transform.position;
                leftBullet.SetObj(_speed, _damage, _scale);
                leftBullet.SetAlpha(1);
                rightBullet.transform.parent = _boss.transform;
                rightBullet.transform.position = _boss.transform.position;
                rightBullet.SetObj(_speed, _damage, _scale);
                rightBullet.SetAlpha(1);
                float angle = Angle(true, _spawnCnt, i);
                float angle2 = Angle(false, _spawnCnt, i);
                leftBullet.transform.Rotate(new Vector3(0, 0, angle));
                rightBullet.transform.Rotate(new Vector3(0, 0, angle2));
                _bullets.Add(leftBullet);
                _bullets.Add(rightBullet);
            }

            if (_moveBeforeFire)
            {
                CompleteActionExecute();
                yield return new WaitForSeconds(_moveDuration);
            }

            if (_shootDelay > 0)
                yield return new WaitForSeconds(_shootDelay);

            foreach (BossBaseBullet b in _bullets)
            {
                yield return new WaitForSeconds(fireDelay);
                SoundManager.Instance.PlaySound("SFX_Boss_ThornCounterFire");
                b.SetCanMove(true);
            }
            if (!_moveBeforeFire)
                CompleteActionExecute();
        }

        private float Angle(bool clockwise, float spawnCnt, float currentCnt)
        {
            float bossDir = Mathf.Atan2(_counterDir.y, _counterDir.x) * Mathf.Rad2Deg;
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
