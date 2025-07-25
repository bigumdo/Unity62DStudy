

using System.Collections.Generic;
using UnityEngine;
using YUI.Bullets.WordBullets;
using YUI.Cores;
using YUI.PatternModules;

namespace YUI.Agents.Bosses
{
    public class PatternManager : MonoSingleton<PatternManager>
    {
        [HideInInspector] public bool isExecutingPattern;

        [SerializeField] private WordBulletSpawner _wordBulletSpawner;

        private BossPatternDataSO _patternList;

        private Dictionary<string, PatternSO> _phase1PatternDictionary;
        private Dictionary<string, PatternSO> _phase2PatternDictionary;
        private string _lastPatternName;
        private int _duplicatedPatternCnt;


        public void PattternSetting(BossPatternDataSO patternDataSO)
        {
            _patternList = patternDataSO;
            _phase1PatternDictionary = new Dictionary<string, PatternSO>();
            _phase2PatternDictionary = new Dictionary<string, PatternSO>();
            Debug.Assert(patternDataSO.phase1Patterns != null, "_phase1PatternDictionary is not present");
            Debug.Assert(patternDataSO.phase2Patterns != null, "_phase2PatternDictionary is not present");
            Debug.Assert(patternDataSO.finalPhasePattern != null, "finalPhasePattern is not present");
            patternDataSO.phase1Patterns.ForEach(x => _phase1PatternDictionary.Add(x.patternName, x));
            patternDataSO.phase2Patterns.ForEach(x => _phase2PatternDictionary.Add(x.patternName, x));
            ModuleInit();
        }

        private void ModuleInit()
        {
            foreach (var x in _phase1PatternDictionary.Values)
                x.Init(BossManager.Instance.Boss);
            foreach (var x in _phase2PatternDictionary.Values)
                x.Init(BossManager.Instance.Boss);
            _patternList.finalPhasePattern.Init(BossManager.Instance.Boss);
            _patternList.phaseChangePattern.Init(BossManager.Instance.Boss);
        }

        public List<string> GetDialog(BossStateEnum currentPhsae)
        {
            switch (currentPhsae)
            {
                case BossStateEnum.Phase1:
                    return _patternList.Phase1DialogKeyList;
                case BossStateEnum.Phase2:
                    return _patternList.Phase2DialogKeyList;
                case BossStateEnum.FinalPhase:
                    return _patternList.FinalPhaseDialogKeyList;
            }
            return null;
        }

        public PatternSO GetRandomPattern(BossStateEnum currentState)
        {
            switch (currentState)
            {
                case BossStateEnum.Phase1:
                    return GetPattern(_phase1PatternDictionary);
                case BossStateEnum.Phase2:
                    return GetPattern(_phase2PatternDictionary);
                case BossStateEnum.FinalPhase:
                    return _patternList.finalPhasePattern;
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
                randomCnt = Mathf.Clamp(randomCnt > 0 ? --randomCnt : ++randomCnt,0,patterns.Count - 1);
            _lastPatternName = patterns[randomCnt].patternName;
            return patterns[randomCnt];
        }

        public PatternSO GetPhase1Pattern(string patternName)
        {
            if (_phase1PatternDictionary.TryGetValue(patternName, out PatternSO pattern))
            {
                return pattern;
            }

            return default;
        }

        public PatternSO GetPhase2Pattern(string patternName)
        {
            if (_phase2PatternDictionary.TryGetValue(patternName, out PatternSO pattern))
            {
                return pattern;
            }

            return default;
        }

        public PatternSO GetPhaseChagnePattern()
        {
            return _patternList.phaseChangePattern;
        }

        public PatternSO GetFinalPhasePattern()
        {
            return _patternList.finalPhasePattern;
        }

        public void SetWordSpawner(bool value)
        {
            _wordBulletSpawner.SetShootable(value);
        }
    }
}
