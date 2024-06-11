using System.Collections.Generic;
using GridSystem;
using UnityEngine;

namespace BuildSystem
{
    public class TileIdentifierGameObject : MonoBehaviour
    {
        private LayerMask _LayerMask;
        private const float MAX_RAY_CAST_DISTANCE = 100f;
        public TileGameObject TileGameObject { get; private set; }
        
        private void Awake()
        {
            _LayerMask = LayerMask.GetMask("GridTile");
        }

        public TileGameObject Scan()
        {
            var ray = new Ray(transform.position, -transform.up);

            return !Physics.Raycast(ray, out var hit, MAX_RAY_CAST_DISTANCE, _LayerMask) ? 
                null : GridBuilderRectangle.GridFieldDictionary.GetValueOrDefault(hit.transform);
        }

        public void CacheTile(in TileGameObject tile) => TileGameObject = tile;
    }
}
