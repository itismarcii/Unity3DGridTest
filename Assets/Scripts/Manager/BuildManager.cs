using System;
using System.Collections.Generic;
using BuildSystem;
using Combat;
using GridSystem;
using UnityEngine;

namespace Manager
{
    public class BuildManager : MonoBehaviour
    {
        public static readonly List<TurretGameObject> PlacedBuildingTurrets = new();
        [SerializeField] private BuildingInfoScriptableObject _BuildingInfo;
        public BuildingScriptableObject[] Buildings { get; private set; }
        [SerializeField] public BuildingButton ButtonPrefab;
        private Transform _ParentObjectBuildingUI;
        public int SelectedBuildingID { get; private set; } = -1;
        
        private BuildingTileGameObject _SelectedBuilding;
        private BuildingTileGameObject[] _PlaceBuildingCaches;

        public Action<bool> OnBuildManagerActiveToggle;

        
        private void Awake()
        {
            Buildings = Resources.LoadAll<BuildingScriptableObject>("Helper/Building/Objects");
        }

        private void OnEnable()
        {
            OnBuildManagerActiveToggle?.Invoke(true);
            if (_SelectedBuilding) _SelectedBuilding.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (_SelectedBuilding) _SelectedBuilding.gameObject.SetActive(false);
            
            OnBuildManagerActiveToggle?.Invoke(false);
            SelectedBuildingID = -1;
            
            foreach (var placeBuildingCache in _PlaceBuildingCaches)
            {
                if(!placeBuildingCache) continue;
                placeBuildingCache.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (DeplaceGridBuilding(out var mouseClick) || _SelectedBuilding)
            {
                if(mouseClick) return;
                
                RotateHoverBuilding();
                if(!HoverBuilding(_SelectedBuilding, out var pos)) return;
                if (InputManager.IsMainAction)
                {
                    if(!GameManager.Instance.PlaceBuildingFree(_SelectedBuilding)) return;
                }
                else if (InputManager.IsSecondaryAction)
                {
                    DestroyBuilding(_SelectedBuilding);
                } else return;
                
                _SelectedBuilding = null;
                return;
            }
            
            if(SelectedBuildingID < 0) return;
            RotateHoverBuilding();
            if(!HoverBuilding()) return;
            if(InputManager.IsMainAction) GameManager.Instance.PlaceBuilding();
        }

        private void DestroyBuilding(in BuildingTileGameObject building)
        {
            foreach (var turret in building.TurretGameObjects)
            {
                PlacedBuildingTurrets.Remove(turret);
            }

            Destroy(building.gameObject);
        }

        private bool DeplaceGridBuilding(out bool blockMouseClick)
        {           
            blockMouseClick = false;
            
            if (_SelectedBuilding) return false;
            
            var buildingTile = GameManager.Instance.GetHoverBuildingTile();
            
            if (buildingTile == null) return false;
            if (!InputManager.IsMainAction) return false;

            blockMouseClick = true;
            
            _SelectedBuilding = buildingTile.BuildingTileGameObject;
            if (!_SelectedBuilding) return false;

            foreach (var tile in _SelectedBuilding.BuildingTiles)
            {
                tile.UnassignBuildingTile();
            }

            return true;
        }
        
        public void MoveBuilding(in BuildingTileGameObject buildingTileGameObject) =>
            _SelectedBuilding = buildingTileGameObject;

        public void RotateTo(in Quaternion rotation)
        {
            _PlaceBuildingCaches[SelectedBuildingID].transform.rotation = rotation;
        }
        
        private void RotateHoverBuilding()
        {
            if(Input.GetKeyDown(KeyCode.Q)) RotateBuildingRight();
            else if(Input.GetKeyDown(KeyCode.E)) RotateBuildingLeft();
        }

        public void SetParentObjectBuildingUI(in Transform parent) => _ParentObjectBuildingUI = parent;

        public BuildingTileGameObject SelectBuilding(in int index)
        {
            SelectedBuildingID = index;
            var selected = _PlaceBuildingCaches[index];
            selected.gameObject.SetActive(true);
            
            foreach (var turrets in selected.TurretGameObjects)
            {
                turrets.Select();
            }
            
            return selected;
        }

        public BuildingScriptableObject SelectedBuilding() => Buildings[SelectedBuildingID];

        public void DeselectBuilding()
        {
            if(SelectedBuildingID < 0) return;
            
            _PlaceBuildingCaches[SelectedBuildingID].gameObject.SetActive(false);
            foreach (var turrets in _PlaceBuildingCaches[SelectedBuildingID].TurretGameObjects)
            {
                turrets.Deselect();
            }
        }

        public BuildingButton[] CreateButtons()
        {
            var list = new List<BuildingButton>();

            if (_PlaceBuildingCaches != null)
            {
                foreach (var cacheObject in _PlaceBuildingCaches)
                {
                    if(cacheObject) Destroy(cacheObject.gameObject);
                }
            }
            
            _PlaceBuildingCaches = new BuildingTileGameObject[Buildings.Length];

            for (var i = 0; i < Buildings.Length; i++)
            {
                ButtonPrefab.CreateButton(_ParentObjectBuildingUI, i);
                
                var building = Buildings[i];
                _PlaceBuildingCaches[i] = Instantiate(building.Prefab);
                _PlaceBuildingCaches[i].gameObject.SetActive(false);
            }
            
            return list.ToArray();
        }

        public BuildingTileGameObject Place(out BuildingTile[] tiles)
        {
            tiles = _PlaceBuildingCaches[SelectedBuildingID].TileCache;
            var building = Instantiate(Buildings[SelectedBuildingID].Prefab);
            var position = _PlaceBuildingCaches[SelectedBuildingID].transform.position;
            building.transform.position = new Vector3(position.x, 0, position.z);
            building.transform.rotation = _PlaceBuildingCaches[SelectedBuildingID].transform.rotation;
            PlacedBuildingTurrets.AddRange(building.TurretGameObjects);

            OnBuildManagerActiveToggle += (x) =>
            {
                foreach (var turret in building.TurretGameObjects)
                {
                    turret.ToggleRangeIdenticator(x);
                }
            };

            SelectedBuildingID = -1;
            return building;
        }
        
        public void Place(BuildingTileGameObject building, out BuildingTile[] tiles)
        {
            tiles = building.TileCache;
            var pos = building.transform.position;
            building.transform.position = new Vector3(pos.x, 0, pos.z);

            OnBuildManagerActiveToggle += (x) =>
            {
                foreach (var turret in building.TurretGameObjects)
                {
                    turret.ToggleRangeIdenticator(x);
                }
            };
        }

        public bool HoverBuilding()
        {
            var pos = GameManager.Instance.GetHoverPosition();
            if(pos.y < 0) return false;
            var building = _PlaceBuildingCaches[SelectedBuildingID];
            building.transform.position = new Vector3(pos.x, 0.1f, pos.z);

            var canBePlaced= building.CanBePlaced();
            building.SetMaterial(canBePlaced
                ? _BuildingInfo.PlaceHoverMaterial
                : _BuildingInfo.CantPlaceMaterial);

            return canBePlaced;
        }
        
        public bool HoverBuilding(in BuildingTileGameObject buildingTile, out Vector3 pos)
        {
            pos = GameManager.Instance.GetHoverPosition();
            if (pos.y < -0.01f) return false;
                
            buildingTile.transform.position = new Vector3(pos.x, 0.1f, pos.z);
            
            var canBePlaced= buildingTile.CanBePlaced();
            buildingTile.SetMaterial(canBePlaced
                ? _BuildingInfo.PlaceHoverMaterial
                : _BuildingInfo.CantPlaceMaterial);

            return canBePlaced;
        }

        public uint GetCost() => Buildings[SelectedBuildingID].GoldCost;

        public bool CanBePlaced(in Vector3 position)
        {
            var placeBuilding = _PlaceBuildingCaches[SelectedBuildingID];
            placeBuilding.gameObject.transform.position = new Vector3(position.x, 0.1f, position.z);;
            return placeBuilding.CanBePlaced();
        }

        public void RotateBuildingLeft()
        {
            var placeBuilding = _SelectedBuilding ? _SelectedBuilding : _PlaceBuildingCaches[SelectedBuildingID];
            var euler = placeBuilding.gameObject.transform.eulerAngles;
            placeBuilding.gameObject.transform.rotation = Quaternion.Euler(euler.x, euler.y + 90, euler.z);
        }

        public void RotateBuildingRight()
        {
            var placeBuilding = _SelectedBuilding ? _SelectedBuilding : _PlaceBuildingCaches[SelectedBuildingID];
            var euler = placeBuilding.gameObject.transform.eulerAngles;
            placeBuilding.gameObject.transform.rotation = Quaternion.Euler(euler.x, euler.y + -90, euler.z);
        }
    }
}
