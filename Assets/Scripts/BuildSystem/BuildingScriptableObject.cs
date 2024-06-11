using GridSystem;
using UnityEngine;

namespace BuildSystem
{
    [CreateAssetMenu(menuName = "GridTest3D/Building/Building", fileName = "new Building Info")]
    public class BuildingScriptableObject : ScriptableObject
    {
        [field: SerializeField] public BuildingTile Info { get; private set; }
        [field: SerializeField] public BuildingTileGameObject Prefab { get; private set; }
        [field: SerializeField] public uint GoldCost { get; private set; }
    }
}
