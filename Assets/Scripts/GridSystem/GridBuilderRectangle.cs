using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace GridSystem
{
    [RequireComponent(typeof(Grid))]
    public class GridBuilderRectangle : MonoBehaviour
    {
        [Serializable]
        public enum TileFormat
        {
            None,
            Path,
            Building,
            Spawn,
            Finish
        }
        
        internal readonly List<TileGameObject> TileGameObjectCache = new();
        public static readonly Dictionary<Transform, TileGameObject> GridFieldDictionary = new();
        
        public const int MIN_WORLD_DIMENSION = 5;
        public const int MAX_WORLD_DIMENSION = 35;
        
        [field: SerializeField] public Grid Grid { get; private set; }
        [field: SerializeField] public TileGameObject TilePrefab;
        [field: SerializeField] public Vector2Int WorldDimension;
        
        public TileFormat SelectedTileFormat { get; private set; } = TileFormat.None;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            Grid = Grid == null ? GetComponent<Grid>() : Grid;
            Grid.cellLayout = GridLayout.CellLayout.Rectangle;
            
            WorldDimension = new Vector2Int(
                WorldDimension.x < MIN_WORLD_DIMENSION ? MIN_WORLD_DIMENSION : WorldDimension.x,
                WorldDimension.y < MIN_WORLD_DIMENSION ? MIN_WORLD_DIMENSION : WorldDimension.y
            );
        }
#endif

        private void Awake()
        {
            GridManager.GridBuilder = this;
        }
        
        public void Setup(in GridManager.Config config) => WorldDimension = config.WorldDimension;
        
        private void BuildGrid()
        {
            GridFieldDictionary.Clear();
            GridManager.GridField.Clear();
            
            var rows = (int)(WorldDimension.x / Grid.cellSize.x);
            var cols = (int)(WorldDimension.y / Grid.cellSize.y);

            var totalTilesNeeded = rows * cols;

            for (var col = 0; col < cols; col++)
            {
                for (var row = 0; row < rows; row++)
                {
                    var index = col * rows + row;

                    if (index >= TileGameObjectCache.Count)
                    {
                        var tileObject = Instantiate(TilePrefab, transform);
                        TileGameObjectCache.Add(tileObject);
                    }

                    var cachedTileObject = TileGameObjectCache[index];
                    cachedTileObject.gameObject.SetActive(true);
                    cachedTileObject.transform.position = Grid.GetCellCenterWorld(new Vector3Int(row, 0, col));
                    var gridTile = new GridTile(Grid, row, col);
                    cachedTileObject.Setup(gridTile);
                    GridManager.GridField.Add(gridTile.ID, gridTile);
                    cachedTileObject.name = cachedTileObject.Tile.ID.ToString();
                    GridFieldDictionary.Add(cachedTileObject.transform, cachedTileObject);
                }
            }

            for (var i = totalTilesNeeded; i < TileGameObjectCache.Count; i++)
            {
                TileGameObjectCache[i].gameObject.SetActive(false);
            }
        }

        public void BuildGrid(Vector2Int dimensions)
        {
            WorldDimension = new Vector2Int
            {
                x = dimensions.x < MIN_WORLD_DIMENSION ? MIN_WORLD_DIMENSION : 
                    dimensions.x > MAX_WORLD_DIMENSION ? MAX_WORLD_DIMENSION : dimensions.x,
                y = dimensions.y < MIN_WORLD_DIMENSION ? MIN_WORLD_DIMENSION : 
                    dimensions.y > MAX_WORLD_DIMENSION ? MAX_WORLD_DIMENSION : dimensions.y,
            };
            
            BuildGrid();
        }

        #region UI Selection

        public void SelectPathTileFormat() => SelectedTileFormat = TileFormat.Path;
        public void SelectBuildingTileFormat() => SelectedTileFormat = TileFormat.Building;
        public void SelectSpawnTileFormat() => SelectedTileFormat = TileFormat.Spawn;
        public void SelectFinishTileFormat() => SelectedTileFormat = TileFormat.Finish;

        #endregion
    }
}
