using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YUI
{
    public struct TextChunk
    {
        public string text;
        public Color color;
        public int size;
        public bool bold;
    }

    public static class HighlightTextParser
    {
        private static Regex regex = new Regex(@"<(?<tag>\w+)(=(?<value>[^>]+))?>(?<text>.*?)</\k<tag>>");

        public static List<TextChunk> Parse(string input)
        {
            var chunks = new List<TextChunk>();
            int currentIndex = 0;

            foreach (Match match in regex.Matches(input))
            {
                int index = match.Index;

                if (index > currentIndex)
                {
                    string before = input.Substring(currentIndex, index - currentIndex);
                    chunks.Add(new TextChunk { text = before, color = Color.white, size = 14, bold = false });
                }

                string tag = match.Groups["tag"].Value;
                string value = match.Groups["value"].Value;
                string content = match.Groups["text"].Value;

                var chunk = new TextChunk { text = content, color = Color.white, size = 14, bold = false };

                switch (tag.ToLower())
                {
                    case "color":
                        if (ColorUtility.TryParseHtmlString(value, out var c))
                            chunk.color = c;
                        break;
                    case "highlight":
                        chunk.color = Color.yellow;
                        break;
                    case "b":
                        chunk.bold = true;
                        break;
                    case "size":
                        if (int.TryParse(value, out var s))
                            chunk.size = s;
                        break;
                }

                chunks.Add(chunk);
                currentIndex = match.Index + match.Length;
            }

            if (currentIndex < input.Length)
            {
                string tail = input.Substring(currentIndex);
                chunks.Add(new TextChunk { text = tail, color = Color.white, size = 14, bold = false });
            }

            return chunks;
        }
    }

}
