using Config;
using UnityEngine;

namespace Board
{
    [CreateAssetMenu(menuName = "Configs/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public CellConfig[] Cells;

        public StackConfig[] AvailableStacks;
    }
}