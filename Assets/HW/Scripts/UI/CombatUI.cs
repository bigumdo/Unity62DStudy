using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;

namespace YUI
{
    public class CombatUI : ToolkitUI
    {
        private Player _player;
        private Boss _boss;
        private PlayerHealth _health;
        private VisualElement _bossHealthLeft;
        private VisualElement _bossHealthRight;

        private Coroutine _bossHpCoroutine;
        private float _currentBossHpRatio = 1f;

        private VisualElement _statusPanel;

        private Label _playerCurHealth;
        private Label _playerMaxHealth;
        private Label _playerState;
        private Label _playerCurOverload;
        private Label _playerDashTime;
        private Label _playerCurDash;
        private Label _playerSkill;
        private Label _playerSkillTime;
        private Label _playerCurCounter;
        private Label _playerCurCounterTime;

        private List<Label> _allLabel;
        private List<Label> _playerHealths;
        private List<Label> _playerStates;
        private List<Label> _playerDashs;
        private List<Label> _playerSkills;
        private List<Label> _playerCounters;

        private List<VisualElement> _bossHealths;
        private List<VisualElement> _bossFrames;

        private VisualElement _root;

        private Coroutine _overloadCoroutine;
        private Coroutine _playerHpCoroutine;

        protected override void Awake()
        {
            base.Awake();
            UIManager.Instance.AddUI(this);
            if (visualTreeAsset != null)
            {
                _root = visualTreeAsset.CloneTree();
                _root.style.flexGrow = 1;
                _root.style.position = Position.Absolute;
                _root.style.width = new Length(100, LengthUnit.Percent);
                _root.style.height = new Length(100, LengthUnit.Percent);

                _root.pickingMode = PickingMode.Ignore;
            }
        }


        public override void Close() { }

        public override void Open()
        {
            _boss = BossManager.Instance.Boss;
        }

