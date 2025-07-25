using System;
using System.Collections;
using UnityEngine;
using YUI.Agents.Bosses;

namespace YUI.PatternModules
{
    public abstract class PatternModule : ScriptableObject, IPatternActionModule, ICloneable
    {
        public abstract IEnumerator Execute();

        [HideInInspector] public bool isComplete;
        public event Action OnComplete;

        protected Boss _boss;

        public virtual void Init(Boss boss)
        {
            isComplete = false;
            _boss = boss;
        }

        protected virtual void CompleteActionExecute()
        {
            isComplete = true;
            if(OnComplete != null)
                OnComplete?.Invoke();
        }

        public object Clone()
        {
            return Instantiate(this);
        }
    }
}
