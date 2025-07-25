using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YUI.Dialogs {
    public class CSVParser {
        private const int ExpectedFieldCount = 14;

        public static Dictionary<string, List<DialogData>> Parse(string data) {
            Dictionary<string, List<DialogData>> dataDict = new Dictionary<string, List<DialogData>>();
            string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 2; i < lines.Length; i++) {
                string line = lines[i];
                
                string pattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";
                string[] values = Regex.Split(line, pattern);

                if (string.IsNullOrWhiteSpace(values[0]) || values.Length < ExpectedFieldCount) {
                    Debug.LogWarning($"[CSVParser] Skipping malformed or empty key line {i + 1}: {line}");
                    continue;
                }

                for (int j = 0; j < values.Length; j++) {
                    values[j] = values[j].Trim();
                }

                if (!int.TryParse(values[1], out int order)) {
                    Debug.LogWarning($"[CSVParser] Invalid Order at line {i + 1}: {values[1]}");
                    continue;
                }

                DialogData dialogData = new DialogData
                {
                    Order = order,
                    Line = values[2],
                    LineType = TryParseInt(values[3]),
                    LineDelay = TryParseFloat(values[4]),
                    CharDelay = TryParseFloat(values[5]),
                    EffectOnPlay = TryParseInt(values[6]),
                    TextColor = TryParseColor(values[7]),
                    Speecher = values[8],
                    CallbackEventType = values[9],
                    SpeecherVoice = values[10],
                    PlayEffectSound = values[11],
                    DialogType = TryParseInt(values[12]),
                    ChoiceText_1 = values[13],
                    ChoiceEvent_1 = values[14],
                    ChoiceEventString_1 = values[15],
                    ChoiceText_2 = values[16],
                    ChoiceEvent_2 = values[17],
                    ChoiceEventString_2 = values[18]
                };

                if (!dataDict.ContainsKey(values[0])) {
                    dataDict[values[0]] = new List<DialogData>();
                }

                dataDict[values[0]].Add(dialogData);
            }

            return dataDict;
        }

        private static int TryParseInt(string input, int defaultValue = 0) => int.TryParse(input, out var result) ? result : defaultValue;
        private static float TryParseFloat(string input, float defaultValue = 0f) => float.TryParse(input, out var result) ? result : defaultValue;
        private static bool TryParseBool(string input, bool defaultValue = false) => bool.TryParse(input, out var result) ? result : defaultValue;
        
        private static Color TryParseColor(string input)
        {
            input = input.Trim().Trim('"'); // 따옴표와 공백 제거

            if (ColorUtility.TryParseHtmlString(input, out var color))
                return color;

            var parts = input.Split(',');
            if (parts.Length == 3 &&
                float.TryParse(parts[0], out float r) &&
                float.TryParse(parts[1], out float g) &&
                float.TryParse(parts[2], out float b))
            {
                return new Color(r, g, b, 1f);
            }

            return Color.white;
        }
    }
}
