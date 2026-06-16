using System.Linq;
using Board;
using Stack;

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
                .Where(s => s.Color == cell.CurrentStacks[^1].Color)
                .Sum(s => s.Count));
            
            if (total > mergeThreshold)
            {
                return null;
            }

            var placedStackData = group
                .SelectMany(x => x.CurrentStacks)
                .FirstOrDefault(s => s.Placed);

            var targetStackData = group
                .SelectMany(x => x.CurrentStacks)
                .FirstOrDefault(s => !s.Placed && s.Color == cell.CurrentStacks[^1].Color);

            return new MergeResult
            {
                Group = group,
                TotalCount = total,
                Color = cell.CurrentStacks[^1].Color,
                PlacedStackData = placedStackData,
                TargetStackData = targetStackData
            };
        }
    }
}