using UnityEngine;
using YUI.PatternModules;

namespace YUI.Agents.Bosses
{
    public class TutorialBoss : Boss
    {

        private void Start()
        {
            BossManager.Instance.SetTorialBoss(this);
            StartBT();
        }

        public TutorialBossPatternDataSO patternSO;
    }
}
