using System;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;

namespace YUI.Apples
{
    public class AppleSpawner : MonoBehaviour
    {
        [SerializeField] private Vector3 size;
        [SerializeField] private AppleObject applePrefab;
        public List<AppleObject> spawnedApples = new List<AppleObject>();

        private void Update()
        {
            if (GameManager.Instance.IsBattle && PlayerManager.Instance.Player.IsDead == false && spawnedApples.Count == 0)
            {
                SpawnApple();
            }

            spawnedApples.RemoveAll(apple => apple == null || apple.gameObject == null);
        }

        public void AllDestroy()
        {
            for (int i = spawnedApples.Count - 1; i >= 0; i--)
            {
                if (spawnedApples[i] != null)
                {
                    Destroy(spawnedApples[i].gameObject);
                }
            }
            spawnedApples.Clear();
        }

        private void SpawnApple()
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                UnityEngine.Random.Range(-size.x / 2, size.x / 2),
                UnityEngine.Random.Range(-size.y / 2, size.y / 2)
            );

            AppleObject apple = Instantiate(applePrefab, spawnPosition, Quaternion.identity);
            apple.Init(this);
            spawnedApples.Add(apple);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
