using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YUI.Fields {

    public class FieldEventTrigger : MonoBehaviour {
        [SerializeField] private LayerMask whatIsPlayer;
        [SerializeField] private FieldEventData fieldEvent;

        private void Start() {
            for (int i = 0; i < fieldEvent.EventSOList.Count; i++) {
                var instance = Instantiate(fieldEvent.EventSOList[i]);
                instance.SetOwner(this);
                fieldEvent.EventSOList[i] = instance;
            }
        }

        public FieldEventVariable GetVariable(string variableName) {
            foreach (var iter in fieldEvent.fieldEventVariableList) {
                if (iter.variableName == variableName) {
                    return iter;
                }
            }
            return null;
        }

        public void SetActive(bool isActive) {
            fieldEvent.EventSOList.ForEach(x => x.SetActive(isActive));
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!IsInLayerMask(collision.gameObject.layer)) return;

            foreach (var eventSO in fieldEvent.EventSOList) {
                if (fieldEvent.EventType == FieldEventType.OnTriggerEnter) {
                    eventSO.SetCurrentCollidedObject(collision.gameObject);
                    eventSO.Execute();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!IsInLayerMask(other.gameObject.layer)) return;

            foreach (var eventSO in fieldEvent.EventSOList) {
                if (fieldEvent.EventType == FieldEventType.OnTriggerEnter) {
                    eventSO.SetCurrentCollidedObject(other.gameObject);
                    eventSO.Execute();
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (!IsInLayerMask(other.gameObject.layer)) return;

            foreach (var eventSO in fieldEvent.EventSOList) {
                if (fieldEvent.EventType == FieldEventType.OnTriggerStay) {
                    eventSO.SetCurrentCollidedObject(other.gameObject);
                    eventSO.Execute();
                }
            }
        }

        private bool IsInLayerMask(int layer) {
            return (whatIsPlayer.value & (1 << layer)) != 0;
        }
    }
}
