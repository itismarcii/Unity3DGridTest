using System;
using BuildSystem;
using Character.Enemy;
using GridSystem;
using Player;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        
        public static GameManager Instance { get; private set; }
        public static Camera Camera { get; private set; }

        private const int MAX_RAY_CAST_DISTANCE = 100;
        
        public uint CurrentLevel { get; private set; } = 0;

        [HideInInspector] public bool IsPaused = false;

        #region Worldstage Setter

        public enum WorldSetting
        {
            Menu,
            Create,
            Play,
            Transition
        }
        
        private WorldSetting _WorldStage;

        public WorldSetting WorldStage
        {
            get => _WorldStage;
            private set
            {
                _WorldStage = value;
                OnWorldSettingChange?.Invoke(_WorldStage);
            }
        }
        
        public Action OnLose;
        public Action<WorldSetting> OnWorldSettingChange;

        #endregion
        
        #region Game speed variables

        public static float DeltaTime { get; private set; }
        public float GameSpeed { get; private set; } = 1;
        private const float MAX_SPEED = 16f;
        private const float MIN_SPEED = 0.5f;

        #endregion

        #region Manager References

        public enum ManagerEnum : byte
        {
            GRID_MANAGER,
            INPUT_MANAGER,
            ENEMY_MANAGER,
            PLAYER_MANAGER,
            MENU_MANAGER,
            SCENE_MANAGER,
            BUILD_MANAGER,
            COMBAT_MANAGER,
            CAMERA_MANAGER
        }
        
        [SerializeField] private GridManager _GridManager;
        [SerializeField] private InputManager _InputManager;
        [SerializeField] private EnemyManager _EnemyManager;
        [SerializeField] private PlayerManager _PlayerManager;
        [SerializeField] private MenuUIManager _MenuUIManager;
        [SerializeField] private SceneTransitionManager _SceneManger;
        [SerializeField] private BuildManager _BuildManager;
        [SerializeField] private CombatManager _CombatManager;
        [SerializeField] private CameraManager _CameraManager;

        #endregion
        
#if UNITY_EDITOR
        private void OnValidate()
        {
             _GridManager ??= GetComponent<GridManager>();
             _InputManager ??= GetComponent<InputManager>();
             _EnemyManager ??= GetComponent<EnemyManager>();
             _PlayerManager ??= GetComponent<PlayerManager>();
             _MenuUIManager ??= GetComponent<MenuUIManager>();
             _SceneManger ??= GetComponent<SceneTransitionManager>();
             _CombatManager ??= GetComponent<CombatManager>();
             _CameraManager ??= GetComponent<CameraManager>();
        }
