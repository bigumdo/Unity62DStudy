using System.Collections;
using UnityEngine;
using YUI.Agents.Bosses;
using YUI.ObjPooling;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "CounterRootSpawnModule", menuName = "SO/Boss/Module/Spawn/CounterRootSpawnModule")]
    public class CounterRootSpawnModule : PatternModule
    {
        [SerializeField] private float _startPos;
        [SerializeField] private float _endPos;

        [SerializeField] private float _rangeDisplayTime;
        [SerializeField] private float _attackDelayTime;
        [SerializeField] private float _attackTime;
        [SerializeField] private float _damage;
        [SerializeField] private float _length;

        public override IEnumerator Execute()
        {
            bool topAndBottom = Random.Range(-1, 2) > 0;
            Vector3 spawnPos = BossManager.Instance.startPos;
            float xPos = Random.Range(_startPos, _endPos);
            if (topAndBottom)
                spawnPos += new Vector3(xPos, 8, 0);
            else
                spawnPos += new Vector3(xPos, -8, 0);

            CounterRoot counterRoot = PoolingManager.Instance.Pop("CounterRoot") as CounterRoot;
            counterRoot.SetData(spawnPos, _rangeDisplayTime, _attackDelayTime, _attackTime, _length, _damage);
            BossManager.Instance.counterObjList.Add(counterRoot);
            yield return counterRoot.Excute();

            CompleteActionExecute();
        }
    }
}
