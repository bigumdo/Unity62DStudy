using System;
using System.Collections.Generic;
using UnityEngine;

namespace YUI.Cores
{
    [System.Serializable]
    public struct CursorData
    {
        public string cursorName;
        public Texture2D cursorTexture;
    }

    public class MouseManager : MonoSingleton<MouseManager>
    {
        public Vector2 MouseDir { get; private set; }
        public List<CursorData> mouseCursor = new List<CursorData>();
        private Dictionary<string,Texture2D> cursorDictionary;

        private void Awake()
        {
            cursorDictionary = new Dictionary<string, Texture2D>();
            mouseCursor.ForEach(cursor => cursorDictionary.Add(cursor.cursorName, cursor.cursorTexture));
            SetCursor("Shoot");
        }

        private void Update()
        {
            MouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public void SetCursor(string cursorName)
        {
            Texture2D changeCursor = cursorDictionary.GetValueOrDefault(cursorName);
            Vector2 cursorPoint = new Vector2(changeCursor.width/2, changeCursor.height/2);
            Cursor.SetCursor(changeCursor, cursorPoint, CursorMode.ForceSoftware);
        }
    }
}
