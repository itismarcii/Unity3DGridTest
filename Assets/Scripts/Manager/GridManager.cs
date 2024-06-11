using System;
using System.Collections.Generic;
using BuildSystem;
using GridSystem;
using UnityEngine;

namespace Manager
{
    public class GridManager : MonoBehaviour
    {
        public struct Config
        {
            public Vector2Int WorldDimension;
        }

        private const float MAX_RAY_CAST_DISTANCE = 100f;
        
        internal static GridBuilderRectangle GridBuilder;
        [SerializeField] private GridTileInfosScriptableObject _GridTileInfos;
        public static Dictionary<Vector2Int, GridTile> GridField = new();
        public static Dictionary<Vector2Int, PathTile> SpawnTiles = new();
        public static Dictionary<Vector2Int, PathTile> FinishTiles = new();
        internal static GridTileInfosScriptableObject GridTileInfos;
        
        private LayerMask _LayerMask;
        private Config _WorldConfig;
        private TileGameObject _CurrentHoverTile;
        public Action OnBuildWorldFinished;

        
        private void Awake()
        {
            _LayerMask = LayerMask.GetMask("GridTile");
            GridTileInfos = _GridTileInfos;
            
            GridField.Clear();
            SpawnTiles.Clear();
            FinishTiles.Clear();
        }
        
        private void Update()
        {
            UpdateStage();
        }

