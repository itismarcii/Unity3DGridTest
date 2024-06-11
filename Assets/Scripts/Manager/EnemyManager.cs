using System;
using System.Collections.Generic;
using Character.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class EnemyManager : MonoBehaviour
    {
        protected class SpawnContainer
        {
            public EnemyInfoScriptableObject Enemy { get; private set; }
            public uint Amount;
            public float CurrentTime;
            public float Interval { get; }

            public SpawnContainer(Character.Enemy.EnemyInfoScriptableObject enemy, uint amount, float interval)
            {
                Enemy = enemy;
                Amount = amount;
                Interval = interval;
                CurrentTime = 0;
            }
        }
        
        public static readonly Dictionary<uint, List<EnemyGameObject>> ActiveEnemies = new();
        private List<SpawnContainer> _SpawnContainers;
        private List<List<Vector2Int>> _EnemyPaths;
        private EnemyLevelInfoScriptableObject _LevelInfo;

        private uint _EnemyAmount;
        public Action<uint> OnEnemyDeath, OnEnemyAmountChange;
        public static bool Spawning { get; private set; }= false;
        
        public uint EnemyAmount
        {
            get => _EnemyAmount;
            set
            {
                _EnemyAmount = value;
                OnEnemyAmountChange?.Invoke(_EnemyAmount);
            }
        }

        private void Awake()
        {
            LoadEnemyLevelInfo();
            ActiveEnemies.Clear();
        }

        private void Start()
        {
            GameManager.Instance.OnWorldSettingChange += ToggleManager;
        }

        private void ToggleManager(GameManager.WorldSetting stage)
        {
            enabled = stage == GameManager.WorldSetting.Play;
        }

        private void Update()
        {
            if (!Spawning) return;
            
            var removeRange = new List<SpawnContainer>();

            foreach (var container in _SpawnContainers)
            {
                container.CurrentTime += GameManager.DeltaTime;
                if (!(container.CurrentTime > container.Interval)) continue;

                container.CurrentTime = 0;
                SpawnEnemy(container.Enemy);

                if (--container.Amount <= 0) removeRange.Add(container);
            }

            foreach (var container in removeRange)
            {
                _SpawnContainers.Remove(container);
            }

            Spawning = _SpawnContainers.Count > 0;
        }

        public void LoadEnemyLevelInfo() =>
            _LevelInfo = _LevelInfo ? _LevelInfo : Resources.Load<EnemyLevelInfoScriptableObject>("Helper/Level/LevelInfo");
        
        private void ForceLoadEnemyLevelInfo() => 
            _LevelInfo = Resources.Load<EnemyLevelInfoScriptableObject>("Helper/Level");
        
        public void StartEnemySpawn(in uint level, out string error)
        {
            error = "";
            EnemyLevelInfo levelInfo;

            if (level < _LevelInfo.Levels.Length)
            {
                levelInfo = _LevelInfo.Levels[level];
            }
            else
            {
                error = "Level exceeds the level count. Will select max level.";
                levelInfo = _LevelInfo.Levels[^1];
            }

            _SpawnContainers = new List<SpawnContainer>();

            uint enemyAmount = 0;
            
            foreach (var info in levelInfo.Infos)
            {
                enemyAmount += info.Amount;
                _SpawnContainers.Add(new SpawnContainer(info.EnemyInfo, info.Amount, info.SpawnInterval));
            }

            EnemyAmount = enemyAmount;
            Spawning = true;
        }

        private void SpawnEnemy(in Character.Enemy.EnemyInfoScriptableObject enemy)
        {
            var enemyObject = Instantiate(enemy.EnemyGameObject);
            var enemyInfo = new EnemyClass(enemy.EnemyInfo, enemy.EnemyInfo.IsFlying);
            enemyObject.SpawnEnemy(enemyInfo);
            enemyObject.SetPath(_EnemyPaths[Random.Range(0, _EnemyPaths.Count)]);

            enemyObject.OnFinishReached += GameManager.Instance.HitEnemy;
            enemyInfo.OnDeath += EnemyDeath;
            enemyObject.OnFinishReached += EnemyFinish;

            if (ActiveEnemies.TryGetValue(enemyInfo.ID, out var enemies))
            {
                enemies.Add(enemyObject);
            }
            else
            {
                ActiveEnemies.Add(enemyInfo.ID, new List<EnemyGameObject>() { enemyObject });
            }
        }

        private void EnemyFinish(uint amount, EnemyClass enemy)
        {
            EnemyAmount--;
            ActiveEnemies.Remove(enemy.ID);
        }


        private void EnemyDeath(uint goldReward, EnemyClass enemyClass)
        {
            EnemyAmount--;
            OnEnemyDeath?.Invoke(goldReward);
            ActiveEnemies.Remove(enemyClass.ID);
        }

        public void SetPaths(in List<List<Vector2Int>> paths) => _EnemyPaths = paths;

        public EnemyLevelInfo GetLevelInfo(in uint level) =>
            level > _LevelInfo.Levels.Length ? _LevelInfo.Levels[^1] : _LevelInfo.Levels[level];
    }
}
