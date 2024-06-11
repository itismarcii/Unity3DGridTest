using System;
using Manager;
using UnityEngine;

namespace GridSystem
{
    public class TileGameObject : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _Renderer;
        private Material _BaseMaterial;
        public GridTile Tile { get; private set; }
        
        public void Setup(in GridTile tile)
        {
            Tile = tile;
            
            _Renderer.material = tile switch
            {
                BuildingTile buildingTile => GridManager.GridTileInfos.BuildingGridMaterial,
                PathTile pathTile => GridManager.GridTileInfos.PathGridMaterial,
                _ => GridManager.GridTileInfos.DefaultGridMaterial,
            };
            
            _BaseMaterial = _Renderer.material;
        }

        public void ResetTile()
        {
            Tile = null;
        }
        
        public void Hover(in Material material)
        {
            _Renderer.material = material;
        }

        public void LeaveHover() => _Renderer.material = _BaseMaterial;

        public bool PlaceTile(in GridTile gridTile)
        {            
            GridManager.GridField[Tile.ID] = gridTile;
            Tile = gridTile;
            _Renderer.material = GridManager.GridTileInfos.DefaultGridMaterial;
            _BaseMaterial = GridManager.GridTileInfos.DefaultGridMaterial;
            return true;
        }
        
        public bool PlaceTile(in PathTile gridTile)
        {
            if (Tile is PathTile tile)
            {
                if(tile.TileType == gridTile.TileType) return false;
            } 
            
            GridManager.GridField[Tile.ID] = gridTile;
            Tile = gridTile;
            
            _Renderer.material = gridTile.TileType switch
            {
                PathTile.Type.Default => GridManager.GridTileInfos.PathGridMaterial,
                PathTile.Type.Spawn => GridManager.GridTileInfos.SpawnPathGridMaterial,
                PathTile.Type.Finish => GridManager.GridTileInfos.FinishPathGridMaterial,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _BaseMaterial = _Renderer.material;
            return true;
        }
        
        public bool PlaceTile(in BuildingTile gridTile)
        {
            if(Tile is BuildingTile) return false;
            Tile = gridTile;
            _Renderer.material = GridManager.GridTileInfos.BuildingGridMaterial;
            _BaseMaterial = GridManager.GridTileInfos.BuildingGridMaterial;
            return true;
        }
    }
}