        private void UpdateStage()
        {
            switch (GameManager.Instance.WorldStage)
            {
                case GameManager.WorldSetting.Create:
                    CreateStage();
                    return;
                case GameManager.WorldSetting.Play:
                    return;
                case GameManager.WorldSetting.Transition:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public TileGameObject MouseHoverTileObject()
        {
            var ray = GameManager.Camera.ScreenPointToRay(Input.mousePosition);

            return Physics.Raycast(ray, out var hit, MAX_RAY_CAST_DISTANCE, _LayerMask)
                ? GridBuilderRectangle.GridFieldDictionary.GetValueOrDefault(hit.transform)
                : null;
        }
        
        #region Create Stage

        private void CreateStage()
        {
            if (GridBuilder.SelectedTileFormat == GridBuilderRectangle.TileFormat.None) return;
            HoverBuildingCreateStage();
            if(!_CurrentHoverTile) return;
            
            if (InputManager.IsMainAction)
            {
                PlaceTile();
            }
        }

        private void PlaceTile()
        {
            Action<Vector2Int> onSpecialCase = null;
            
            if (_CurrentHoverTile.Tile is PathTile tile)
            {
                switch (tile.TileType)
                {
                    case PathTile.Type.Spawn:
                        onSpecialCase += (id) => SpawnTiles.Remove(id);
                        break;
                    case PathTile.Type.Finish:
                        onSpecialCase += (id) => FinishTiles.Remove(id);
                        break;
                }
            }
            
            switch (GridBuilder.SelectedTileFormat)
            {
                case GridBuilderRectangle.TileFormat.None:
                    return;
                case GridBuilderRectangle.TileFormat.Path:
                    var pTile = new PathTile(_CurrentHoverTile.Tile, PathTile.Type.Default);
                    if (_CurrentHoverTile.PlaceTile(pTile))
                    {
                        GridField[pTile.ID] = pTile;
                        onSpecialCase?.Invoke(pTile.ID);
                    }
                    break;
                case GridBuilderRectangle.TileFormat.Building:
                    var bTile = new BuildingTile(_CurrentHoverTile.Tile);
                    if (_CurrentHoverTile.PlaceTile(bTile))
                    {
                        GridField[bTile.ID] = bTile;
                        onSpecialCase?.Invoke(bTile.ID);
                    }
                    break;
                case GridBuilderRectangle.TileFormat.Spawn:
                    var sTile = new PathTile(_CurrentHoverTile.Tile, PathTile.Type.Spawn);
                    if (_CurrentHoverTile.PlaceTile(sTile))
                    {
                        GridField[sTile.ID] = sTile;
                        SpawnTiles[sTile.ID] = sTile;
                        onSpecialCase?.Invoke(sTile.ID);
                    }
                    break;
                case GridBuilderRectangle.TileFormat.Finish:
                    var fTile = new PathTile(_CurrentHoverTile.Tile, PathTile.Type.Finish);
                    if (_CurrentHoverTile.PlaceTile(fTile))
                    {
                        GridField[fTile.ID] = fTile;
                        FinishTiles[fTile.ID] = fTile;
                        onSpecialCase?.Invoke(fTile.ID);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public List<List<Vector2Int>> CreateWorld(out string error)
        {
            error = "";
            
            if (SpawnTiles.Count <= 0 || FinishTiles.Count <= 0)
            {
                error = "No start or finish.";
                return null;
            }

            var paths = new List<List<Vector2Int>>();
            
            foreach (var startTile in SpawnTiles)
            {
                foreach (var finishTile in FinishTiles)
                {
                    if (!PathFinderHandler.HasAValidPath(startTile.Value, finishTile.Value, out var path))
                    {
                        error = "Not all start lead to a finish.";
                        return null;
                    }
                    
                    paths.Add(path);
                }
            }
            
            return paths;
        }
        
        public void BuildWorld()
        {
            GridBuilder.BuildGrid(_WorldConfig.WorldDimension);
            OnBuildWorldFinished?.Invoke();
        }

        internal void SetWorldConfig(in Config worldConfig) => _WorldConfig = worldConfig;

        private GridTile MouseHoverTileInfo()
        {
            var ray = GameManager.Camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, MAX_RAY_CAST_DISTANCE, _LayerMask)) return null;

            return GridBuilderRectangle.GridFieldDictionary.TryGetValue(hit.transform, out var tileGameObject)
                ? tileGameObject.Tile
                : null;
        }
        
        public bool HoverBuildingCreateStage()
        {
            var gridTile = MouseHoverTileObject();
            if (!gridTile)
            {
                _CurrentHoverTile?.LeaveHover();
                _CurrentHoverTile = null;
                return false;
            }

            if (gridTile == _CurrentHoverTile) return true;
           
            _CurrentHoverTile?.LeaveHover();
            _CurrentHoverTile = gridTile;

            var hoverMaterial = GridBuilder.SelectedTileFormat switch
            {
                GridBuilderRectangle.TileFormat.Building => GridTileInfos.BuildingGridMaterial,
                GridBuilderRectangle.TileFormat.Path => GridTileInfos.PathGridMaterial,
                GridBuilderRectangle.TileFormat.Spawn => GridTileInfos.SpawnPathGridMaterial,
                GridBuilderRectangle.TileFormat.Finish => GridTileInfos.FinishPathGridMaterial,
                _ => GridTileInfos.DefaultGridMaterial
            };
            
            _CurrentHoverTile.Hover(hoverMaterial);
            return true;
        }

        public void FinishMap()
        {
            foreach (var tile in GridBuilder.TileGameObjectCache)
            {
                if (tile.Tile is PathTile or BuildingTile) continue;
                tile.PlaceTile(new BuildingTile(tile.Tile));
            }
        }

        #endregion

        #region Transition Stage

        public void HoverBuilding(in BuildingTileGameObject building)
        {
            var tile = MouseHoverTileObject();

            if (tile == null)
            {
                var pos = GameManager.Camera.ScreenPointToRay(Input.mousePosition).GetPoint(0);
                building.transform.position = new Vector3(pos.x, 0, pos.y);
            }
            else
            {
                building.transform.position = tile.transform.position;
            }
        }

        public TileGameObject HoverTilePosition() => MouseHoverTileObject();

        #endregion

        public Vector2Int GetCurrentWorldDimension() => GridBuilder.WorldDimension;
        
    }
}
