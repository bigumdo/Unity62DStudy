using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Cores;
using YUI.PatternModules;

namespace YUI.Agents.Bosses
{
    public class TutorialBossManger : MonoSingleton<TutorialBossManger>
    {
        private TutorialBossPatternDataSO _patternList;

        private Dictionary<string, PatternSO> _attack1PatternDictionary;
        private Dictionary<string, PatternSO> _attack2PatternDictionary;
        private Dictionary<string, PatternSO> _counterPatternDictionary;
        private string _lastPatternName;
        private int _duplicatedPatternCnt;

        [HideInInspector] public bool isExecutingPattern;

        public void PattternSetting(TutorialBossPatternDataSO patternDataSO)
        {
            _patternList = patternDataSO;
            _attack1PatternDictionary = new Dictionary<string, PatternSO>();
            _attack2PatternDictionary = new Dictionary<string, PatternSO>();
            _counterPatternDictionary = new Dictionary<string, PatternSO>();
            Debug.Assert(patternDataSO.attack1Patterns != null, "_attack1PatternDictionary is not present");
            Debug.Assert(patternDataSO.attack2Patterns != null, "_attack2PatternDictionary is not present");
            Debug.Assert(patternDataSO.counterPatterns != null, "_counterPatternDictionary is not present");
            patternDataSO.attack1Patterns.ForEach(x => _attack1PatternDictionary.Add(x.patternName, x));
            patternDataSO.attack2Patterns.ForEach(x => _attack2PatternDictionary.Add(x.patternName, x));
            patternDataSO.counterPatterns.ForEach(x => _counterPatternDictionary.Add(x.patternName, x));
            ModuleInit();
        }

        private void ModuleInit()
        {
            foreach (var x in _attack1PatternDictionary.Values)
                x.Init(BossManager.Instance.Boss);
            foreach (var x in _attack2PatternDictionary.Values)
                x.Init(BossManager.Instance.Boss);
            foreach (var x in _counterPatternDictionary.Values)
                x.Init(BossManager.Instance.Boss);
        }

        public PatternSO GetRandomPattern(TutorialBossStateEnum currentState)
        {
            switch (currentState)
            {
                case TutorialBossStateEnum.Attack1:
                    return GetPattern(_attack1PatternDictionary);
                case TutorialBossStateEnum.Attack2:
                    return GetPattern(_attack2PatternDictionary);
                case TutorialBossStateEnum.Counter:
                    return GetPattern(_counterPatternDictionary);
            }
            return null;
        }
        private PatternSO GetPattern(Dictionary<string, PatternSO> patternDictionary)
        {
            List<PatternSO> patterns = new List<PatternSO>(patternDictionary.Values);
            int randomCnt = Random.Range(0, patterns.Count);
            if (_lastPatternName == patterns[randomCnt].patternName)
                _duplicatedPatternCnt++;
            else
                _duplicatedPatternCnt = 0;
            if (_duplicatedPatternCnt >= 3)
                randomCnt = Mathf.Clamp(randomCnt > 0 ? --randomCnt : ++randomCnt, 0, patterns.Count - 1);
            _lastPatternName = patterns[randomCnt].patternName;
            return patterns[randomCnt];
        }

        public void BossChangeState(TutorialBossStateEnum changeState)
        {
            StartCoroutine(ChangeDelay(changeState));
        }

        private IEnumerator ChangeDelay(TutorialBossStateEnum changeState)
        {
            yield return new WaitUntil(() => !PatternManager.Instance.isExecutingPattern);
            Debug.Log(changeState);
            BossManager.Instance.Boss.GetVariable<TutorialStateChangeEvent>("TutorialStateChangeEvent").Value.SendEventMessage(changeState);
        }

        private void Update()
        {
            //if (Keyboard.current.vKey.wasPressedThisFrame)
            //{
            //    BossChangeState(TutorialBossStateEnum.None);
            //}
            //if (Keyboard.current.bKey.wasPressedThisFrame)
            //    BossChangeState(TutorialBossStateEnum.Attack1);
            //if (Keyboard.current.mKey.wasPressedThisFrame)
            //    BossChangeState(TutorialBossStateEnum.Attack2);
            //if (Keyboard.current.kKey.wasPressedThisFrame)
            //    BossChangeState(TutorialBossStateEnum.Counter);
            //if (Keyboard.current.lKey.wasPressedThisFrame)
            //    BossChangeState(TutorialBossStateEnum.Dead);
        }
    }
}
