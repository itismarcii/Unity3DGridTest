using System;
using UnityEditor;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RangeIdentifierGameObject : MonoBehaviour
    {
        private MeshFilter _MeshFilter;
        private MeshRenderer _MeshRenderer;
        [SerializeField] private Mesh _Mesh; 
        [SerializeField] private Material _Material; 
        private float _Range;

        public void SetRange(in float range)
        {
            _Range = range;
            transform.localScale = new Vector3(_Range, transform.localScale.y, _Range);
        }

        public void Select() => _MeshRenderer.enabled = true;
        public void Deselect() => _MeshRenderer.enabled = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _MeshFilter ??= GetComponent<MeshFilter>();
            _MeshRenderer ??= GetComponent<MeshRenderer>();

            _MeshFilter.sharedMesh ??= _Mesh;
            _MeshRenderer.sharedMaterial ??= _Material;
        }
#endif

        private void Awake()
        {
            _MeshFilter ??= GetComponent<MeshFilter>();
            _MeshRenderer ??= GetComponent<MeshRenderer>();
        
            _MeshFilter.mesh = _Mesh;
            _MeshRenderer.material = _Material;
        }
    }
}
