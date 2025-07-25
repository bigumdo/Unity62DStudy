using System.Collections;
using UnityEngine;

namespace YUI.PatternModules
{
    public interface IPatternActionModule
    {
        public IEnumerator Execute();
    }
}
