using Manager;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIScene : MonoBehaviour
{
    [field: Header("CreateWorld")]
    [SerializeField] private Canvas _MenuCanvas;
    [SerializeField] public Button _CreateWorld;
    
    private void Start()
    {
        _CreateWorld.onClick.AddListener(GameManager.Instance.LoadWorld);
    }
}
