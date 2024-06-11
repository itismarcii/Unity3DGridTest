using TMPro.Examples;
using UnityEngine;

namespace Character
{
    public abstract class CharacterGameObject : MonoBehaviour
    {
        public abstract void Spawn(CharacterClass characterClass);
        public abstract CharacterClass GetClassInfo();
    }
}
