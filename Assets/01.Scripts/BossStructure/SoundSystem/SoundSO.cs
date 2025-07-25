using UnityEngine;

namespace YUI.SoundSystem
{
    public enum soundType
    {
        BGM,
        SFX
    }


    [CreateAssetMenu(fileName = "SoundSO", menuName = "SO/Sound/SoundSO")]
    public class SoundSO : ScriptableObject
    {
        public string soundName;
        public soundType type;
        public AudioClip clip;
        [Range(0,1)]
        public float volume;
        [Range(0,1)]
        public float pitch;
        public bool loop;
    }
}
