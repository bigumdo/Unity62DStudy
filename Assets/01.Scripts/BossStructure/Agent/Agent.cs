using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace YUI.Agents
{
    public class Agent : MonoBehaviour
    {
        [ReadOnly] public string CurrentState;

        [Space]

        public UnityEvent OnHitEvent;
        public UnityEvent OnDeadEvent;

        public bool IsDead { get; set; }

        protected Dictionary<Type, IAgentComponent> _components;

        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IAgentComponent>();
            GetComponentsInChildren<IAgentComponent>(true).ToList()
                .ForEach(component => _components.Add(component.GetType(), component));

            InitComponent();
            AfterInitComponents();
        }

        protected virtual void InitComponent()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        protected virtual void AfterInitComponents()
        {
            _components.Values.ToList().ForEach(component =>
            {
                if (component is IAfterInit afterInit)
                {
                    afterInit.AfterInit();
                }
            });

            OnHitEvent.AddListener(HandleHitEvent);
            OnDeadEvent.AddListener(HandleDeadEvent);
        }

        public T GetCompo<T>(bool isDerived = false) where T : IAgentComponent
        {
            if(_components.TryGetValue(typeof(T), out IAgentComponent compo))
            {
                return (T)compo;
            }

            if (!isDerived)
                return default;

            Type findType = _components.Keys.FirstOrDefault(t => t.IsSubclassOf(typeof(T)));

            if(findType != null)
                return (T)_components[findType];

            return default;
        }

        private void OnDestroy()
        {
            OnHitEvent.RemoveListener(HandleHitEvent);
            OnDeadEvent.RemoveListener(HandleDeadEvent);
        }

        public virtual void HandleHitEvent()
        {
            
        }

        public virtual void HandleDeadEvent()
        {
            
        }

        public Coroutine StartDelayCallback(float delayTime, Action callback) {
            return StartCoroutine(DelayCoroutine(delayTime, callback));
        }
    
        private IEnumerator DelayCoroutine(float delayTime, Action callback) {
            yield return new WaitForSeconds(delayTime);
            callback?.Invoke();
        }
    }
}
