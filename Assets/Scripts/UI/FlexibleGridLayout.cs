using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            UNIFORM,
            WIDTH,
            HEIGHT,
            FIXED_ROWS,
            FIXED_COLUMNS
        }
        
        [SerializeField] private int _Rows;
        [SerializeField] private int _Columns;
        [SerializeField, HideInInspector] private Vector2 _CellSize;
        [SerializeField] private FitType _FitType;
        [SerializeField] private Vector2 _Spacing;

        private bool _FitX = false, _FitY = false;
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            _Rows = _Rows <= 0 ? 1 : _Rows; 
            _Columns = _Columns <= 0 ? 1 : _Columns; 
            
            if (_FitType is FitType.UNIFORM or FitType.WIDTH or FitType.HEIGHT)
            {
                _FitX = true;
                _FitY = true;
                var sqrRt = MathF.Sqrt(transform.childCount);
                _Rows = Mathf.CeilToInt(sqrRt);
                _Columns = Mathf.CeilToInt(sqrRt);
            }

            switch (_FitType)
            {
                case FitType.WIDTH or FitType.FIXED_COLUMNS:
                    _Rows = Mathf.CeilToInt(transform.childCount / (float) _Columns);
                    break;
                case FitType.HEIGHT or FitType.FIXED_ROWS:
                    _Columns = Mathf.CeilToInt(transform.childCount / (float) _Rows);
                    break;
            }

            var rect = rectTransform.rect;

            var cellWidth = (rect.width / _Columns) - ((_Spacing.x / _Columns) * 2) -
                            (padding.left / (float) _Columns) - (padding.right / (float) _Columns);
            var cellHeight = (rect.height / _Rows) - ((_Spacing.y / _Rows) * 2) - (padding.top / (float) _Rows) -
                             (padding.bottom / (float) _Columns);  
            
            _CellSize.x = _FitX ? cellWidth : _CellSize.x;
            _CellSize.y = _FitY ? cellHeight : _CellSize.y;
            
            for (var i = 0; i < rectChildren.Count; i++)
            {
                var rowCount = i / _Columns;
                var columnCount = i % _Columns;

                var item = rectChildren[i];
                var xPos = (_CellSize.x * columnCount) + (_Spacing.x * columnCount) + (padding.left);
                var yPos = (_CellSize.y * rowCount) + (_Spacing.y * rowCount) + (padding.top);
                
                SetChildAlongAxis(item, 0, xPos, _CellSize.x);
                SetChildAlongAxis(item, 1, yPos, _CellSize.y);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}
