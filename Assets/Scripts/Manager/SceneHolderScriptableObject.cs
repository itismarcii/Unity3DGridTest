using UnityEditor;
using UnityEngine;

namespace Manager
{    
    [CreateAssetMenu(menuName = "GridTest3D/Infos/SceneTransition", fileName = "new Scene Info")]
    public class SceneHolderScriptableObject : ScriptableObject
    {
#if UNITY_EDITOR
        [field: SerializeField] public SceneAsset MainScene { get; private set; }
        [field: SerializeField] public SceneAsset WorldScene { get; private set; }
#endif
    }
}
