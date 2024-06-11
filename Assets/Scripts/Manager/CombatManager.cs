using System;
using Character.Enemy;
using Combat;
using UnityEngine;

namespace Manager
{
    public class CombatManager : MonoBehaviour
    {
        private void Awake()
        {
            GameManager.Instance.OnWorldSettingChange += ToggleManager;
        }
        
        private void ToggleManager(GameManager.WorldSetting stage)
        {
            if(!this) return;
            enabled = stage == GameManager.WorldSetting.Play;
        }

        private void Update()
        {
            foreach (var turret in BuildManager.PlacedBuildingTurrets)
            {
                turret.LastShotTime += GameManager.DeltaTime;
    
                if (turret.LastShotTime < turret.AttackSpeed) continue;

                if (!turret.LockedEnemy)
                {
                    var foundEnemy = false;

                    if (turret.CanAttackFly)
                    {
                        if (GetNewFlyEnemy(turret.transform.position, turret.Range, out var flyEnemy))
                        {
                            turret.LockedEnemy = flyEnemy;
                            foundEnemy = true;
                        }
                    }

                    if (!foundEnemy && GetNewEnemy(turret.transform.position, turret.Range, out var groundEnemy))
                    {
                        turret.LockedEnemy = groundEnemy;
                    }

                    if (!turret.LockedEnemy) continue;
                }
    
                turret.LastShotTime = 0;
                ShotTarget(turret, turret.LockedEnemy);
            }
        }

        private void ShotTarget(in TurretGameObject turret, in EnemyGameObject enemy) => enemy.Hit(turret.Damage);

        private bool GetNewFlyEnemy(in Vector3 pos, in float maxRange, out EnemyGameObject newEnemy)
        {
            newEnemy = null;
            foreach (var enemies in EnemyManager.ActiveEnemies.Values)
            {
                if(!enemies[0].EnemyInfo.IsFlying) continue;
                var distance = 100000f;
                
                foreach (var enemy in enemies)
                {
                    var enemyDistance = Vector3.Distance(pos, enemy.transform.position);
                    if(enemyDistance > distance || enemyDistance > maxRange) continue;
                    distance = enemyDistance;
                    newEnemy = enemy;
                }
            }
            
            return newEnemy;
        }
        
        private bool GetNewEnemy(in Vector3 pos, in float maxRange, out EnemyGameObject newEnemy)
        {
            newEnemy = null;
            
            foreach (var enemies in EnemyManager.ActiveEnemies.Values)
            {
                if(enemies[0].EnemyInfo.IsFlying) continue;
                var distance = 100000f;
                
                foreach (var enemy in enemies)
                {
                    var enemyDistance = Vector3.Distance(pos, enemy.transform.position);
                    if(enemyDistance > distance || enemyDistance > maxRange) continue;
                    distance = enemyDistance;
                    newEnemy = enemy;
                }
            }

            return newEnemy;
        }
    }
}
