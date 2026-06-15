using System.Linq;
using Board;

namespace Merge
{
    public class MergeSystem
    {
        private GroupFinder finder;

        public MergeSystem(GroupFinder finder)
        {
            this.finder = finder;
        }

        public MergeResult Check(HexCell cell)
        {
            var group = finder.FindGroup(cell);
            var total = group.Sum(x => x.CurrentStacks.Count);

            if (total > 10)
            {
                return null;
            }

            return new MergeResult
            {
                Group = group,
                TotalCount = total,
                Color = cell.CurrentStacks[0].Data.Color
            };
        }
    }
}