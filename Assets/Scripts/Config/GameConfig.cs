using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int DestroyThreshold = 10;
        public float PerHexDestroyDuration = 0.1f;
    }
}