using UnityEngine;

namespace GridSystem
{
    [CreateAssetMenu(menuName = "GridTest3D/Infos/GridTile", fileName = "new GridTile Info")]
    public class GridTileInfosScriptableObject : ScriptableObject
    {
        [field: Header("DefaultTile")]
        [field: SerializeField] public Material DefaultGridMaterial { get; private set; }
        
        [field: Header("PathTile")]
        [field: SerializeField] public Material PathGridMaterial { get; private set; }
        
        [field: Header("BuildingTile")]
        [field: SerializeField] public Material BuildingGridMaterial { get; private set; }
        
        [field: Header("SelectionTile")]
        [field: SerializeField] public Material SelectionGridMaterial { get; private set; }
        
        [field: Header("SpawnTile")]
        [field: SerializeField] public Material SpawnPathGridMaterial { get; private set; }
        
        [field: Header("FinishTile")]
        [field: SerializeField] public Material FinishPathGridMaterial { get; private set; }
    }
}
