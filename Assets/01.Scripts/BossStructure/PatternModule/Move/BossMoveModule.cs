using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using YUI.Agents.Bosses;

namespace YUI.PatternModules
{
    [CreateAssetMenu(fileName = "BossMoveModule", menuName = "SO/Boss/Module/Move/BossMoveModule")]
    public class BossMoveModule : PatternModule
    {
        [SerializeField] private int _horizontalTileCount = 5;
        [SerializeField] private int _verticalTileCount = 4;
        [Min(0)]
        [SerializeField] private int _selectTile = 1;
        [SerializeField] private float durationTime;

        private float _tileWidth;
        private float _tileHeight;
        private Vector3 _offset;

        Vector3 _bottomLeft;
        Vector3 _topRight;

        public override void Init(Boss boss)
        {
            base.Init(boss);
            float viewDistance = Camera.main.transform.position.y;

            _bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, viewDistance));
            _topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, viewDistance));

            float totalWidth = _topRight.x - _bottomLeft.x;
            float totalHeight = _topRight.z - _bottomLeft.z;

            _tileWidth = totalWidth / _horizontalTileCount;
            _tileHeight = totalHeight / _verticalTileCount;
            _offset = new Vector3(_tileWidth * 0.5f, 0, _tileHeight * 0.5f);
        }

        public override IEnumerator Execute()
        {
            int index = Mathf.Clamp(_selectTile - 1, 0, _horizontalTileCount * _verticalTileCount - 1);
            int tileZ = index / _horizontalTileCount;
            int tileX = index % _horizontalTileCount;

            float posX = _bottomLeft.x + (_offset.x + tileX * _tileWidth);
            float posZ = _topRight.z - (_offset.z + tileZ * _tileHeight);

            Vector3 worldPos = new Vector3(posX, 0, posZ);
            yield return _boss.GetCompo<BossMover>().DOMove(worldPos, durationTime);
            CompleteActionExecute();
        }

        [ContextMenu("TEST")]
        public void Test()
        {
            float viewDistance = Camera.main.transform.position.y;

            _bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, viewDistance));
            _topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, viewDistance));

            float totalWidth = _topRight.x - _bottomLeft.x;
            float totalHeight = _topRight.z - _bottomLeft.z;

            _tileWidth = totalWidth / _horizontalTileCount;
            _tileHeight = totalHeight / _verticalTileCount;
            _offset = new Vector3(_tileWidth * 0.5f, 0, _tileHeight * 0.5f);

            int index = Mathf.Clamp(_selectTile - 1, 0, _horizontalTileCount * _verticalTileCount - 1);
            int tileZ = index / _horizontalTileCount;
            int tileX = index % _horizontalTileCount;

            float posX = _bottomLeft.x + (_offset.x + tileX * _tileWidth);
            float posZ = _topRight.z - (_offset.z + tileZ * _tileHeight);

            Vector3 worldPos = new Vector3(posX, 0, posZ);
        }
    }
}

