using UnityEngine;

namespace Character
{
    public abstract class CharacterInfoScriptableObject : ScriptableObject
    {
        public virtual CharacterClass CharacterInfo { get; private set; }
    }
}
