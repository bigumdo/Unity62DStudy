using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.Rooms;
using YUI.UI;
using YUI.UI.DialogSystem;

namespace YUI.Portals {
    public class Portal : MonoBehaviour, IInteractable
    {
        [SerializeField] protected string dialogKey;
        [SerializeField] protected Transform exitPos;
        [SerializeField] protected LayerMask whatIsPlayer;
        [SerializeField] protected Collider2D confinerCollider;

        public Transform GetExitPos() => exitPos;
        public Collider2D GetConfinerCollider() => confinerCollider;

        public void Interact(Player player)
        {
            if (GameManager.Instance.GetCurrentRoom() is BossRoom bossRoom)
            {
                if (!bossRoom.IsCleared()) return;
            }

            List<DialogData>  dialogDatas = DialogManager.Instance.GetLines(dialogKey);
            UIManager.Instance.ShowUI<DialogCanvas>();
            UIManager.Instance.GetUI<DialogCanvas>().StartDialogOpenRoutine(dialogDatas, () => UIManager.Instance.GetUI<DialogCanvas>().StartDialogRoutine(dialogDatas, () =>
            {
                SoundManager.Instance.PlaySound("SFX_OpenDoor");
            }));
        }

        public void Interactable(bool active)
        {
            if (GameManager.Instance.GetCurrentRoom() is BossRoom bossRoom)
            {
                if (!bossRoom.IsCleared()) return;
            }

            if (active)
            {
                UIManager.Instance.GetUI<InteractPopup>().Open();
            }
            else
            {
                UIManager.Instance.GetUI<InteractPopup>().Close();
            }
        }
    }
}
