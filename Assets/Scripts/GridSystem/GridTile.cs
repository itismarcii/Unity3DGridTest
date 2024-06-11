using UnityEngine;

namespace GridSystem
{
    public class GridTile
    {
        public Vector2Int ID { get; protected set; }
        public Vector2 WorldPositionXZ { get; private set; }

        public GridTile(in Grid grid, in int widthPos, in int heightPos)
        {
            ID = new Vector2Int(widthPos, heightPos);
            var pos = grid.GetCellCenterWorld(new Vector3Int(widthPos, 0, heightPos));
            WorldPositionXZ = new Vector2(pos.x, pos.z);
        }

        public GridTile(in GridTile tile)
        {
            ID = tile.ID;
            WorldPositionXZ = tile.WorldPositionXZ;
        }
    }
}
