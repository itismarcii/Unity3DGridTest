using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace BuildSystem
{
    [RequireComponent(typeof(Button))]
    public class BuildingButton : MonoBehaviour
    {
        private int IndexID;
        [field: SerializeField] public Button Button { get; private set; }
    
#if UNITY_EDITOR
        private void OnValidate()
        {
            Button ??= GetComponent<Button>();
        }
#endif

        public BuildingButton CreateButton(in Transform parent, in int index)
        {
            var button = Instantiate(this, parent);
            button.Button.onClick.AddListener(button.Select);
            button.IndexID = index;
            return button;
        }

        private void Select()
        {
            GameManager.Instance.SelectBuildingInfo(IndexID);
        }
    }
}
