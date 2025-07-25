using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents;
using YUI.Agents.Bosses;
using YUI.Cores;

namespace YUI.Rooms
{
    public class BossRoom : Room
    {
        [SerializeField] private GameObject past;
        [SerializeField] private GameObject room;
        [SerializeField] private GameObject battle;
        [SerializeField] private Collider2D battleConfiner;
        [SerializeField] private List<string> bossDialogueList;

        private bool isCleared = false;

        protected override void Start()
        {
            base.Start();

            isCleared = false;
            GameManager.Instance.IsBattle = false;
            //UIManager.Instance.SetBossGaugeVisibility(false);
        }

        public bool IsCleared() => isCleared;
        public void StartClearRoutine()
        {
            StartCoroutine(ClearRoutine());
        }
        public override void EnterRoom()
        {
            base.EnterRoom();
        }

        protected override void EndStartRoomRoutineCallback()
        {
            base.EndStartRoomRoutineCallback();
            BossManager.Instance.Boss.GetCompo<AgentRenderer>(true).Renderer.material.SetFloat("_Value", 0);
            //BossManager.Instance.StartBT();
        }

        private IEnumerator ClearRoutine()
        {
            isCleared = true;
            CameraManager.Instance.SetDefaultCam();

            yield return null;
        }

        public Collider2D GetBattleCollider() => battleConfiner;

        public void ActiveTileMap(bool active, string type)
        {
            switch (type)
            {
                case "ROOM":
                    {
                        room.SetActive(active);
                        break;
                    }
                case "BATTLE":
                    {
                        battle.SetActive(active);
                        break;
                    }
                case "PAST":
                    {
                        past.SetActive(active);
                        break;
                    }
            }
        }
    }
}
