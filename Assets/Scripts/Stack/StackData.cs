using System;
using Config;

namespace Stack
{
    [Serializable]
    public class StackData
    {
        public ColorType Color;
        public int Count;
        public bool Placed;
        [NonSerialized] public StackView View;
    }
}