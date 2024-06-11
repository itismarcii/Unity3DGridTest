using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Enemy
{
    [Serializable]
    public class EnemyClass : CharacterClass
    {
        public static List<EnemyClass> Enemies = new();

        [field: SerializeField] public uint DifficultClass { get; private set; }
        [SerializeField] private uint _GoldReward;
        [field: SerializeField] public uint Damage { get; private set; }
        [field: SerializeField] public bool IsFlying { get; private set; }
        public Action<uint, EnemyClass> OnDeath;
        
        public override void Death()
        {
            base.Death();
            Enemies.Remove(this);
            OnDeath?.Invoke(_GoldReward,this);
            OnDeath = null;
        }
        
        public EnemyClass(in EnemyClass characterClass, in bool isFlying = false) : base(characterClass)
        {
            _GoldReward = characterClass._GoldReward;
            Damage = characterClass.Damage;
            Enemies.Add(this);
            IsFlying = isFlying;
        }
    }
}