#endif
        
        private void Awake()
        {
            #region Singleton

            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            #endregion
            
            Camera = Camera.main;
            SetPlayer(new PlayerClass(_PlayerManager.CurrentPlayer));
            DisableAllManager();
            SetEnabledManger(ManagerEnum.SCENE_MANAGER, true);
            SetEnabledManger(ManagerEnum.INPUT_MANAGER, true);
            SubscribeToEnemyAmountChange(FinishStageCounter);
            OnWorldSettingChange += WorldChange;
        }

        private void FinishStageCounter(uint amount)
        {
            if (amount <= 0)
            {
                FinishStage();
            }
        }
        
        private void Update()
        {
            DeltaTime = !IsPaused ? Time.deltaTime * GameSpeed : 0;
        }

        #region Scenemanager logic

        public void LoadWorld()
        {
            var worldCreator = MenuUIManager.WorldCreatorUI;
            _GridManager.SetWorldConfig(new GridManager.Config 
            {
                WorldDimension = new Vector2Int(worldCreator.XDimension, worldCreator.YDimension)
            });
            
            _SceneManger.OnSceneTransitionFinish += GenerateWorld;
            _SceneManger.OnSceneTransitionFinish += SetupBuildingUI;
            _GridManager.OnBuildWorldFinished += SetupCameraForScene;
            
#if UNITY_EDITOR
            _SceneManger.TransitionScene(_SceneManger.SceneHolder.WorldScene.name);
#else
            _SceneManger.TransitionScene("WorldScene");
#endif
        }

        #endregion

        #region Manager setup logic

        private void SetEnabledManger(in ManagerEnum managerCode, in bool isEnabled)
        {
            switch (managerCode)
            {
                case ManagerEnum.GRID_MANAGER:
                    _GridManager.enabled = isEnabled;
                    break;
                case ManagerEnum.INPUT_MANAGER:
                    _InputManager.enabled = isEnabled;
                    break;
                case ManagerEnum.ENEMY_MANAGER:
                    _EnemyManager.enabled = isEnabled;
                    break;
                case ManagerEnum.PLAYER_MANAGER:
                    _PlayerManager.enabled = isEnabled;
                    break;
                case ManagerEnum.MENU_MANAGER:
                    _MenuUIManager.enabled = isEnabled;
                    break;
                case ManagerEnum.SCENE_MANAGER:
                    _SceneManger.enabled = isEnabled;
                    break;
                case ManagerEnum.BUILD_MANAGER:
                    _BuildManager.enabled = isEnabled;
                    break;
                case ManagerEnum.COMBAT_MANAGER:
                    _CombatManager.enabled = isEnabled;
                    break;
                case ManagerEnum.CAMERA_MANAGER:
                    _CameraManager.enabled = isEnabled;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(managerCode), managerCode, null);
            }
        }

        public void DisableAllManager()
        {
            if(_GridManager) _GridManager.enabled = false;
            if(_InputManager) _InputManager.enabled = false;
            if(_EnemyManager) _EnemyManager.enabled = false;
            if(_PlayerManager) _PlayerManager.enabled = false;
            if(_MenuUIManager) _MenuUIManager.enabled = false;
            if(_SceneManger) _SceneManger.enabled = false;
            if(_BuildManager) _BuildManager.enabled = false;
            if(_CombatManager) _CombatManager.enabled = false;
        }
        
        #endregion
        
        #region Gridmanager logic

        private void GenerateWorld()
        {
            SetEnabledManger(ManagerEnum.GRID_MANAGER, true);
            SetEnabledManger(ManagerEnum.PLAYER_MANAGER, true);
            WorldStage = WorldSetting.Create;
            _GridManager.BuildWorld();
            _SceneManger.OnSceneTransitionFinish -= GenerateWorld;
        }

        public bool CreateWorld()
        {
            SetEnabledManger(ManagerEnum.ENEMY_MANAGER, true);
            SetPlayer(_PlayerManager.CurrentPlayer);
            var paths = _GridManager.CreateWorld(out var error);
            
            if(error != "")
            {
                Debug.Log(error);
                return false;
            }

            WorldStage = WorldSetting.Transition;
            _GridManager.FinishMap();
            _EnemyManager.SetPaths(paths);

            return true;
        }

        #endregion

        #region System logic

        public void MenuReset()
        {
            SetPlayer(new PlayerClass(_PlayerManager.CurrentPlayer));
            DisableAllManager();
            SetEnabledManger(ManagerEnum.SCENE_MANAGER, true);
            SetEnabledManger(ManagerEnum.INPUT_MANAGER, true);
#if UNITY_EDITOR
            _SceneManger.TransitionScene(_SceneManger.SceneHolder.MainScene.name);
#else
            _SceneManger.TransitionScene("MainScene");
#endif
            

        }

        public void WorldReset()
        {
            throw new NotImplementedException();
        }
        
        public Vector3 GetHoverPosition()
        {
            var tile = _GridManager.HoverTilePosition();
            if (tile)  return tile.transform.position;

            var position = tile ? tile.transform.position : GetWorldHoverPosition();
            return new Vector3(position.x, -1, position.z);
        }

        public BuildingTile GetHoverBuildingTile()
        {
            var tile = _GridManager.MouseHoverTileObject();
            if (!tile) return null;
            
            return tile.Tile is BuildingTile buildingTile ? buildingTile : null;
        }

        private Vector3 GetWorldHoverPosition() =>
            Camera.ScreenPointToRay(Input.mousePosition).GetPoint(MAX_RAY_CAST_DISTANCE);

        private void WorldChange(WorldSetting stage)
        {
            switch (stage)
            {
                case WorldSetting.Menu:
                    break;
                case WorldSetting.Create:
                    break;
                case WorldSetting.Play:
                    IsPaused = false;
                    SetEnabledManger(ManagerEnum.ENEMY_MANAGER, true);
                    break;
                case WorldSetting.Transition:
                    IsPaused = true;
                    SetEnabledManger(ManagerEnum.ENEMY_MANAGER, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
            }
        }

        public void StartWave()
        {
            WorldStage = WorldSetting.Play;
            StartEnemyWave(CurrentLevel);
        }
        
        private void GameOver()
        {
            WorldStage = WorldSetting.Transition;
            OnLose?.Invoke();
            OnLose = null;
            OnWorldSettingChange = null;
            OnWorldSettingChange += WorldChange;
        }
        
        public void CloseGame()
        {
            SetEnabledManger(ManagerEnum.GRID_MANAGER, false);
            SetEnabledManger(ManagerEnum.ENEMY_MANAGER, false);
        }
        
        public void SpeedUp() => GameSpeed = GameSpeed * 2 > MAX_SPEED ? MAX_SPEED : GameSpeed * 2;
        public void SpeedDown() => GameSpeed = GameSpeed / 2 < MIN_SPEED ? MIN_SPEED : GameSpeed / 2;
        public void CloseApplication() => Application.Quit();

        public EnemyLevelInfo GetWaveInfo(in uint level) => _EnemyManager.GetLevelInfo(level);
        public EnemyLevelInfo GetNextWaveInfo() => GetWaveInfo(CurrentLevel + 1);

        #endregion
        
        #region Enemymanager logic

        public void SubscribeToEnemyAmountChange(Action<uint> action) => _EnemyManager.OnEnemyAmountChange += action;
        public void UnsubscribeToEnemyAmountChange(Action<uint> action) => _EnemyManager.OnEnemyAmountChange -= action;
        
        public void StartEnemyManager()
        {
            SetEnabledManger(ManagerEnum.ENEMY_MANAGER, true);
            _EnemyManager.LoadEnemyLevelInfo();
            _EnemyManager.OnEnemyDeath += RewardGold;
            OnLose += ResetEnemyManager;
        }

        public void SetLevel(in uint level) => CurrentLevel = level;

        // ReSharper disable Unity.PerformanceAnalysis
        public void StartEnemyWave(in uint level)
        { 
            _EnemyManager.OnEnemyDeath += RewardGold;
            _EnemyManager.StartEnemySpawn(level, out var error);
            _EnemyManager.OnEnemyAmountChange?.Invoke(_EnemyManager.EnemyAmount);
            
            if(error == "") return;
            Debug.LogWarning(error);
        }

        public void StartNextLevel() => StartEnemyWave(++CurrentLevel);

        public void StopEnemyManager()
        {
            SetEnabledManger(ManagerEnum.ENEMY_MANAGER, false);
            _EnemyManager.OnEnemyDeath -= RewardGold;
            ResetEnemyManager();
        }

        private void ResetEnemyManager()
        {
            OnLose -= ResetEnemyManager;
            CurrentLevel = 0;
        }

        #endregion

        #region Playermanager logic

        public void SetPlayer(in PlayerClass player)
        {
            _PlayerManager.CurrentPlayer = player;
            _PlayerManager.CurrentPlayer.OnPlayerDeath += GameOver;
        }
        
        private void RewardGold(uint goldReward) => _PlayerManager.CurrentPlayer.Gold += goldReward;

        public void HitEnemy(uint amount, EnemyClass enemy) => _PlayerManager.CurrentPlayer.Health -= amount;
        public PlayerClass GetPlayerClassInfo() => _PlayerManager.CurrentPlayer;

        #endregion

        #region Inputmanager logic

        //

        #endregion

        #region Buildingmanager logic

        private void SetupBuildingUI()
        {            
            _SceneManger.OnSceneTransitionFinish -= SetupBuildingUI;
            _BuildManager.SetParentObjectBuildingUI(_MenuUIManager.GetBuildingFrame());
            PopulateBuildingButtons();
        }
        
        public bool PlaceBuilding()
        {
            var goldCost = _BuildManager.GetCost();
            if (_PlayerManager.CurrentPlayer.Gold >= goldCost)
            {
                var position = GetHoverPosition();
                if (!_BuildManager.CanBePlaced(position)) return false;
                
                _PlayerManager.CurrentPlayer.Gold -= goldCost;
                var newBuilding = _BuildManager.Place(out var tiles);
                
                foreach (var tile in tiles)
                {
                    tile.SetBuildingTile(newBuilding);
                }

                newBuilding.SetupBuildingTiles(tiles);
                return true;
            }

            return false;
        }

        public bool PlaceBuildingFree(in BuildingTileGameObject building)
        {
            if (!building.CanBePlaced()) return false;

            _BuildManager.Place(building, out var tiles);

            foreach (var tile in tiles)
            {
                tile.SetBuildingTile(building);
            }

            building.SetupBuildingTiles(tiles);
            return true;
        }

        public void PopulateBuildingButtons() => _BuildManager.CreateButtons();
        public void SelectBuildingInfo(in int index)
        {
            DeselectBuildingInfo();
            _BuildManager.SelectBuilding(index);
        }

        public void DeselectBuildingInfo() => _BuildManager.DeselectBuilding();

        public void SetBuildingManager(in bool state) => _BuildManager.enabled = state;

        #endregion

        #region Cameramanager logic

        public void SetupCameraForScene()
        {
            Camera = Camera.main;
            _GridManager.OnBuildWorldFinished -= SetupCameraForScene; _CameraManager.SetCamera(Camera);
            _CameraManager.AdjustCameraPositionToGrid(_GridManager.transform, _GridManager.GetCurrentWorldDimension());
        }

        #endregion

        #region CombatManager logic

        private void FinishStage()
        {
            _EnemyManager.OnEnemyDeath -= RewardGold;
            StartNextLevel();
            WorldStage = WorldSetting.Transition;
        }
        
        #endregion
    }
}
