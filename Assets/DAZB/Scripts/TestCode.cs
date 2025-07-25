using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.Agents.players;
using YUI.Cores;
using YUI.Dialogs;
using YUI.UI.DialogSystem;


namespace YUI
{
    public class TestCode : MonoBehaviour
    {
        public string key;
        private List<DialogData> dialogDatas;

        private void Update()
        {
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                dialogDatas = DialogManager.Instance.GetLines(key);

                /* foreach (DialogData dialogData in dialogDatas) {
                    Debug.Log($"Order: {dialogData.Order}, Line: {dialogData.Line}, LineType: {dialogData.LineType}, " +
                                $"LineDelay: {dialogData.LineDelay}, CharDelay: {dialogData.CharDelay}, " +
                                $"EffectOnPlay: {dialogData.EffectOnPlay}, TextColor: {dialogData.TextColor}, " +
                                $"Speecher: {dialogData.Speecher}, SpeecherImage: {dialogData.SpeecherImage}, " +
                                $"BlsOnLeft: {dialogData.BlsOnLeft}, CallbackEventType: {dialogData.CallbackEventType}" +
                                $", SpeecherVoice: {dialogData.SpeecherVoice}, PlayEffectSound: {dialogData.PlayEffectSound}" );
                }  */

                //PlayerManager.Instance.Player.GetCompo<PlayerHealth>().ApplyDamage(5);

                UIManager.Instance.ShowUI<DialogCanvas>();
                UIManager.Instance.GetUI<DialogCanvas>().StartDialogOpenRoutine(dialogDatas, () => UIManager.Instance.GetUI<DialogCanvas>().StartDialogRoutine(dialogDatas));
            }
        }
    }
}
