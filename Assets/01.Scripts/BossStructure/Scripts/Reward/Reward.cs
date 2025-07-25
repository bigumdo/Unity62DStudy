using System.Collections.Generic;
using UnityEngine;
using YUI.Cores;
using YUI.Skills;

namespace YUI.Rewards {
    public class Reward : MonoBehaviour {
        [SerializeField] private List<PassiveSkill> passiveNodeList;
        [SerializeField] private RewardObject rewardObjectPrefab;
        [SerializeField] private float spawnOffset;

        private List<RewardObject> rewardObjects = new List<RewardObject>();

        private void Awake()
        {
            CreateRewardObject();
        }

        [ContextMenu("CreateRewardObject")]
        public void CreateRewardObject() {
            List<PassiveSkill> shuffledPassiveNodes = ShuffleList(new List<PassiveSkill>(passiveNodeList));

            for (int i = 0; i < 3; ++i) {
                float angle = -((360f / 3 * i) + 30);
                float rad = angle * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * spawnOffset;

                RewardObject rewardObject = Instantiate(rewardObjectPrefab, transform.position + pos, Quaternion.identity);
                rewardObject.SetOwner(this);

                rewardObject.SetPassiveNode(shuffledPassiveNodes[i]);

                rewardObjects.Add(rewardObject);
            }
        }

        public void SelectedObject()
        {
            foreach (var rewardObject in rewardObjects)
            {
                Destroy(rewardObject.gameObject);
            }
            rewardObjects.Clear();

            GameManager.Instance.GetCurrentRoom().OpenAllDoor();
            
            Destroy(gameObject);
        }

        private List<PassiveSkill> ShuffleList(List<PassiveSkill> list) {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--) {
                int randIndex = Random.Range(0, i + 1);

                // 요소를 교환
                PassiveSkill temp = list[i];
                list[i] = list[randIndex];
                list[randIndex] = temp;

                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
            return list;
        }
    }
}
