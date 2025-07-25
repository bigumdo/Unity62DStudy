using System;
using System.Collections;
using UnityEngine;
using YUI.Agents.AfterImages;
using YUI.Animators;
using YUI.Cores;
using YUI.Effects;
using YUI.Fields;
using YUI.FSM;
using YUI.ObjPooling;
using YUI.Skills;
using YUI.StatSystem;

namespace YUI.Agents.players {
    public enum PlayerMode {
        NORMAL = 0, RELEASE, OVERLOAD
    }

    public class Player : Agent {
        [field: SerializeField] public InputReader InputReader {get; private set;}

        private PlayerMode _playerMode = PlayerMode.NORMAL;
        public PlayerMode PlayerMode
        {
            get => _playerMode;
            private set
            {
                if (_playerMode != value)
                {
                    _playerMode = value;
                    OnPlayerModeChanged?.Invoke(_playerMode);
                }
            }
        }

        public AgentStateListSO StateList;

        [Space]
        public bool CanMoveAtDash = false;
        public StatSO DashDistanceStat;
        public StatSO DashCooldownStat;
        public StatSO MaxDashCount;
        public StatSO MaxOverloadDashCount;
        public StatSO DashChargingStartTime;
        public StatSO DashChargingEndTime;
        public StatSO ChargingDashDistanceMultiplier;
        public StatSO ChargingDashDamageMultiplier;

        [Space]

        public StatSO AttackCooldownStat;
        public StatSO CounterCooldownStat;
        public StatSO TotalChargingCompleteTimeStat;
        public StatSO IndividualChargingCompleteTimeStat;
        public StatSO ChargingCountStat;
        public StatSO OverloadDecreaseStat;
        public StatSO OverloadIncreaseStat;
        public StatSO AttackStat;
        public StatSO DamageTakenWhenNormalStat;
        public StatSO DamageTakenWhenOverloadedStat;
        public StatSO MoveSpeedStat;
        public StatSO chargingAttackDelayStat;

        private StateMachine stateMachine;

        [Space]

        public float LastDashTime = -Mathf.Infinity;
        public float LastCounterTime = -Mathf.Infinity;
        public float LastAttackTime = -Mathf.Infinity;

        public int GetRemainCooldown(string name)
        {
            switch (name)
            {
                case "DASH":
                    return (int)Mathf.Clamp(LastDashTime + DashCooldownStat.Value - Time.time, 0f, DashCooldownStat.Value);
                case "COUNTER":
                    return (int)Mathf.Clamp(LastCounterTime + CounterCooldownStat.Value - Time.time, 0f, CounterCooldownStat.Value);
                case "SKILL":
                    PlayerSkill playerSkill = GetCompo<PlayerSkill>(true);
                    return (int)Mathf.Clamp(playerSkill.GetMainSkillSO().LastExecutionTime + playerSkill.GetMainSkillSO().Cooldown - Time.time, 0f, playerSkill.GetMainSkillSO().Cooldown);
                default:
                    return 0;
            }
        }

        [Space]

        public AnimParamSO MovementX;
        public AnimParamSO MovementY;
        public AnimParamSO BattleParam;
        public AnimParamSO RunParam;

        private float _currentDashCount;
        public float CurrentDashCount
        {
            get => _currentDashCount;
            private set
            {
                if (Mathf.Approximately(_currentDashCount, value)) return;

                _currentDashCount = value;
                OnDashCountChanged?.Invoke(_currentDashCount);
            }
        }


        public bool IsProtected = false;

        AgentAfterImage agentAfterimage;

        private bool _isCoating;
        public bool IsCoating
        {
            get => _isCoating;
            set
            {
                if (_isCoating != value)
                {
                    _isCoating = value;
                    OnCounterConditionChanged?.Invoke(_playerMode);
                }
            }
        }

        #region UIAction

        public event Action<PlayerMode> OnPlayerModeChanged;
        public event Action<PlayerMode> OnCounterConditionChanged;
        public event Action<float> OnDashCountChanged;
        public event Action OnAppleEat;

        #endregion

