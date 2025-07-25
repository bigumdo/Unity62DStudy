using System.Collections.Generic;
using UnityEngine;

namespace YUI.SoundSystem
{
    [CreateAssetMenu(fileName = "SoundListSO", menuName = "SO/Sound/SoundListSO")]
    public class SoundListSO : ScriptableObject
    {
        public List<SoundSO> sounds = new List<SoundSO>();
    }
}
