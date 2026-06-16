using System;
using Config;
using Stack;
using UnityEngine;

namespace Board
{
    [Serializable]
    public class StackSpawnSettings
    {
        public ColorType ColorType;
        [Range(0, 9)] public int Count = 1;
    }
}