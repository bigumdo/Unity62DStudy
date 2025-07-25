using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BulletFireModule", menuName = "SO/Boss/Module/Spawn/BulletFireModule")]
    public class BulletFireModule : PatternModule
    {
        [Header("BulletSetting")]
        [Tooltip("������ ���۵Ǵ� ���� ����")]
        [SerializeField] protected float _startAngle;
        [Tooltip("�ѹ� �߻��ϴ� �Ѿ� ���� ����")]
        [SerializeField] protected float _settingAngle;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _speed;
        [SerializeField] protected Vector2 _scale;

        [Space]
        [Header("SpawnSetting")]
        [SerializeField] protected float _spawnCnt;
        [Tooltip("Bullet�� �����κ��� �󸶳� �ָ� ��ȯ���� ����")]
        [SerializeField] protected float _spawnDistance;
        [Tooltip("Distance��ŭ �̵��ϴµ� �ɸ��� �ð� ����")]
        [SerializeField] protected float _spawnMoveTime;
        [Tooltip("��ġ�� �̵� �� Fade�Ǵ� �ð��� ����")]
        [SerializeField] protected float _spawnFadeTime;
        [Tooltip("�� ��ȯ�Ǵµ� �ɸ��� ������ ����")]
        [SerializeField] protected float _spawnDelay;


        [Space]
        [Header("ShootingSetting")]
        [Tooltip("ó�� �߻��ϴµ� �ɸ��� Delay ����")]
        [SerializeField] protected float _shootStartDelay;
        [Tooltip("ó�� �߻� ���� �߻��ϴµ� �ɸ��� �ð� ����")]
        [SerializeField] protected float _shootCooldown;
        [Tooltip("�ѹ��� �߻��ϴ� �Ѿ� ���� ����")]
        [SerializeField] protected float _shootCnt;

        [Space]
        [Header("MoveSetting")]
        [Tooltip("������ �̵� �� �߻縦 ���� ����")]
        [SerializeField] protected bool _moveBeforeFire;
        [Tooltip("������ �̵��ϴµ� �ɸ��� �ð� ����")]
        [SerializeField] protected float _moveDuration;

        private const string _bulletName = "ThornBullet";

        public override IEnumerator Execute()
        {
            List<BossBaseBullet> bullets = new List<BossBaseBullet>();
            Vector3 playerPos = PlayerManager.Instance.Player.transform.position;
            for (int i =0;i<_spawnCnt; ++i)
            {

                float angle = -_startAngle + _settingAngle * (i % _shootCnt);
                BossBaseBullet bullet = PoolingManager.Instance.Pop(_bulletName) as BossBaseBullet;
                bullet.transform.position = BossManager.Instance.bossTrm.position;
                Vector3 dir = (playerPos - bullet.transform.position).normalized;
                bullet.transform.right = dir;
                bullet.transform.Rotate(new Vector3(0,0,angle));
                bullet.SetObj(_speed, _damage, _scale);
                if (i < _shootCnt)
                {
                    yield return bullet.DOMove(_spawnDistance, _spawnMoveTime);
                    SoundManager.Instance.PlaySound("SFX_Boss_BulletFireSpawn");
                    yield return bullet.DOFade(1, _spawnFadeTime);
                }
                else
                {
                    bullet.transform.position += bullet.transform.right * _spawnDistance;
                    bullet.SetAlpha(1);
                }
                bullet.transform.parent = _boss.transform;
                bullets.Add(bullet);
                if(_spawnDelay > 0)
                    yield return new WaitForSeconds(_spawnDelay);
            }

            if (_moveBeforeFire)
            {
                CompleteActionExecute();
                yield return new WaitForSeconds(_moveDuration);
            }

            if(_shootStartDelay > 0)
                yield return new WaitForSeconds(_shootStartDelay);

            SoundManager.Instance.PlaySound("SFX_Boss_BulletFireFire");
            for (int i =0;i< bullets.Count; ++i)
            {
                if (i % _shootCnt == 0 && _shootCnt < _spawnCnt)
                {
                    yield return new WaitForSeconds(_shootCooldown);
                    SoundManager.Instance.PlaySound("SFX_Boss_BulletFireFire");
                }
                bullets[i].SetCanMove(true);
            }

            if (!_moveBeforeFire)
                CompleteActionExecute();
        }
    }
}
