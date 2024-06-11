using UnityEngine;

namespace Character.Enemy
{
    [CreateAssetMenu(menuName = "GridTest3D/Character/EnemyInfo", fileName = "new Enemy Info")]
    public class EnemyInfoScriptableObject : CharacterInfoScriptableObject
    {
        [field: SerializeField] public EnemyGameObject EnemyGameObject { get; private set; }
        public override CharacterClass CharacterInfo => EnemyGameObject.GetClassInfo();
        public EnemyClass EnemyInfo => EnemyGameObject.EnemyInfo;
    }
}
