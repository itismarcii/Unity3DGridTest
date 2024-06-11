using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerClass
    {
        [field: SerializeField] public string PlayerName { get; private set; } = "Player";
        [SerializeField] private uint _PlayerMaxHealth = 10;
        private uint _PlayerHealth;
        [SerializeField] private uint _Gold = 10;

        public Action OnPlayerDeath;
        public Action<uint> OnHealthChange, OnGoldChange;

        public PlayerClass(in PlayerClass playerClass)
        {
            PlayerName = playerClass.PlayerName;
            _PlayerMaxHealth = playerClass._PlayerMaxHealth;
            _PlayerHealth = _PlayerMaxHealth;
            _Gold = playerClass.Gold;
        }
        
        public PlayerClass(string playerName, uint playerMaxHealth, uint gold)
        {
            PlayerName = playerName;
            _PlayerMaxHealth = playerMaxHealth;
            _PlayerHealth = playerMaxHealth;
            _Gold = gold;
        }

        public uint Health
        {
            get => _PlayerHealth;
            set
            {
                _PlayerHealth = value;
                OnHealthChange?.Invoke(_PlayerHealth);
                
                if (_PlayerHealth > 0) return;
                
                OnPlayerDeath?.Invoke();
                _PlayerHealth = 0;
            }
        }

        public uint Gold
        {
            get => _Gold;
            set
            {
                _Gold = value;
                OnGoldChange?.Invoke(_Gold);
            }
        }

        public void ResetPlayerHealth() => _PlayerHealth = _PlayerMaxHealth;
    }
}
