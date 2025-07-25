
using System.Collections;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Bullets;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BulletSpawner", menuName = "SO/Boss/Module/Spawn/CircularSpawner")]
    public class CircularSpawnModule : PatternModule
    {
        [Header("BulletSetting")]
        [SerializeField] private float _startAngle;
        [SerializeField] private float _settingAngle;
        [SerializeField] private float _damage;
        [SerializeField] private float _speed;
        [SerializeField] private Vector2 _scale;

        [Space]
        [Header("SpawnSetting")]
        [SerializeField] private float _spawnCnt;
        [SerializeField] private float _spawnDistance;
        [SerializeField] private float _spawnDelay;

        private const string _bulletName = "ThornBullet";

        public override IEnumerator Execute()
        {
            float startAngle = -_startAngle;
            for (int i = 0; i < _spawnCnt; ++i)
            {
                float angle = startAngle + _settingAngle * i;

                BossBaseBullet bullet = PoolingManager.Instance.Pop(_bulletName) as BossBaseBullet;
                bullet.SetObj(_speed, _damage, _scale);
                bullet.SetCanMove(true);
                bullet.SetAlpha(1);
                bullet.transform.position = BossManager.Instance.Boss.transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
                if (_spawnDistance > 0)
                    bullet.transform.Translate(Vector2.right * _spawnDistance);
            }
            CompleteActionExecute();
            yield break;
        }
    }
}
