using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Tab
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        
        [Serializable]
        public struct Graphics
        {
            public float Size;
            public Image Image;
            public TMP_Text Text;
        }
        
        private int _IndexGroup;

        [SerializeField] internal TabGroup _TabGroup;
        [SerializeField] private Graphics Graphic;
        [SerializeField] private Color ActiveColor;
        [SerializeField] private Color HoverColor;
        [SerializeField] private Color InactiveColor;

        public int IndexGroup => _IndexGroup;

        public UnityEvent OnTabSelect, OnTabDeselect;

        public void OnPointerEnter(PointerEventData eventData) => _TabGroup.OnTabEnter(this);
        
        public void OnPointerClick(PointerEventData eventData) => _TabGroup.OnTabSelected(this);
        
        public void OnPointerExit(PointerEventData eventData) => _TabGroup.OnTabExit(this);
        public void SetIndexGroup(in int index) => _IndexGroup = index;

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (Graphic.Image) Graphic.Image.color = InactiveColor;
        }

#endif
        
        private void Awake()
        {
            Graphic.Image.color = InactiveColor;
            _TabGroup.Subscribe(this);
        }

        public void Select()
        {
            TabSelectImageChange();
            OnTabSelect?.Invoke();
        } 
        
        private void TabSelectImageChange()
        {
            Graphic.Image.color = ActiveColor;
        }

        public void TabDeselectImageChange()
        {
            Graphic.Image.color = InactiveColor;

        }

        public void TabHoverImageChange()
        {
            Graphic.Image.color = HoverColor;
        }
    }
}
