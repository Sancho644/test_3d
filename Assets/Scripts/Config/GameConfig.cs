using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int MergeThreshold = 10;
        public float BaseMoveSpeed = 1f;
        public float SpeedIncrease = 0.3f;
    }
}