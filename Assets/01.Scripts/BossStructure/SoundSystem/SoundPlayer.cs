using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using YUI.Cores;
using YUI.ObjPooling;

namespace YUI.SoundSystem
{
    public class SoundPlayer : PoolableMono
    {
        private AudioSource _audio;
        [SerializeField] AudioMixerGroup _bgmGroup, _sfxGroup;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundSO sound)
        {
            if (sound.type == soundType.BGM)
            {
                if (SoundManager.Instance.bgmSound != null)
                {
                    PoolingManager.Instance.Push(SoundManager.Instance.bgmSound);
                    SoundManager.Instance.bgmSound = this;
                }
                _audio.outputAudioMixerGroup = _bgmGroup;
                _audio.clip = sound.clip;
                _audio.volume = sound.volume;
                _audio.pitch = sound.pitch;
                _audio.loop = sound.loop;
                _audio.Play();
            }
            else if (sound.type == soundType.SFX)
            {
                _audio.outputAudioMixerGroup = _sfxGroup;
                _audio.volume = sound.volume;
                _audio.pitch = sound.pitch;
                _audio.PlayOneShot(sound.clip);
                StartCoroutine(StopSound(sound.clip.length + 0.1f));
            }
        }

        private IEnumerator StopSound(float soundLength)
        {
            yield return new WaitForSeconds(soundLength);
            PoolingManager.Instance.Push(this);
        }

        public override void ResetItem()
        {
            
        }

        [ContextMenu("Test")]
        public void Test()
        {
            _audio.PlayOneShot(_audio.clip);
        }
    }
}
