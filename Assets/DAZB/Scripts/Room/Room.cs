using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.Effects;
using YUI.ObjPooling;
using YUI.Portals;
using YUI.UI.DialogSystem;

namespace YUI.Rooms
{
    public enum DoorType
    {
        LEFT, RIGHT, TOP, BOTTOM
    }

    [System.Serializable]
    public class DoorData
    {
        public DoorType doorType;
        public GameObject doorObject;
        public Transform effectSpawnedTrm;
    }

    public class Room : MonoBehaviour
    {
        [SerializeField] protected List<DoorData> doors = new List<DoorData>();
        [SerializeField] protected string startDialogKey;

        protected virtual void Start()
        {
            //CloseAllDoor(false);
            GameManager.Instance.SetCurrentRoom(this);
            StartCoroutine(StartRoomRoutine());
        }

        private IEnumerator StartRoomRoutine()
        {
            CameraManager.Instance.FadeOut(0f);

            if (PlayerManager.Instance.Player != null)
            {
                PlayerManager.Instance.Player.InputReader.Enable(false);
                PlayerManager.Instance.Player.InputReader.SetSlowMode(true);
            }

            yield return new WaitForSeconds(0.2f);

            EnterRoom();
            CameraManager.Instance.FadeIn(0.75f);

            yield return new WaitForSeconds(0.5f);

            if (PlayerManager.Instance.Player != null)
            {
                PlayerManager.Instance.Player.InputReader.Enable(true);
                PlayerManager.Instance.Player.InputReader.SetSlowMode(false);
            }

            if (startDialogKey != "")
            {
                List<DialogData> dialogDatas = DialogManager.Instance?.GetLines(startDialogKey);

                var dialogCanvas = UIManager.Instance?.GetUI<DialogCanvas>();
                if (dialogCanvas != null && dialogDatas != null)
                {
                    UIManager.Instance.ShowUI<DialogCanvas>();
                    dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, null));
                }

                yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>()?.isFinished == true);
            }

            EndStartRoomRoutineCallback();
        }

        protected virtual void EndStartRoomRoutineCallback()
        {
            
        }

        public virtual void EnterRoom()
        {
            GameManager.Instance.SetCurrentRoom(this);
        }

        public List<DoorData> GetDoors() => doors;

        public void OpenDoor(DoorType doorType)
        {
            foreach (var door in doors)
            {
                if (door.doorType == doorType && door.doorObject != null)
                {
                    door.doorObject.SetActive(true);
                }
            }
        }

        public void CloseDoor(DoorType doorType)
        {
            foreach (var door in doors)
            {
                if (door.doorType == doorType && door.doorObject != null)
                {
                    door.doorObject.SetActive(false);

                    EffectPlayer effect = PoolingManager.Instance.Pop("WallOpenEffect") as EffectPlayer;
                    effect.transform.position = door.effectSpawnedTrm.position;

                    CameraManager.Instance.ShakeCamera(4, 2, 0.3f);
                }
            }
        }

        public void CloseAllDoor(bool isEffect = true)
        {
            foreach (var door in doors)
            {
                door.doorObject.SetActive(true);

                if (isEffect)
                {                    
                    EffectPlayer effect = PoolingManager.Instance.Pop("WallOpenEffect") as EffectPlayer;
                    effect.transform.position = door.effectSpawnedTrm.position;
                    CameraManager.Instance.ShakeCamera(4, 2, 0.3f);
                }

            }
        }

        public void OpenAllDoor()
        {
            foreach (var door in doors)
            {
                door.doorObject.SetActive(false);
            }
        }
    }
}
 