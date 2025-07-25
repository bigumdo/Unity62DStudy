using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;

namespace YUI.StatSystem
{
    [CreateAssetMenu(fileName = "StatSO", menuName = "SO/StatSystem/Stat")]
    public class StatSO : ScriptableObject, ICloneable
    {
        public delegate void ValueChangeHandler(StatSO stat, float current, float previous);
        public event ValueChangeHandler OnValueChange;

        public string statName;
        public string description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _displayName;
        [SerializeField] private float _baseValue, _minValue, _maxValue;
        private Dictionary<string, float> _modifyValueByKey = new Dictionary<string, float>();
        
        [field:SerializeField] public bool IsPercent { get; private set; }
        
        private float _modifiedValue = 0;
        public Sprite Icon => _icon;
        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }
        
        public float Value => Mathf.Clamp(_baseValue + _modifiedValue, _minValue, _maxValue);
        public bool IsMax => Mathf.Approximately(Value, _maxValue);
        public bool IsMin => Mathf.Approximately(Value, _minValue);

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                float prevValue = Value;
                _baseValue = Mathf.Clamp(value, _minValue, _maxValue);
                TryInvokeValueChangedEvent(Value, prevValue);

            }
        }

        public void AddModifier(string key, float value)
        {
            //Ű�� �ִٸ� return
            if (_modifyValueByKey.ContainsKey(key)) return;
            
            float prevValue = Value;
            _modifiedValue += value;
            _modifyValueByKey.Add(key, value);
            
            TryInvokeValueChangedEvent(Value, prevValue);
        }

        public void RemoveModifier(string key)
        {
            //���࿡ Ű�� �ִٸ� ����ȴ�.
            if (_modifyValueByKey.TryGetValue(key, out float value))
            {
                float prevValue = Value;
                _modifiedValue -= value;
                _modifyValueByKey.Remove(key);
                
                TryInvokeValueChangedEvent(Value, prevValue);
            }
        }

        private void TryInvokeValueChangedEvent(float value, float prevValue)
        {
            //���� ���� ���鿡 �񱳿��� ���Ǵ� �Լ� ���� ���̰� ���ٸ� ���� ������ �����Ѵ�.
            if(Mathf.Approximately(prevValue, value) == false) 
                OnValueChange?.Invoke(this, value, prevValue);
        }

        public object Clone()
        {
            return Instantiate(this);
        }
    }
}
