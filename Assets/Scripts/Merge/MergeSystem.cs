using System.Linq;
using Board;

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

            var placedStackData = group
                .SelectMany(x => x.CurrentStacks)
                .FirstOrDefault(s => s.Placed && s.Color == mergeColor);

            var targetStackData = group
                .SelectMany(x => x.CurrentStacks)
                .FirstOrDefault(s => !s.Placed && s.Color == mergeColor);

            if (targetStackData == null || targetStackData.Count >= destroyThreshold)
                return null;

            if (placedStackData == null || placedStackData.Count <= 0)
                return null;

            return new MergeResult
            {
                Group = group,
                TotalCount = 0,
                Color = mergeColor,
                PlacedStackData = placedStackData,
                TargetStackData = targetStackData
            };
        }
    }
}