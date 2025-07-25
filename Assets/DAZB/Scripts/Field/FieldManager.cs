using UnityEngine;
using YUI.Cores;

namespace YUI.Fields {
    public class FieldManager : MonoSingleton<FieldManager> {
        private Vector3 playerSpawnPoint;

        public void SetPlayerSpawnPoint(Vector3 spawnPoint) => playerSpawnPoint = spawnPoint;
        public Vector3 GetPlayerSpawnPoint() => playerSpawnPoint;
    }
}
