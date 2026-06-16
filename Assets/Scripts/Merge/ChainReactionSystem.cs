using System.Collections;
using System.Linq;
using Animation;
using Board;
using Stack;
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

                foreach (var cell in board.Cells)
                {
                    if (cell == null || cell.IsEmpty)
                        continue;

                    MergeResult merge = _mergeSystem.Check(cell);

                    if (merge == null)
                        continue;
                    
                    foundAny = true;

                    yield return PlayMerge(merge, animationSystem);

                    ExecuteMerge(merge, board);
                    IncreaseSpeed();

                    break; 
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
            var placedStackData = merge.PlacedStackData;
            var targetStackData = merge.TargetStackData;

            if (placedStackData == null || targetStackData == null)
                return;

            targetStackData.Count += placedStackData.Count;

            if (targetStackData.View != null)
            {
                targetStackData.View.SpawnHex(placedStackData.Count);
            }

            placedStackData.Count = 0;
            placedStackData.Placed = false;

            if (placedStackData.View != null)
            {
                placedStackData.View.gameObject.SetActive(false);
            }

            foreach (var cell in merge.Group)
            {
                cell.CurrentStacks.Remove(placedStackData);
            }
        }

        private void IncreaseSpeed()
        {
            _speedMultiplier *= 1.3f;
        }
    }
}