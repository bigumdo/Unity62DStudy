using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using YUI.Agents.players;
using YUI.Rooms;

namespace YUI.Cores {
    public class GameManager : MonoSingleton<GameManager> {
        [SerializeField] private LayerMask donDisableLayer;
        /* [SerializeField] private GameObject tileMap; */
        public bool IsBattle = false;
        public bool IsGmaeStop = false;
        private Room currentRoom;

        public Room GetCurrentRoom() => currentRoom;
        public void SetCurrentRoom(Room room) => currentRoom = room;

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName, bool isFade = true)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, isFade));
        }

        public void LoadSceneUI(string sceneName)
        {
            if (sceneName == "Current")
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
            if (PlayerManager.Instance.Player != null)
                PlayerManager.Instance.Player.InputReader.ResetEvents();

            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator LoadSceneRoutine(string sceneName, bool isFade = true)
        {
            if (PlayerManager.Instance.Player != null)
            {
                PlayerManager.Instance.Player.InputReader.Enable(true);
                PlayerManager.Instance.Player.InputReader.SetSlowMode(false);
            }

            if (isFade)
            {
                CameraManager.Instance.FadeOut(0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            if(PlayerManager.Instance.Player != null)
                PlayerManager.Instance.Player.InputReader.ResetEvents();
            SceneManager.LoadScene(sceneName);
        }

        public void Quit()
        {
            Application.Quit();
        }

        [ContextMenu("ALL Disable")]
        public void AllObjectDisable()
        {
            List<GameObject> allObjectList = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(go => (donDisableLayer.value & (1 << go.layer)) == 0).ToList();

            allObjectList.ForEach(x => x.SetActive(false));
        }
    }
}
