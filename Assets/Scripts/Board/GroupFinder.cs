using System.Collections.Generic;

namespace Board
{
    public class GroupFinder
    {
        public List<HexCell> FindGroup(HexCell start)
        {
            var result = new List<HexCell>();

            if (start.IsEmpty)
            {
                return result;
            }

            var color = start.CurrentStacks[^1].Data.Color;
            var visited = new HashSet<HexCell>();
            var queue = new Queue<HexCell>();

            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();

                if (!visited.Add(cell))
                    continue;

                if (cell.IsEmpty)
                    continue;

                if (cell.CurrentStacks[^1].Data.Color != color)
                    continue;

                result.Add(cell);

                foreach (var n in cell.Neighbors)
                    queue.Enqueue(n);
            }

            return result;
        }
    }
}