        protected override void AfterInitComponents()
        {
            base.AfterInitComponents();

            GetStats();

            CurrentDashCount = MaxDashCount.Value;

            stateMachine = new StateMachine(this, StateList);

            PlayerManager.Instance.SetPlayer(this);

            //FieldManager.Instance.SetPlayerSpawnPoint(transform.position);

            StartCoroutine(RechargeDashRoutine());

            agentAfterimage = GetCompo<AgentAfterImage>(true);

            OnDeadEvent.AddListener(DeadEvent);

            InputReader.Init();
        }

        private void Start() {
            stateMachine.Initialize("RUN");
        }

        private void GetStats()
        {
            AgentStat agentStat = GetCompo<AgentStat>(true);

            DashDistanceStat = agentStat.GetStat(DashDistanceStat);
            DashCooldownStat = agentStat.GetStat(DashCooldownStat);
            MaxDashCount = agentStat.GetStat(MaxDashCount);
            MaxOverloadDashCount = agentStat.GetStat(MaxOverloadDashCount);
            DashChargingStartTime = agentStat.GetStat(DashChargingStartTime);
            DashChargingEndTime = agentStat.GetStat(DashChargingEndTime);
            ChargingDashDistanceMultiplier = agentStat.GetStat(ChargingDashDistanceMultiplier);
            ChargingDashDamageMultiplier = agentStat.GetStat(ChargingDashDamageMultiplier);

            AttackCooldownStat = agentStat.GetStat(AttackCooldownStat);
            TotalChargingCompleteTimeStat = agentStat.GetStat(TotalChargingCompleteTimeStat);
            IndividualChargingCompleteTimeStat = agentStat.GetStat(IndividualChargingCompleteTimeStat);
            ChargingCountStat = agentStat.GetStat(ChargingCountStat);
            OverloadDecreaseStat = agentStat.GetStat(OverloadDecreaseStat);
            OverloadIncreaseStat = agentStat.GetStat(OverloadIncreaseStat);
            AttackStat = agentStat.GetStat(AttackStat);
            DamageTakenWhenNormalStat = agentStat.GetStat(DamageTakenWhenNormalStat);
            DamageTakenWhenOverloadedStat = agentStat.GetStat(DamageTakenWhenOverloadedStat);
            MoveSpeedStat = agentStat.GetStat(MoveSpeedStat);
            chargingAttackDelayStat = agentStat.GetStat(chargingAttackDelayStat);
            CounterCooldownStat = agentStat.GetStat(CounterCooldownStat);
        }

        private void DeadEvent()
        {
            if (CurrentState != "DEAD")
            {
                ChangeState("DEAD");
            }
        }

        private void AddReleaseBuff()
        {
            AgentStat stat = GetCompo<AgentStat>(true);

            stat.AddModifier(AttackStat, "ReleaseBuffAttackStat", 13 - AttackStat.BaseValue);
            stat.AddModifier(ChargingCountStat, "ReleaseBuffChargingCountStat", 4 - ChargingCountStat.BaseValue);
        } 

        private void RemoveReleaseBuff() {
            AgentStat stat = GetCompo<AgentStat>(true);

            stat.RemoveModifier(AttackStat, "ReleaseBuffAttackStat");
            stat.RemoveModifier(ChargingCountStat, "ReleaseBuffChargingCountStat");
        }

        private void AddOverloadDebuff() {
            AgentStat stat = GetCompo<AgentStat>(true);
            CameraManager.Instance.SetPlayerPanicScreen(true);
            stat.AddModifier(MoveSpeedStat, "OverloadDebuffMoveSpeedStat", 3 - MoveSpeedStat.BaseValue);
        }

        private void RemoveOverloadDebuff() {
            AgentStat stat = GetCompo<AgentStat>(true);
            CameraManager.Instance.SetPlayerPanicScreen(false);
            stat.RemoveModifier(MoveSpeedStat, "OverloadDebuffMoveSpeedStat");
        }

        private Coroutine coroutine;

        private void OnEnable() {
            RegisterPlayerOverloadEvents();
        }

        private void OnDisable()
        {
            UnregisterPlayerOverloadEvents();

            OnDeadEvent.RemoveListener(DeadEvent);
        }

        private void RegisterPlayerOverloadEvents() {
            PlayerOverload playerOverload = GetCompo<PlayerOverload>(true);

            playerOverload.ReleaseEvent += HandleReleaseEvent;
            playerOverload.ReleaseEndEvent += HandleReleaseEndEvent;
            playerOverload.OverloadEvent += HandleOverloadEvent;
            playerOverload.OverloadEndEvent += HandleOverloadEndEvent;
        }

