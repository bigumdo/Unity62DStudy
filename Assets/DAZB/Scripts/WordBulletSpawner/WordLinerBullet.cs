using System.Collections;
using UnityEngine;

namespace YUI.Bullets.WordBullets
{
    public class WordLinerBullet : WordBullet
    {
        protected override IEnumerator ShootRoutine()
        {
            SetRandomWord();
            SetRandomColor();
            SetRandomSize();

            while (true)
            {
                transform.Translate(Vector2.right * _speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
