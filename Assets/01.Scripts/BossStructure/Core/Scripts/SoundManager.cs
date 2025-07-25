using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using System.Collections.Generic;
using YUI.ObjPooling;
using YUI.SoundSystem;
using System;
using Unity.VisualScripting;

namespace YUI.Cores
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [HideInInspector]public SoundPlayer bgmSound;

        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private SoundListSO _soundList;
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private float _minValue = -80f;
        [SerializeField] private float _maxValue = 0f;

        private Slider _masterSlider;
        private Slider _bgmSlider;
        private Slider _sfxSlider;

        private Dictionary<string, SoundSO> _soundDictionary;

        private void Awake()
        {
            _soundDictionary = new Dictionary<string, SoundSO>();
            _soundList.sounds.ForEach(x => _soundDictionary.Add(x.soundName, x));

        }

        public void InitSliders()
        {
            var settingRoot = _uiDocument.rootVisualElement;

            _masterSlider = settingRoot.Q<Slider>("master-slider");
            _bgmSlider = settingRoot.Q<Slider>("bgm-slider");
            _sfxSlider = settingRoot.Q<Slider>("sfx-slider");

            InitSlider(_masterSlider, OnMasterVolumeChanged);
            InitSlider(_bgmSlider, OnBgmVolumeChanged);
            InitSlider(_sfxSlider, OnSfxVolumeChanged);
        }

        private void InitSlider(Slider slider, Action<float> onChanged)
        {
            if (slider == null)
            {
                Debug.LogWarning("Slider not found.");
                return;
            }

            slider.lowValue = _minValue;
            slider.highValue = _maxValue;
            slider.RegisterValueChangedCallback(evt => onChanged?.Invoke(evt.newValue));
        }

        private void OnMasterVolumeChanged(float value)
        {
            _mixer.SetFloat("MasterVolume", value);
        }

        private void OnBgmVolumeChanged(float value)
        {
            _mixer.SetFloat("BGMVolume", value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            _mixer.SetFloat("SFXVolume", value);
        }

        public void PlaySound(string soundName)
        {
            SoundPlayer sound = PoolingManager.Instance.Pop("SoundPlayer") as SoundPlayer;

            if (_soundDictionary.TryGetValue(soundName, out SoundSO soundSO))
            {
                sound.PlaySound(soundSO);
            }
            else
            {
                Debug.LogError($"{soundName} Not Found");
            }
        }
    }
}
