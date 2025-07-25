using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using YUI.Agents;
using YUI.Agents.Bosses;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.Rooms;
using YUI.UI;
using YUI.UI.DialogSystem;

namespace YUI
{
    public class FirstBossBed : MonoBehaviour, IInteractable
    {
        [SerializeField] private Light2D globalLight;
        [SerializeField] private Transform center;
        [SerializeField] private GameObject bedObject;
        [SerializeField] private GameObject rainEffect;

        private bool isActing = false;

        public void Interact(Player player)
        {
            if (isActing || player == null) return;

            isActing = true;

            StartCoroutine(Routine(player));
        }

        public void Interactable(bool active)
        {
            var popup = UIManager.Instance?.GetUI<InteractPopup>();
            if (popup == null) return;

            if (isActing)
            {
                popup.Close();
                return;
            }

            if (active)
            {
                popup.Open();
            }
            else
            {
                popup.Close();
            }
        }

        private IEnumerator Routine(Player player)
        {
            float x;
            float y;

            float elapsedTime = 0f;
            float time = 1f;

            PlayerManager.Instance?.StopPlayerInput();

            float playerStartAngle = player.transform.eulerAngles.z;

            Vector2 startPos = player.transform.position;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                x = Mathf.Lerp(startPos.x, center.position.x, elapsedTime / time);
                y = Mathf.Lerp(startPos.y, center.position.y, elapsedTime / time);

                player.transform.Find("Visual").transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(playerStartAngle, 0, elapsedTime / time));
                player.transform.position = new Vector2(x, y);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            elapsedTime = 0;
            time = 1f;

            while (elapsedTime < time)
            {
                float intensity = Mathf.Lerp(1, 0, elapsedTime / time);
                if (globalLight != null)
                    globalLight.intensity = intensity;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            player.transform.Find("Visual").transform.eulerAngles = new Vector3(0, 0, 0);

            if (globalLight != null)
                globalLight.intensity = 0;

            SpriteRenderer playerSr = player.GetComponentInChildren<SpriteRenderer>();

            yield return new WaitForSeconds(0.5f);

            List<DialogData> dialogDatas = DialogManager.Instance?.GetLines("Boss1_1");

            var dialogCanvas = UIManager.Instance?.GetUI<DialogCanvas>();
            if (dialogCanvas != null && dialogDatas != null)
            {
                UIManager.Instance.ShowUI<DialogCanvas>();
                dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, null, false));
            }

            player.GetCompo<PlayerMover>()?.SetMoveType(PlayerMoveType.BATTLE);

            yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>()?.isFinished == true);

            bedObject.SetActive(false);

            SoundManager.Instance?.PlaySound("BGM_Rain_Loop");
            if (rainEffect != null)
                rainEffect.SetActive(true);

            dialogDatas = DialogManager.Instance?.GetLines("Boss1_2");

            if (dialogCanvas != null && dialogDatas != null)
            {
                UIManager.Instance.ShowUI<DialogCanvas>();
                dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, null, false));
            }

            yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>()?.isFinished == true);

            var room = GameManager.Instance?.GetCurrentRoom() as BossRoom;
            if (room != null)
            {
                room.ActiveTileMap(true, "PAST");
                room.ActiveTileMap(false, "ROOM");
            }

            Color playerColor = playerSr.color;
            Color newColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.7f);

            playerSr.color = newColor;

            player.transform.position = Vector3.zero;

            player.transform.GetComponentInChildren<CanvasGroup>().alpha = 0.7f;

            elapsedTime = 0;

            while (elapsedTime < time)
            {
                float intensity = Mathf.Lerp(0, 1, elapsedTime / time);
                if (globalLight != null)
                    globalLight.intensity = intensity;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (globalLight != null)
                globalLight.intensity = 1;

            yield return new WaitForSeconds(2f);

            dialogDatas = DialogManager.Instance?.GetLines("Boss1_3");

            if (dialogCanvas != null && dialogDatas != null)
            {
                UIManager.Instance.ShowUI<DialogCanvas>();
                dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, null, false));
            }

            yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>()?.isFinished == true);

            elapsedTime = 0;

            while (elapsedTime < time)
            {
                float intensity = Mathf.Lerp(1, 0, elapsedTime / time);
                if (globalLight != null)
                    globalLight.intensity = intensity;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (globalLight != null)
                globalLight.intensity = 0;

            if (rainEffect != null)
                rainEffect.SetActive(false);

            if (room != null)
            {
                room.ActiveTileMap(false, "PAST");
                room.ActiveTileMap(true, "BATTLE");
            }

            CameraManager.Instance.SetConfiner(room.GetBattleCollider());

            yield return new WaitForSeconds(0.5f);

            playerColor = playerSr.color;
            newColor = new Color(playerColor.r, playerColor.g, playerColor.b, 0.2f);

            playerSr.color = newColor;

            elapsedTime = 0;

            while (elapsedTime < time)
            {
                float intensity = Mathf.Lerp(0, 1, elapsedTime / time);
                if (globalLight != null)
                    globalLight.intensity = intensity;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (globalLight != null)
                globalLight.intensity = 1;

            yield return new WaitForSeconds(0.5f);

            dialogDatas = DialogManager.Instance?.GetLines("Boss1_4");

            if (dialogCanvas != null && dialogDatas != null)
            {
                UIManager.Instance.ShowUI<DialogCanvas>();
                dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, null, false));
            }

            yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>()?.isFinished == true);

            elapsedTime = 0;
            time = 1;

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / time;

                var boss = BossManager.Instance?.Boss;
                var renderer = boss?.GetCompo<AgentRenderer>(true)?.Renderer;

                if (renderer != null && renderer.material != null)
                    renderer.material.SetFloat("_Value", Mathf.Lerp(0, 1, t));


                yield return null;
            }
            UIManager.Instance.SetBossGaugeVisibility(true);

            dialogDatas = DialogManager.Instance?.GetLines("Boss1_5");

            if (dialogCanvas != null && dialogDatas != null)
            {
                UIManager.Instance.ShowUI<DialogCanvas>();
                dialogCanvas.StartDialogOpenRoutine(dialogDatas, () => dialogCanvas.StartDialogRoutine(dialogDatas, null, false));
            }

            yield return new WaitUntil(() => UIManager.Instance.GetUI<DialogCanvas>()?.isFinished == true);

            playerColor = playerSr.color;

            elapsedTime = 0;
            time = 0.5f;

            CanvasGroup canvasGroup =  player.transform.GetComponentInChildren<CanvasGroup>();

            while (elapsedTime < time)
            {
                newColor = new Color(playerColor.r, playerColor.g, playerColor.b, Mathf.Lerp(0.2f, 1, elapsedTime / time));
                playerSr.color = newColor;
                canvasGroup.alpha = Mathf.Lerp(0.2f, 1, elapsedTime / time);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            GameManager.Instance.IsBattle = true;
            PlayerManager.Instance?.StartPlayerInput();
            BossManager.Instance?.StartBT();

            yield return null;
        }
    }
}
