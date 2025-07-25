using YUI.Cores;
using System.Collections.Generic;
using UnityEngine;

namespace YUI.ObjPooling
{
    public class PoolingManager : MonoSingleton<PoolingManager>
    {
        private Dictionary<string, Pool<PoolableMono>> _pools
                        = new Dictionary<string, Pool<PoolableMono>>();

        public PoolingTableSO listSO;

        private void Awake()
        {
            foreach (PoolingSetting item in listSO.datas)
            {
                CreatePool(item);
            }
        }

        private void CreatePool(PoolingSetting item)
        {
            var pool = new Pool<PoolableMono>(item.prefab, item.typeName, transform, item.poolingSettingCnt);
            _pools.Add(item.prefab.type, pool);
        }


        public PoolableMono Pop(string type)
        {
            if (_pools.ContainsKey(type) == false)
            {
                Debug.LogError($"Prefab does not exist on pool : {type.ToString()}");
                return null;
            }

            PoolableMono item = _pools[type].Pop();
            item.ResetItem();
            return item;
        }

        public void Push(PoolableMono obj, bool resetParent = false)
        {
            if (resetParent)
                obj.transform.SetParent(transform);
            _pools[obj.type].Push(obj);
        }
    }
}
