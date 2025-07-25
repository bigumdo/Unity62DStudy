using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YUI.PatternModules;

namespace YUI
{
    [CreateAssetMenu(fileName = "WordVerticalShot", menuName = "SO/Boss/Module/Spawn/WordVerticalShot")]
    public class WordVerticalShotModule : PatternModule
    {
        [SerializeField] private Vector2 rangeOffset;
        [SerializeField] private float rangeY;

        [Space, Space]

        [SerializeField] private float minSpeed;
        [SerializeField] private float maxSpeed;

        [Space, Space]

        [SerializeField] private List<string> wordList;
        [SerializeField] private Color minColor;
        [SerializeField] private Color maxColor;

        [Space, Space]

        [SerializeField] private float minFontSize;
        [SerializeField] private float maxFontSize;
        [SerializeField] private float damage;


        public override IEnumerator Execute()
        {
            yield return null;
        }
    }
}
