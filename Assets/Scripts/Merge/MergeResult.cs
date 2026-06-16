using System.Collections.Generic;
using Board;
using Config;
using Stack;

namespace Merge
{
    public class MergeResult
    {
        public List<HexCell> Group;
        public int TotalCount;
        public ColorType Color;
        public StackData PlacedStackData;
        public StackData TargetStackData;
    }
}