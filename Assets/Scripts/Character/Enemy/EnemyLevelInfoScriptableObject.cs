using UnityEngine;

namespace Character.Enemy
{
    [CreateAssetMenu(menuName = "GridTest3D/Infos/LevelInfo", fileName = "new Level Info")]
    public class EnemyLevelInfoScriptableObject : ScriptableObject
    {
        [field: SerializeField] public EnemyLevelInfo[] Levels { get; private set; }
    }
}
