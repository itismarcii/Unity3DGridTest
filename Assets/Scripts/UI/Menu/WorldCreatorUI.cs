using System;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class WorldCreatorUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _XWorldDimension;
        [SerializeField] private TMP_InputField _YWorldDimension;
        [SerializeField] private Toggle _FlyingEnemyToggle;

        private void Awake()
        {
            MenuUIManager.WorldCreatorUI = this;
        }
        
        public int XDimension => int.TryParse(_XWorldDimension.text, out var x) ? x : 0;
        public int YDimension => int.TryParse(_YWorldDimension.text, out var y) ? y : 0;
        public bool FlyingEnemies => _FlyingEnemyToggle.enabled;
    }
}
