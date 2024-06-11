using System;

namespace Character.Enemy
{
    [Serializable]
    public class EnemyLevelInfo
    {
        [Serializable]
        public struct Container
        {
            public uint Amount;
            public float SpawnDelay;
            public float SpawnInterval;
            public EnemyInfoScriptableObject EnemyInfo;
        }

        public Container[] Infos;
    }
}
