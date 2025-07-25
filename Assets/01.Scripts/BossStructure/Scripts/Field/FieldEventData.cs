using System.Collections.Generic;
using UnityEngine;

namespace YUI.Fields {
    public enum FieldEventType {
        None,
        OnCollisionEnter,
        OnTriggerEnter,
        OnTriggerStay,
    }

    [System.Serializable]
    public class FieldEventVariable {
        [Header("GameObject가 아닌 변수는 SO에서 수정")]
        public string variableName;
        public Object value;
    }

    [System.Serializable]
    public class FieldEventData {
        public List<FieldEventSO> EventSOList;
        public FieldEventType EventType;
        public List<FieldEventVariable> fieldEventVariableList;
    }
}