        public void SetBossGaugeVisibility(bool isVisible)
        {
            if (_root == null) return;
            _root.Q("boss-panel").style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private void Start()
        {
            _player = PlayerManager.Instance.Player;
            _health = _player.GetCompo<PlayerHealth>();

            if (_root != null)
            {
                root.Q("combat-container").Add(_root);

                _bossHealthLeft = _root.Q("left-boss-health");
                _bossHealthRight = _root.Q("right-boss-health");

                _statusPanel = _root.Q("status-panel");

                _playerCurHealth = _root.Q<Label>("cur-health");
                _playerMaxHealth = _root.Q<Label>("max-health");
                _playerState = _root.Q<Label>("cur-state");
                _playerCurOverload = _root.Q<Label>("cur-overload");
                _playerCurDash = _root.Q<Label>("cur-dash");
                _playerDashTime = _root.Q<Label>("dash-time");
                _playerCurCounter = _root.Q<Label>("cur-counter");
                _playerCurCounterTime = _root.Q<Label>("counter-time");
                _playerSkill = _root.Q<Label>("cur-skill");
                _playerSkillTime = _root.Q<Label>("skill-time");

                _playerHealths = _statusPanel.Q("health-panel").Query<Label>(className: "label").ToList();
                _playerStates = _statusPanel.Q("state-panel").Query<Label>(className: "label").ToList();
                _playerDashs = _statusPanel.Q("dash-panel").Query<Label>(className: "label").ToList();
                _playerSkills = _statusPanel.Q("skill").Query<Label>(className: "label").ToList();
                _playerCounters = _statusPanel.Q("counter").Query<Label>(className: "label").ToList();
                _bossHealths = _root.Q("boss-panel").Query(className: "boss-health").ToList();
                _bossFrames = _root.Q("boss-panel").Query(className: "frame").ToList();
                _allLabel = _statusPanel.Query<Label>(className: "label").ToList();
            }

            SetBossGaugeVisibility(false);

            _player.OnPlayerModeChanged += PlayerOverload;
            _player.OnPlayerModeChanged += UpdateCurCounter;
            _player.OnPlayerModeChanged += UpdateCurState;
            _player.OnCounterConditionChanged += UpdateCurCounter;
            _player.OnDashCountChanged += UpdateCurDash;
            _health.PlayerHealthChanged += UpdateCurHealth;
            _player.GetCompo<PlayerOverload>().OverloadChanged += UpdateCurOverload;

            InitializePlayerHealthUI();
            UpdateCurDash();

            Open();
        }

        private void UpdateCurDash(float count)
        {
            _playerCurDash.text = $"{_player.CurrentDashCount}";

            UpdateCurDashUI();
        }

        private void UpdateCurDashUI()
        {
            int curDash = (int)_player.CurrentDashCount;
            int maxDash = (int)_player.MaxDashCount.Value;

            if (_player.PlayerMode == PlayerMode.OVERLOAD) return;

            foreach (Label label in _playerDashs)
            {
                label.RemoveFromClassList("green");
                label.RemoveFromClassList("orange");
                label.RemoveFromClassList("red");

                if (curDash >= maxDash) label.AddToClassList("green");
                else if (curDash >= 1) label.AddToClassList("orange");
                else label.AddToClassList("red");
            }
        }

        private void OnDisable()
        {
            _player.OnPlayerModeChanged -= PlayerOverload;
            _player.OnPlayerModeChanged -= UpdateCurCounter;
            _player.OnPlayerModeChanged -= UpdateCurState;
            _player.OnCounterConditionChanged -= UpdateCurCounter;
            _player.OnDashCountChanged -= UpdateCurDash;
            _health.PlayerHealthChanged -= UpdateCurHealth;
            _player.GetCompo<PlayerOverload>().OverloadChanged -= UpdateCurOverload;
        }

        private void Update()
        {
            UpdateCurSkill();

            _playerDashTime.text = $"({_player.GetRemainCooldown("DASH")})";
            _playerSkillTime.text = $"({_player.GetRemainCooldown("SKILL")})";
            _playerCurCounterTime.text = $"({_player.GetRemainCooldown("COUNTER")})";

        }

        private void UpdateCurSkill()
        {
            _playerSkill.text = _player.GetCompo<PlayerSkill>().GetMainSkillSO().CanExecuteSkill(_player) ? "ON" : "OFF";
        }

        private void UpdateCurCounter(PlayerMode mode)
        {
            bool isCanCounter = _player.PlayerMode == PlayerMode.RELEASE && !_player.IsCoating && _player.CanCounter();
            _playerCurCounter.text = isCanCounter ? "ON" : "OFF";

            foreach (Label label in _playerCounters)
            {
                if (isCanCounter) label.AddToClassList("orange");
                else label.RemoveFromClassList("orange");
            }
        }

        private void UpdateCurDash()
        {
            _playerCurDash.text = $"{_player.CurrentDashCount}";


            UpdateCurDashUI();
        }


        private void UpdateCurOverload(float overload)
        {
            float target = overload;

            if (_overloadCoroutine != null)
                StopCoroutine(_overloadCoroutine);

            _overloadCoroutine = StartCoroutine(AnimateOverloadInt(target));
        }


        private IEnumerator AnimateOverloadInt(float target)
        {
            if (!float.TryParse(_playerCurOverload.text, out float currentFloat))
                currentFloat = 0;

            int current = Mathf.RoundToInt(currentFloat);
            int targetInt = Mathf.RoundToInt(target);

            float totalDuration = 0.25f;
            int steps = Mathf.Max(1, Mathf.Abs(targetInt - current));
            float interval = totalDuration / steps;

            while (current != targetInt)
            {
                current += current < targetInt ? 1 : -1;
                _playerCurOverload.text = current.ToString();
                yield return new WaitForSeconds(interval);
            }

            _overloadCoroutine = null;
        }

        private void UpdateCurState(PlayerMode playermode)
        {
            string state = playermode switch
            {
                PlayerMode.NORMAL => "Normal",
                PlayerMode.RELEASE => "Release",
                PlayerMode.OVERLOAD => "Panic",
                _ => null
            };
            _playerState.text = state;

            foreach (Label label in _playerStates)
            {

                if (playermode == PlayerMode.RELEASE)
                    label.AddToClassList("orange");
                else
                {
                    if (playermode == PlayerMode.OVERLOAD) return;
                    label.RemoveFromClassList("orange");

                }
            }
        }
        private void InitializePlayerHealthUI()
        {
            float currentHp = _player.GetCompo<PlayerHealth>().GetCurrentHp();
            float maxHp = _player.GetCompo<PlayerHealth>().GetMaxHp();

            _playerCurHealth.text = Mathf.RoundToInt(currentHp).ToString();
            _playerMaxHealth.text = Mathf.RoundToInt(maxHp).ToString();
        }

        private void UpdateCurHealth()
        {
            float currentHp = _player.GetCompo<PlayerHealth>().GetCurrentHp();
            float maxHp = _player.GetCompo<PlayerHealth>().GetMaxHp();

            if (_playerHpCoroutine != null)
                StopCoroutine(_playerHpCoroutine);

            _playerHpCoroutine = StartCoroutine(AnimatePlayerHpText(currentHp, maxHp));

            UpdateHealthColor(currentHp);
        }

        private IEnumerator AnimatePlayerHpText(float targetHp, float maxHp)
        {
            if (!int.TryParse(_playerCurHealth.text, out int currentInt))
                currentInt = Mathf.RoundToInt(targetHp); // 실패 시 targetHp로 초기화

            int targetInt = Mathf.RoundToInt(targetHp);
            int maxInt = Mathf.RoundToInt(maxHp);

            float totalDuration = 0.25f;
            int steps = Mathf.Max(1, Mathf.Abs(targetInt - currentInt));
            float interval = totalDuration / steps;

            while (currentInt != targetInt)
            {
                currentInt += currentInt < targetInt ? 1 : -1;
                _playerCurHealth.text = currentInt.ToString();
                _playerMaxHealth.text = maxInt.ToString();
                yield return new WaitForSeconds(interval);
            }

            _playerHpCoroutine = null;
        }

        private IEnumerator FlashPlayerHealth()
        {
            foreach (Label label in _playerHealths) label.AddToClassList("player-health-white");

            yield return new WaitForSeconds(0.25f);

            foreach (Label label in _playerHealths) label.RemoveFromClassList("player-health-white");
        }

        private void UpdateHealthColor(float currentHp)
        {
            if (_player.PlayerMode == PlayerMode.OVERLOAD) return;

            string targetClass = currentHp switch
            {
                >= 70 => "green",
                >= 40 => "orange",
                _ => "red"
            };

            foreach (Label label in _playerHealths)
            {
                if (label.ClassListContains(targetClass)) continue;

                label.RemoveFromClassList("green");
                label.RemoveFromClassList("orange");
                label.RemoveFromClassList("red");
                label.AddToClassList(targetClass);
            }

            StartCoroutine(FlashPlayerHealth());
        }


        public void UpdateBossHpUI()
        {
            _boss = BossManager.Instance.Boss;
            float maxHp = _boss.GetCompo<BossHealth>().GetMaxHp();
            float bossHp = _boss.GetCompo<BossHealth>().GetCurrentHp();

            float target = Mathf.Clamp01(bossHp / maxHp);

            if (_bossHpCoroutine != null)
                StopCoroutine(_bossHpCoroutine);

            _bossHpCoroutine = StartCoroutine(AnimateBossHp(target));
        }

        private IEnumerator AnimateBossHp(float target)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            float start = _currentBossHpRatio;
            _currentBossHpRatio = target;

            float half = 0.5f;

            StartCoroutine(FlashBossHealth());

            float prevLeftPercent = -1f;
            float prevRightPercent = -1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float value = Mathf.Lerp(start, target, EaseOutCubic(t));

                float leftPercent, rightPercent;

                if (value > half)
                {
                    leftPercent = ((value - half) / half) * 100f;
                    rightPercent = 100f;
                }
                else
                {
                    leftPercent = 0f;
                    rightPercent = (value / half) * 100f;
                }

                if (!Mathf.Approximately(leftPercent, prevLeftPercent))
                {
                    _bossHealthLeft.style.width = new Length(leftPercent, LengthUnit.Percent);
                    prevLeftPercent = leftPercent;
                }

                if (!Mathf.Approximately(rightPercent, prevRightPercent))
                {
                    _bossHealthRight.style.width = new Length(rightPercent, LengthUnit.Percent);
                    prevRightPercent = rightPercent;
                }
                yield return null;
            }

            _bossHpCoroutine = null;
        }

        private IEnumerator FlashBossHealth()
        {
            foreach (VisualElement element in _bossHealths) element.AddToClassList("boss-health-white");
            foreach (VisualElement element in _bossFrames) element.AddToClassList("boss-frame-white");

            yield return new WaitForSeconds(0.1f);

            foreach (VisualElement element in _bossHealths) element.RemoveFromClassList("boss-health-white");
            foreach (VisualElement element in _bossFrames) element.RemoveFromClassList("boss-frame-white");
        }
        private float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3);

        private void PlayerOverload(PlayerMode mode)
        {
            bool shouldBeRed = mode == PlayerMode.OVERLOAD;
            foreach (Label label in _allLabel)
            {
                if (shouldBeRed) label.AddToClassList("overload-red");
                else label.RemoveFromClassList("overload-red");
            }
        }

        public void PauseUpdate(int pause)
        {
            foreach (VisualElement element in _bossHealths) element.AddToClassList(pause == 1 ? "boss-health-orange" : "boss-health-red");
            foreach (VisualElement element in _bossFrames) element.AddToClassList(pause == 1 ? "boss-frame-orange" : "boss-frame-red");
        }
    }
}