        private void UnregisterPlayerOverloadEvents() {
            PlayerOverload playerOverload = GetCompo<PlayerOverload>(true);

            playerOverload.ReleaseEvent -= HandleReleaseEvent;
            playerOverload.ReleaseEndEvent -= HandleReleaseEndEvent;
            playerOverload.OverloadEvent -= HandleOverloadEvent;
            playerOverload.OverloadEndEvent -= HandleOverloadEndEvent;
        }

        private void HandleReleaseEvent() {
            PlayerMode = PlayerMode.RELEASE;

            SoundManager.Instance.PlaySound("SFX_Player_NormalToRelease");

            EffectPlayer effect = PoolingManager.Instance.Pop("ReleaseEffect") as EffectPlayer;
            effect.transform.position = transform.position;

            CameraManager.Instance.ShakeCamera(3f, 6f, 0.1f);

            AddReleaseBuff();
        }

        private void HandleReleaseEndEvent() {
            PlayerMode = PlayerMode.NORMAL;

            SoundManager.Instance.PlaySound("SFX_Player_ReleaseToNormal");

            RemoveReleaseBuff();

            GetCompo<PlayerSkill>().PassiveSkillExecution(SkillExecutionType.Change);
        }

        private void HandleOverloadEvent() {
            PlayerMode = PlayerMode.OVERLOAD;

            SoundManager.Instance.PlaySound("SFX_Player_ReleaseToOverload");

            ChangeState("STUN");
            AddOverloadDebuff();

            coroutine = StartCoroutine(OverloadRoutine());
        }

        private void HandleOverloadEndEvent() {
            PlayerMode = PlayerMode.NORMAL;

            SoundManager.Instance.PlaySound("SFX_Player_OverloadToNormal");

            RemoveOverloadDebuff();
            StopCoroutine(coroutine);
        }

        private void Update() {
            stateMachine.currentState.Update();

            if (CurrentState == "DEAD") return;
            
            if (PlayerMode == PlayerMode.OVERLOAD)
            {
                if (CurrentDashCount > MaxOverloadDashCount.Value)
                {
                    CurrentDashCount = MaxOverloadDashCount.Value;
                }
            }

            if (GetCompo<PlayerMover>().moveType == PlayerMoveType.NORMAL) return;

            if (InputReader.SlowMode)
            {
                if (agentAfterimage.IsPlay)
                {
                    agentAfterimage.Stop();
                }
            }
            else
            {
                if (!agentAfterimage.IsPlay)
                {
                    agentAfterimage.Play();
                }
            }
        }

        public void SetPlayerMode(PlayerMode mode) {
            PlayerMode = mode;
        }

        public void ChangeState(string stateName) {
            stateMachine.ChangeState(stateName);
        }

        public bool CanDash() {
            if (InputReader.Movement == PlayerMoveDir.STAY) return false;
            return CurrentDashCount > 0;
        }

        public bool CanCounter()
        {
            return LastCounterTime + CounterCooldownStat.Value < Time.time;
        }

        public bool CanAttack()
        {
            return LastAttackTime + AttackCooldownStat.Value < Time.time;
        }

        private IEnumerator OverloadRoutine()
        {
            PlayerOverload playerOverload = GetCompo<PlayerOverload>();

            while (PlayerMode == PlayerMode.OVERLOAD)
            {
                playerOverload.AddOverload(-Time.deltaTime * OverloadDecreaseStat.Value, true);

                yield return null;
            }
        }

        public void UseDash() {
            if (CurrentDashCount > 0) {
                CurrentDashCount--;
            }
        }

        public void RechargeDash() {
            if (CurrentDashCount < (PlayerMode == PlayerMode.NORMAL ? MaxDashCount.Value : MaxOverloadDashCount.Value)) {
                CurrentDashCount++;
            }
        }

        private IEnumerator RechargeDashRoutine() {
            while (true) {
                yield return new WaitForSeconds(DashCooldownStat.Value);
                RechargeDash();
            }
        }

        public void AppleEatEvent()
        {
            OnAppleEat?.Invoke();
        }
    }
}
