using System.Collections.Generic;
using UnityEngine;
using YUI.Cores;

namespace YUI.Dialogs {
    public struct DialogData
    {
        public int Order;
        public string Line;
        public int LineType;
        public float LineDelay;
        public float CharDelay;
        public int EffectOnPlay;
        public Color TextColor;
        public string Speecher;
        public string CallbackEventType;
        public string SpeecherVoice;
        public string PlayEffectSound;
        public int DialogType;
        public string ChoiceText_1;
        public string ChoiceEvent_1;
        public string ChoiceEventString_1;
        public string ChoiceText_2;
        public string ChoiceEvent_2;
        public string ChoiceEventString_2; 
    }

    public class DialogManager : MonoSingleton<DialogManager>
    {
        private Dictionary<string, List<DialogData>> dialogDict;
        private Dictionary<string, Sprite> speecherImageDict = new Dictionary<string, Sprite>();

        [SerializeField] private TextAsset csvFile;

        private void Awake()
        {
            dialogDict = CSVParser.Parse(csvFile.text);

        }

        public List<DialogData> GetLines(string key)
        {
            if (dialogDict.ContainsKey(key))
            {
                return dialogDict[key];
            }
            return null;
        }
        
        public Sprite GetSpeecherImage(string path)
        {
            if (speecherImageDict.ContainsKey(path))
            {
                return speecherImageDict[path];
            }
            return null;
        }
    }
}
