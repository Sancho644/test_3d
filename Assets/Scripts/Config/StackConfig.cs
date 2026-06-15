using System;
using Config;

namespace Board
{
    [Serializable]
    public class StackConfig
    {
        public ColorType Color;
        public int Count;
    }
}