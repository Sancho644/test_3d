using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animation;
using Board;
using UnityEngine;

namespace Merge
{
    public class ChainReactionSystem : MonoBehaviour
    {
        public bool IsResolving { get; private set; }

        private MergeSystem _mergeSystem;
        
        private float _speedMultiplier = 1f;

        private void Awake()
        {
            var groupFinder = new GroupFinder();
            _mergeSystem = new MergeSystem(groupFinder);
        }
        
        public void ResetSpeed()
        {
            _speedMultiplier = 1f;
        }

        public IEnumerator Resolve(BoardController board, AnimationSystem animationSystem)
        {
            IsResolving = true;

            bool foundAny;

            do
            {
                foundAny = false;

                List<HexCell> allCells = board.OccupiedCells().ToList();

                foreach (var cell in allCells)
                {
                    if (cell == null || cell.IsEmpty)
                        continue;

                    MergeResult merge = _mergeSystem.Check(cell);

                    if (merge == null)
                        continue;

                    foundAny = true;

                    yield return PlayMerge(
                        merge,
                        animationSystem);

                    ExecuteMerge(merge, board);

                    IncreaseSpeed();

                    break; // важно: по одному merge за шаг
                }
            } while (foundAny);

            IsResolving = false;
        }

        private IEnumerator PlayMerge(MergeResult merge, AnimationSystem animationSystem)
        {
            yield return animationSystem.PlayMerge(merge, _speedMultiplier);
        }

        private void ExecuteMerge(MergeResult merge, BoardController board)
        {
            /*var target = merge.Group.OrderByDescending(x => x.CurrentStack.Data.Count).First();
            var total = merge.TotalCount;

            target.CurrentStack.Data.Count = total;

            foreach (var cell in merge.Group)
            {
                if (cell == target)
                    continue;

                cell.CurrentStack =
                    null;

                Destroy(cell.CurrentStack?.gameObject);
            }*/
        }

        private void IncreaseSpeed()
        {
            _speedMultiplier *= 1.3f;
        }
    }
}