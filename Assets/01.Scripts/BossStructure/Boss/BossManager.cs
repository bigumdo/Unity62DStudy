using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Agents.players;
using YUI.Cores;
using YUI.ObjPooling;
using YUI.PatternModules;
using YUI.Rooms;
using YUI.UI.CompleteUI;

namespace YUI.Agents.Bosses
{
    public class BossManager : MonoSingleton<BossManager>
    {
        public Boss Boss => _boss;
        private Boss _boss;
        public Transform bossTrm => _boss.transform;

        public event Action OnCounterAttackEvent;
        public event Action OnCounterSuccessEvent;
        public event Action OnCounterFailEvent;

        [HideInInspector] public bool canCounter;
        [HideInInspector] public Vector3 counterDir = Vector3.zero;
        [HideInInspector] public List<CounterRoot> counterObjList;

        public BossMover Mover { get; private set; }

        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private GameObject _reward;
        [HideInInspector] public Vector3 startPos;

        public void SetBoss(Boss boss)
        {
            _boss = boss;
            Mover = boss.GetCompo<BossMover>();
            PatternManager.Instance.PattternSetting(_boss.patternDataSO);
            counterObjList = new List<CounterRoot>();
            startPos = boss.centerPos;
        }

        public void SetTorialBoss(TutorialBoss boss)
        {
            _boss = boss;
            Mover = boss.GetCompo<BossMover>();
            TutorialBossManger.Instance.PattternSetting(boss.patternSO);
            counterObjList = new List<CounterRoot>();
            startPos = boss.centerPos;
        }

        public void StartBT() => _boss.StartBT();
        public void StopBT() => _boss.StopBT();

        public void Success()
        {
            if (OnCounterSuccessEvent != null)
            {
                OnCounterSuccessEvent?.Invoke();
            }
        }

        public void Fail()
        {
            if (OnCounterFailEvent != null)
                OnCounterFailEvent?.Invoke();
        }

        public void Counter()
        {
            if (canCounter)
            {
                OnCounterAttackEvent?.Invoke();
            }
        }
        public void BossDeadPlay()
        {
            _boss.transform.position = startPos;
            _boss.StopBT();
            transform.position = _boss.transform.position;
            StartCoroutine(BossDeadEffectPlay());
            PlayerManager.Instance.Player.InputReader.SetSlowMode(true);
            PlayerManager.Instance.Player.InputReader.Enable(false);

        }

        private IEnumerator BossDeadEffectPlay()
        {
            PoolableMono[] poolables = GameObject.FindObjectsByType<PoolableMono>(FindObjectsSortMode.None);
            foreach (PoolableMono p in poolables)
            {
                PoolingManager.Instance.Push(p);
            }
            _particle.transform.position = _boss.transform.position;
            _particle.gameObject.SetActive(true);
            _particle.Play();
            CameraManager.Instance.ShakeCamera(1, 8, 0.2f);
            yield return new WaitForSeconds(0.6f);
            CameraManager.Instance.ShakeCamera(1, 8, 0.2f);
            yield return new WaitForSeconds(0.4f);
            CameraManager.Instance.ShakeCamera(1, 8, 0.2f);
            yield return new WaitForSeconds(0.3f);
            CameraManager.Instance.ShakeCamera(1, 8, 0.2f);
            yield return new WaitForSeconds(0.1f);
            CameraManager.Instance.ShakeCamera(8, 10, 1.2f);
            yield return new WaitForSeconds(1.1f);
            CameraManager.Instance.SetFadeColor(Color.white);
            _particle.gameObject.SetActive(false);
            CameraManager.Instance.FadeOut(0);
            if (_boss is Griv)
            {
                (GameManager.Instance.GetCurrentRoom() as BossRoom).ActiveTileMap(true, "ROOM");
                (GameManager.Instance.GetCurrentRoom() as BossRoom).ActiveTileMap(false, "BATTLE");
                PlayerManager.Instance.Player.GetCompo<PlayerMover>().SetMoveType(PlayerMoveType.NORMAL);
                PlayerManager.Instance.Player.transform.position = Vector3.zero;
                PlayerManager.Instance.Player.GetCompo<PlayerHealth>().PlayerHeal(100000);
                UIManager.Instance.GetUI<CompleteCanvas>().Open();
            }
            foreach (CounterRoot obj in counterObjList)
            {
                PoolingManager.Instance.Push(obj,true);
            }
            //Instantiate(_reward,startPos,Quaternion.Euler(90,0,0));
            GameManager.Instance.IsBattle = false;
            Destroy(_boss.gameObject);
            yield return new WaitForSeconds(1f);
            CameraManager.Instance.FadeIn(1f, () => CameraManager.Instance.SetFadeColor(Color.black));
            yield return new WaitForSeconds(1f);
            PlayerManager.Instance.Player.InputReader.SetSlowMode(true);
            UIManager.Instance.SetBossGaugeVisibility(false);

            _boss = null;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
                _boss.GetCompo<BossHealth>().ApplyDamage(1000000);
        }
#endif
    }
}
