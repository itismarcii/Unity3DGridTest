using System;
using BuildSystem;
using UnityEngine;

namespace GridSystem
{
    [Serializable]
    public class BuildingTile : GridTile
    {
        public BuildingTileGameObject BuildingTileGameObject { get; private set; } = null;
        
        public bool IsBlocked() => BuildingTileGameObject;
        public void AssignBuildingTile(BuildingTileGameObject tile) => BuildingTileGameObject = tile;
        public void UnassignBuildingTile() => BuildingTileGameObject = null;
        
        public BuildingTile(in GridTile tile) : base(in tile)
        {
        }

        public void SetBuildingTile(in BuildingTileGameObject building) => BuildingTileGameObject = building;
    }
}
