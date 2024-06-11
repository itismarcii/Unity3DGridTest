using System;
using Character.Enemy;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(RangeIdentifierGameObject))]
    public class TurretGameObject : MonoBehaviour
    {
        private RangeIdentifierGameObject _RangeIdentifier;
        
        [field: SerializeField] public uint Damage { get; private set; }
        [field: SerializeField] public float Range { get; private set; }
        [field: SerializeField] public float AttackSpeed { get; private set; }
        [field: SerializeField] public bool CanAttackFly { get; private set; }

        [SerializeField] private MeshRenderer _RangeIdenticatorRenderer;
        [HideInInspector] public EnemyGameObject LockedEnemy;
        [HideInInspector] public float LastShotTime = 0;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _RangeIdentifier ??= GetComponent<RangeIdentifierGameObject>();
            _RangeIdenticatorRenderer ??= GetComponent<MeshRenderer>();
            _RangeIdentifier.SetRange(Range);
        }
#endif

        private void Awake()
        {
            _RangeIdentifier ??= GetComponent<RangeIdentifierGameObject>();
            _RangeIdentifier.SetRange(Range);
        }

        public void Select() => _RangeIdentifier.Select();
        public void Deselect() => _RangeIdentifier.Deselect();

        public void ToggleRangeIdenticator(in bool toggle)
        {
            if(!_RangeIdenticatorRenderer) return;
            _RangeIdenticatorRenderer.enabled = toggle;
        }
    }
}
