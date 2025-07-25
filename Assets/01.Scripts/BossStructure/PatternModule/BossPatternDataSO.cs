using System.Collections.Generic;
using UnityEngine;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BossPatternDataSO", menuName = "SO/Boss/BossPatternDataSO")]
    public class BossPatternDataSO : ScriptableObject
    {
        public string bossName;
        [Space]
        [Space]
        public List<PatternSO> phase1Patterns;
        [Space]
        [Space]
        public PatternSO phaseChangePattern;
        [Space]
        [Space]
        public List<PatternSO> phase2Patterns;
        [Space]
        [Space]
        public PatternSO finalPhasePattern;

        public List<string> Phase1DialogKeyList;
        public List<string> Phase2DialogKeyList;
        public List<string> FinalPhaseDialogKeyList;
    }
}
