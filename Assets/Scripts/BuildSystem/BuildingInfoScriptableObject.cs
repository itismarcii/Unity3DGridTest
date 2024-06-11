using UnityEngine;

namespace BuildSystem
{
    [CreateAssetMenu(menuName = "GridTest3D/Infos/BuildingInfo", fileName = "new Building Info")]
    public class BuildingInfoScriptableObject : ScriptableObject
    {
        [field: Header("Cant place")]
        [field: SerializeField] public Material CantPlaceMaterial { get; private set; }

        [field: Header("Place Hover")]
        [field: SerializeField] public Material PlaceHoverMaterial { get; private set; }
    }
}
