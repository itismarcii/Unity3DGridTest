using System;
using System.Collections.Generic;
using Combat;
using GridSystem;
using UnityEngine;

namespace BuildSystem
{
    public class BuildingTileGameObject : MonoBehaviour
    {
        [SerializeField] private TileIdentifierGameObject[] _Identifiers;
        [field: SerializeField] public TurretGameObject[] TurretGameObjects { get; private set; }
        [SerializeField] private Transform Body;
        
        public BuildingTile[] TileCache { get; private set; }
        public BuildingTile[] BuildingTiles { get; private set; }
        private MeshRenderer[] _BuildingMaterialRenderers;
        private Material[] _BuildingMaterialsMemory;
        public uint ID { get; private set; }

        private void Awake()
        {
            _BuildingMaterialRenderers = Body.GetComponentsInChildren<MeshRenderer>();

            var materialList = new List<Material>();

            foreach (var meshRenderer in _BuildingMaterialRenderers)
            {
                materialList.Add(meshRenderer.sharedMaterial);
            }

            _BuildingMaterialsMemory = materialList.ToArray();
        }

        public bool CanBePlaced()
        {
            var tileCache = new List<BuildingTile>();
            
            foreach (var identifier in _Identifiers)
            {
                var tile = identifier.Scan();
                
                if (!tile)
                {
                    TileCache = null;
                    return false;
                }

                if (tile.Tile is PathTile)
                {
                    TileCache = null;
                    return false;
                }

                if (tile.Tile is not BuildingTile buildingTile) return false;
                
                if (buildingTile.IsBlocked())
                {
                    TileCache = null;
                    return false;
                }

                tileCache.Add(buildingTile);
            }

            TileCache = tileCache.ToArray();
            return true;
        }

        public void SetupBuildingTiles(in BuildingTile[] tiles) => BuildingTiles = tiles;

        public void SetMaterial(in Material material)
        {
            foreach (var meshRenderer in _BuildingMaterialRenderers)
            {
                meshRenderer.sharedMaterial = material;
            }
        }

        public void ToBaseMaterial()
        {
            for (var index = 0; index < _BuildingMaterialRenderers.Length; index++)
            {
                _BuildingMaterialRenderers[index].sharedMaterial = _BuildingMaterialsMemory[index];
            }
        }

        public void SetID(in uint id) => ID = id;
    }
}
