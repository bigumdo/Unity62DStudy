using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Cores;
using YUI.ObjPooling;
using YUI.PatternModules;

namespace YUI
{
    [CreateAssetMenu(fileName = "CircularBulletDelaySpawnModule", menuName = "SO/Boss/Module/Spawn/BulletDelaySpawner")]
    public class DelayedCircularSpawnModule : PatternModule
    {
        [Header("BulletSetting")]
        [Tooltip("처음 스폰되는 시작각도")]
        [SerializeField] protected float _startAngle;
        [Tooltip("소환되는 총알 각도")]
        [SerializeField] protected float _settingAngle;
        [SerializeField] private float _damage;
        [Tooltip("총알 속도")]
        [SerializeField] private List<float> _speed;
        [SerializeField] private Vector2 _scale;
        [Tooltip("총알 회전속도")]
        [SerializeField] private float _rotationSpeed;
        [Tooltip("얼마나 지속할 것인지")]
        [SerializeField] private float _duration;

        [Space]
        [Header("SpawnSetting")]
        [SerializeField] private float _spawnCnt;
        [Tooltip("총알 스폰 위치가 보스에서 얼마나 떯어져서 소환할 것인가")]
        [SerializeField] private float _spawnDistance;
        [Tooltip("스폰 위치까지 이동시간")]
        [SerializeField] protected float _spawnMoveTime;
        [Tooltip("소환되고 보여지기까지 시간")]
        [SerializeField] private float _spawnFadeTime;
        [Tooltip("소환되는 총알 스폰 시간간격")]
        [SerializeField] private float _spawnDelay;

        [Space]
        [Header("DelaySetting")]
        [Tooltip("전부 스폰되고 발사를 시작하는데 걸리는시간")]
        [SerializeField] private float _shootStartDelay;
        [Tooltip("발사 딜레이를 랜덤으로 할 것인가")]
        [SerializeField] private bool _isRandomDelay;
        [Min(0)]
        [SerializeField] private float _randomMinDelay;
        [SerializeField] private float _randomMaxDelay;
        [Tooltip("발사 딜레이")]
        [SerializeField] private float _shootDelay;

        [Space]
        [Header("MoveSetting")]
        [Tooltip("이동 후 발사 할 것인가")]
        [SerializeField] protected bool _moveBeforeFire;
        [Tooltip("이동시간이 얼마나 되는가")]
        [SerializeField] protected float _moveDuration;

        private List<BossBaseBullet> bullets;

        private const string _bulletName = "ThornBullet";

        public override IEnumerator Execute()
        {
            bullets = new List<BossBaseBullet>();
            float startAngle = -_startAngle;
            for (int i = 0; i < _spawnCnt; ++i)
            {
                float angle = startAngle + _settingAngle * i;

                RotationBullet bullet = PoolingManager.Instance.Pop(_bulletName) as RotationBullet;
                bullet.SetObj(_speed[i % _speed.Count], _damage, _scale, _rotationSpeed, _duration);
                bullet.transform.position = BossManager.Instance.Boss.transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                bullet.transform.parent = _boss.transform;
                if (_spawnDistance > 0)
                    yield return bullet.DOMove(_spawnDistance, _spawnMoveTime);
                SoundManager.Instance.PlaySound("SFX_Boss_CircularBulletSpawn");
                yield return bullet.DOFade(1,_spawnFadeTime);
                bullets.Add(bullet);
            }

            if(_moveBeforeFire)
            {
                yield return new WaitForSeconds(_spawnMoveTime);
                CompleteActionExecute();
                yield return new WaitForSeconds(_moveDuration);
            }

            if(_shootStartDelay > 0)
                yield return new WaitForSeconds(_shootStartDelay);


            foreach (BossBaseBullet b in bullets)
            {
                if(_isRandomDelay && _randomMinDelay < _randomMaxDelay)
                {
                    float delay = Random.Range(_randomMinDelay, _randomMaxDelay);
                    yield return new WaitForSeconds(delay);
                }
                else if(_shootDelay > 0)
                    yield return new WaitForSeconds(_shootDelay);
                SoundManager.Instance.PlaySound("SFX_Boss_CircularBulletFire");
                b.SetCanMove(true);
            }
            bullets.Clear();

            if (!_moveBeforeFire)
                CompleteActionExecute();

            yield break;
        }
    }
}
