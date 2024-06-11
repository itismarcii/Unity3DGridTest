using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Tab
{
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> _TabButtons = new List<TabButton>();
        [SerializeField] private TabButton _StartSelect;
        [SerializeField] private GameObject[] _TabGameObjects;
        private Sprite _Idle;
        private Sprite _Hover;
        private Sprite _Active;
        private TabButton _ActiveTab;

        public Action OnTabChange;
        
        public void Subscribe(in TabButton tap) => _TabButtons.Add(tap);
        public void Unsubscribe(in TabButton tap) => _TabButtons.Remove(tap);
        
        private void Awake()
        {
            var tabs = GetComponentsInChildren<TabButton>();
            
            for (var i = 0; i < tabs.Length; i++)
            {
                tabs[i].SetIndexGroup(i);
            }
        }

        private void Start()
        {
            if(_TabButtons.Contains(_StartSelect))
            {
                OnTabSelected(_StartSelect);
            }
            else
            {
                Debug.LogWarning($"{gameObject} : Start select button doesnt belong to this tab group.");
            }
        }

        public void OnTabEnter(in TabButton tap)
        {
            if(tap == _ActiveTab) return;

            ResetTabs();
            tap.TabHoverImageChange();
        }

        public void OnTabExit(in TabButton tap)
        {
            if(tap == _ActiveTab) return;
            ResetTabs();
        }

        public void OnTabSelected(in TabButton tap)
        {
            _ActiveTab?.OnTabDeselect?.Invoke();
            _ActiveTab = tap;
            _ActiveTab.Select();
            
            OnTabChange?.Invoke();
            
            ResetTabs(); 

            var index = tap.IndexGroup;
            
            for (var i = 0; i < _TabGameObjects.Length; i++)
            {
                _TabGameObjects[i].SetActive(i == index);
            }
        }
        
        public void ResetTabs()
        {
            foreach (var tap in _TabButtons)
            {
                if(tap == _ActiveTab) continue;
                tap.TabDeselectImageChange();
            }
        }

    }
}
