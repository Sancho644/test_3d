using System.Collections;
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
            var filledHex = merge.Group.Find(x => x.CurrentStacks.Find(y => y.Data.Placed));
            if (filledHex == null)
            {
                return;
            }
            
            var placedStack = filledHex.CurrentStacks.Find(x => x.Data.Placed);
            var mergeHex = merge.Group.Find(x => x.CurrentStacks.Find(y => !y.Data.Placed));
            var mergeStack = mergeHex.CurrentStacks.Find(x => !x.Data.Placed);

            mergeHex.AddStacks(placedStack);
            mergeStack.SpawnHex(placedStack.Data.Count);
            placedStack.Clear();
            placedStack.gameObject.SetActive(false);
        }

        private void IncreaseSpeed()
        {
            _speedMultiplier *= 1.3f;
        }
    }
}