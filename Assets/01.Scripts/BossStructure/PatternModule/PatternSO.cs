using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.Bosses;

namespace YUI.PatternModules
{
    [System.Serializable]
    public class PatternStep
    {
        [SerializeField] private List<PatternModule> selectModules;
        [HideInInspector] public List<PatternModule> modules;
        public bool isSequential = true;
        public float delay;

        public void Init()
        {
            modules = new List<PatternModule>();
            foreach (PatternModule i in selectModules)
            {
                modules.Add(i.Clone() as  PatternModule);
            }
        }

        public IEnumerator Execute(Boss boss)
        {
            if (isSequential)
            {
                //PatternManager.Instance.isExecutingPattern = true;
                foreach (PatternModule m in modules)
                {
                    boss.StartCoroutine(m.Execute());
                    yield return new WaitUntil(() => m.isComplete);
                    m.isComplete = false;
                }

                if (delay > 0)
                    yield return new WaitForSeconds(delay);
            }
            else
            {
                List<PatternModule> toRemove = new List<PatternModule>();

                foreach (PatternModule t in modules)
                {
                    boss.StartCoroutine((t.Execute()));
                }

                while (modules.Count > toRemove.Count)
                {
                    toRemove.Clear();
                    foreach (PatternModule t in modules)
                    {
                        if (t.isComplete)
                        {
                            toRemove.Add(t);
                        }
                    }

                    yield return null;
                }
                
                foreach(PatternModule m in toRemove)
                {
                    m.isComplete = false;
                }

                if (delay > 0)
                    yield return new WaitForSeconds(delay);
            }

        }
    }

    [System.Serializable]
    public class PatternRepeat
    {
        public bool shouldExecute = true;
        public List<PatternStep> patternSteps;
        public int repeatCnt;

        public void Init() => patternSteps.ForEach(x => x.Init());
    }

    [CreateAssetMenu(fileName = "PatternSO", menuName = "SO/Boss/PatternSO")]
    public class PatternSO : ScriptableObject
    {
        public string patternName;
        public List<PatternRepeat> patternRepeats;
        [SerializeField] private float _delay;

        private float finalPatternCheck;

        public IEnumerator Execute(Boss boss)
        {
            if (patternName == "FinalPattern")
                finalPatternCheck = Time.time;
            
                PatternManager.Instance.isExecutingPattern = true;
            foreach (PatternRepeat pr in patternRepeats)
            {
                if (!pr.shouldExecute)
                    continue;

                if (pr.repeatCnt > 1)
                {
                    for (int i = 0; i < pr.repeatCnt; ++i)
                    {
                        foreach (PatternStep ps in pr.patternSteps)
                        {
                            yield return ps.Execute(boss);
                        }
                    }
                }
                else
                {
                    foreach (PatternStep ps in pr.patternSteps)
                    {
                        yield return ps.Execute(boss);
                    }
                }
            }

            yield return new WaitForSeconds(_delay);
            PatternManager.Instance.isExecutingPattern = false;
        }

        public void Init(Boss boss)
        {
            patternRepeats.ForEach(x => x.Init());
            foreach(PatternRepeat r in patternRepeats)
                foreach (PatternStep a in r.patternSteps)
                    foreach (PatternModule m in a.modules)
                        m.Init(boss);
        }
    }
}
