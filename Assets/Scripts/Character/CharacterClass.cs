using System;
using UnityEngine;

namespace Character
{
    [Serializable]
    public abstract class CharacterClass
    {
        [field: SerializeField] public string Name {get; private set; }
        public uint ID {get; private set; }
        [field: SerializeField] public int HealthPoints {get; private set; }
        [field: SerializeField] public float MovementSpeed {get; private set; }
        [field: SerializeField] public int AttackDamage {get; private set; }

        protected CharacterClass(string name, int healthPoints, float movementSpeed, int attackDamage)
        {
            Name = name;
            HealthPoints = healthPoints;
            MovementSpeed = movementSpeed;
            AttackDamage = attackDamage;
        }

        protected CharacterClass(in CharacterClass characterClass)
        {
            Name = characterClass.Name;
            HealthPoints = characterClass.HealthPoints;
            MovementSpeed = characterClass.MovementSpeed;
            AttackDamage = characterClass.AttackDamage;
        }

        public virtual void Hit(in uint damage)
        {
            HealthPoints -= (int)damage;
            if(HealthPoints <= 0) Death();
        }

        public virtual void Death() {}
    }
}
