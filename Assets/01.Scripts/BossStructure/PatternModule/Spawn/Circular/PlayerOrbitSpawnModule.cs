using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "PlayerOrbitSpawnModule", menuName = "SO/Boss/Module/Spawn/PlayerOrbitSpawnModule")]
    public class PlayerOrbitSpawnModule : PatternModule
    {
        [Header("BulletSetting")]
        [Tooltip("처음 스폰되는 시작각도")]
        [SerializeField] protected float _startAngle;
        [Tooltip("소환되는 총알 각도")]
        [SerializeField] protected float _settingAngle;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _speed;
        [SerializeField] protected Vector2 _scale;
        [Tooltip("회전속도")]
        [SerializeField] private float _rotationSpeed;

        [Space]
        [Header("SpawnSetting")]
        [SerializeField] protected float _spawnCnt;
        [Tooltip("총알 스폰 위치가 보스에서 얼마나 떯어져서 소환할 것인가")]
        [SerializeField] private float _spawnDistance;
        [Tooltip("소환되고 보여지기까지 시간")]
        [SerializeField] private float _spawnFadeTime;
        [Tooltip("소환되는 총알 스폰 시간간격")]
        [SerializeField] private float _spawnDelay;


        [Space]
        [Header("Setting")]
        [Tooltip("속도를 얼마나 지나고 바꿀 것인가")]
        [SerializeField] private float _changeTime;
        [Tooltip("바꿀 속도")]
        [SerializeField] private float _changeRotationSpeed;

        [Space]
        [Header("EnhanceSetting")]
        [Tooltip("패턴을 강호할 것인가")]
        [SerializeField] private bool _shouldEnhance;
        [Tooltip("소환되는 위치를 얼마나 앞에 소환할 것인가")]
        [SerializeField] private float _spawnOffsetLength;

        private const string _bulletName = "ThornRotationBullet";

        public override IEnumerator Execute()
        {
            List<CenterRotationBullet> bullets = new List<CenterRotationBullet>();
            float startAngle = -_startAngle;
            Vector3 playerPos;
            Player player = PlayerManager.Instance.Player;
            if (_shouldEnhance)
            {
                Vector3 playerDir = Vector3.zero;
                switch (player.InputReader.Movement)
                {
                    case PlayerMoveDir.UP:
                        playerDir = Vector3.up;
                        break;
                    case PlayerMoveDir.DOWN:
                        playerDir = Vector3.down;
                        break;
                    case PlayerMoveDir.RIGHT:
                        playerDir = Vector3.right;
                        break;
                    case PlayerMoveDir.LEFT:
                        playerDir = Vector3.left;
                        break;
                }
                playerPos = playerDir * _spawnOffsetLength + player.transform.position;
            }
            else
                playerPos = player.transform.position;

            for (int i = 0; i < _spawnCnt; ++i)
            {
                float angle = startAngle + _settingAngle * i;
                CenterRotationBullet bullet = PoolingManager.Instance.Pop(_bulletName) as CenterRotationBullet;
                bullet.transform.position = playerPos;
                bullet.transform.Rotate(new Vector3(0,0, angle));
                if (_spawnDistance > 0)
                {
                    bullet.transform.Translate(Vector3.right * _spawnDistance);
                }
                bullet.SetObj(_speed, _damage, _scale, _rotationSpeed, _changeRotationSpeed,_changeTime, _spawnDistance, playerPos);
                bullets.Add(bullet);
            }

            SoundManager.Instance.PlaySound("SFX_Boss_PlayerOrbitSpawn");
            foreach (CenterRotationBullet b in bullets)
            {
                _boss.StartCoroutine(BulletActive(b));
            }

            CompleteActionExecute();
            yield break;
        }

        private IEnumerator BulletActive(CenterRotationBullet b)
        {
            yield return b.DOFade(1, _spawnFadeTime);
            b.SetCanMove(true);
        }
    }
}
