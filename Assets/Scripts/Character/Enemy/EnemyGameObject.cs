using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Character.Enemy
{
    public class EnemyGameObject : CharacterGameObject
    {
        private const float MIN_DISTANCE_NEXT_TILE = 0.1f;
        
        [field: SerializeField] public EnemyClass EnemyInfo { get; private set; }
        private List<Vector2Int> Path;
        private int _CurrentTileCount = 0;
        private Vector3 _WorldPosition;

        public Action<uint, EnemyClass> OnFinishReached;
        
        public override void Spawn(CharacterClass characterClass)
        { throw new NotImplementedException(); }

        public override CharacterClass GetClassInfo() => EnemyInfo;
        
        public void SpawnEnemy(EnemyClass enemyClass) => EnemyInfo = enemyClass;

        public void Hit(in uint hit)
        {
            EnemyInfo.Hit(hit);
            if (EnemyInfo.HealthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void SetPath(in List<Vector2Int> path)
        {
            Path = path;
            GetWorldPosition();
            transform.position = _WorldPosition;
            
            if (!EnemyInfo.IsFlying) return;
            _WorldPosition = GridManager.GridField[Path[^1]].WorldPositionXZ;
            _CurrentTileCount = Path.Count;
        }
        
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _WorldPosition, EnemyInfo.MovementSpeed * GameManager.DeltaTime);

            if (!(Vector3.Distance(transform.position, _WorldPosition) < MIN_DISTANCE_NEXT_TILE)) return;
            
            _CurrentTileCount++;
            if (Path.Count <= _CurrentTileCount)
            {
                OnFinishReached?.Invoke(EnemyInfo.Damage, EnemyInfo);
                Destroy(gameObject);
                return;
            }
            
            GetWorldPosition();
        }

        private void GetWorldPosition()
        {
            var worldXZPos = GridManager.GridField[Path[_CurrentTileCount]].WorldPositionXZ;
            _WorldPosition = new Vector3(worldXZPos.x, 0, worldXZPos.y);
        }
    }
}
