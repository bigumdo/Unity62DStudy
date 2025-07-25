using System.Collections.Generic;
using UnityEngine;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "TutorialBossPatternDataSO", menuName = "SO/Boss/TutorialBossPatternDataSO")]
    public class TutorialBossPatternDataSO : ScriptableObject
    {
        public string bossName;
        [Space]
        [Space]
        public List<PatternSO> attack1Patterns;
        [Space]
        [Space]
        public List<PatternSO> attack2Patterns;
        [Space]
        [Space]
        public List<PatternSO> counterPatterns;
    }
}
