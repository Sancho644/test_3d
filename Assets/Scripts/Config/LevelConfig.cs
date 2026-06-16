using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Configs/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public CellConfig[] Cells;

        public StackConfig[] AvailableStacks;
    }
}