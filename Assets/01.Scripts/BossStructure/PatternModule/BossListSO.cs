using System.Collections.Generic;
using UnityEngine;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BossListSO", menuName = "SO/Boss")]
    public class BossListSO : ScriptableObject
    {
        public List<BossPatternDataSO> bosses = new List<BossPatternDataSO>();
    }
}
