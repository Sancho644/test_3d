using System.Collections.Generic;
using Stack;

namespace Board
{
    public class GroupFinder
    {
        public List<HexCell> FindGroup(HexCell start)
        {
            var result = new List<HexCell>();

            if (start.IsEmpty || start.CurrentStacks.Count == 0)
            {
                return result;
            }

            var color = start.CurrentStacks[^1].Color;
            var visited = new HashSet<HexCell>();
            var queue = new Queue<HexCell>();

            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                if (!visited.Add(cell))
                    continue;

                if (cell.IsEmpty || cell.CurrentStacks.Count == 0)
                    continue;

                if (cell.CurrentStacks[^1].Color != color)
                    continue;
                
                if (cell.CurrentStacks[^1].Count == 0)
                    continue;

                result.Add(cell);

                foreach (var n in cell.Neighbors)
                    queue.Enqueue(n);
            }

            return result;
        }
    }
}