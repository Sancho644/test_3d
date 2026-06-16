using System.Linq;
using Board;

namespace Merge
{
    public class MergeSystem
    {
        private GroupFinder finder;
        private int mergeThreshold;

        public MergeSystem(GroupFinder finder, int mergeThreshold = 10)
        {
            this.finder = finder;
            this.mergeThreshold = mergeThreshold;
        }

        public MergeResult Check(HexCell cell)
        {
            if (cell.IsEmpty || cell.CurrentStacks.Count == 0)
                return null;

            var group = finder.FindGroup(cell);

            if (group.Count < 2)
                return null;

            var total = group.Sum(x => x.CurrentStacks
                .Where(s => s.Data.Color == cell.CurrentStacks[^1].Data.Color)
                .Sum(s => s.Data.Count));
            
            if (total > mergeThreshold)
            {
                return null;
            }

            return new MergeResult
            {
                Group = group,
                TotalCount = total,
                Color = cell.CurrentStacks[^1].Data.Color
            };
        }
    }
}