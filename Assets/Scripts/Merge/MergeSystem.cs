using System.Linq;
using Board;
using Stack;

namespace Merge
{
    public class MergeSystem
    {
        private GroupFinder finder;
        private int destroyThreshold;

        public MergeSystem(GroupFinder finder, int destroyThreshold = 10)
        {
            this.finder = finder;
            this.destroyThreshold = destroyThreshold;
        }

        public MergeResult Check(HexCell cell)
        {
            if (cell.IsEmpty || cell.CurrentStacks.Count == 0)
                return null;

            var group = finder.FindGroup(cell);

            if (group.Count < 2)
                return null;

            var mergeColor = cell.CurrentStacks[^1].Color;
            var allSameColor = group
                .SelectMany(x => x.CurrentStacks)
                .Where(s => s.Color == mergeColor && s.Count > 0)
                .ToList();

            if (allSameColor.Count < 2)
                return null;

            var placed = allSameColor.FirstOrDefault(s => s.Placed);
            var nonPlaced = allSameColor.Where(s => !s.Placed).ToList();

            StackData source;
            StackData target;

            if (placed != null && nonPlaced.Count > 0)
            {
                source = placed;
                target = nonPlaced.First();
            }
            else if (nonPlaced.Count >= 2)
            {
                nonPlaced.Sort((a, b) => a.Count.CompareTo(b.Count));
                source = nonPlaced[0];
                target = nonPlaced[1];
            }
            else
            {
                return null;
            }

            if (target.Count >= destroyThreshold)
                return null;

            if (source.Count <= 0)
                return null;

            return new MergeResult
            {
                Group = group,
                TotalCount = 0,
                Color = mergeColor,
                PlacedStackData = source,
                TargetStackData = target
            };
        }
    }
}