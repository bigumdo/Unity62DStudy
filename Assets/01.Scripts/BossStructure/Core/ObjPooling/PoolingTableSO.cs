using System.Collections.Generic;
using UnityEngine;

namespace YUI.ObjPooling
{
    [System.Serializable]
    public struct PoolingSetting
    {
        public string typeName;
        public string poolingName;
        public PoolableMono prefab;
        public int poolingSettingCnt;
    }

    [CreateAssetMenu(menuName = "SO/Pool/PoolingTable")]
    public class PoolingTableSO : ScriptableObject
    {
        public List<PoolingSetting> datas = new List<PoolingSetting>();
    }
}
