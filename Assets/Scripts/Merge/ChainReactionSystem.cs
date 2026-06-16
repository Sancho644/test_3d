using System.Collections;
using System.Linq;
using Animation;
using Board;
using Config;
using UnityEngine;

namespace Merge
{
    public class ChainReactionSystem : MonoBehaviour
    {
        public bool IsResolving { get; private set; }

        [SerializeField] private GameConfig gameConfig;

        private MergeSystem _mergeSystem;
        
        private float _speedMultiplier = 1f;

        private void Awake()
        {
            var groupFinder = new GroupFinder();
            _mergeSystem = new MergeSystem(groupFinder, gameConfig.DestroyThreshold);
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

                    ExecuteMerge(merge);
                    IncreaseSpeed();

                    break; 
                }

                if (!foundAny)
                {
                    foreach (var cell in board.Cells)
                    {
                        if (cell == null || cell.IsEmpty)
                            continue;

                        var stackData = cell.CurrentStacks
                            .FirstOrDefault(s => s.Count >= gameConfig.DestroyThreshold);

                        if (stackData?.View == null)
                            continue;

                        foundAny = true;

                        yield return stackData.View.PlayDestroyAnimation(gameConfig.PerHexDestroyDuration);

                        stackData.Count = 0;
                        stackData.Placed = false;
                        cell.CurrentStacks.Remove(stackData);

                        break;
                    }
                }
            } while (foundAny);

            IsResolving = false;
        }

        private IEnumerator PlayMerge(MergeResult merge, AnimationSystem animationSystem)
        {
            yield return animationSystem.PlayMerge(merge, _speedMultiplier);
        }

        private void ExecuteMerge(MergeResult merge)
        {
            var placedStackData = merge.PlacedStackData;
            var targetStackData = merge.TargetStackData;

            if (placedStackData == null || targetStackData == null)
                return;

            var threshold = gameConfig.DestroyThreshold;
            var total = targetStackData.Count + placedStackData.Count;
            var toMerge = total > threshold ? threshold - targetStackData.Count : placedStackData.Count;

            if (toMerge <= 0)
                return;

            targetStackData.Count += toMerge;

            if (targetStackData.View != null)
            {
                targetStackData.View.SpawnHex(toMerge);
            }

            placedStackData.Count -= toMerge;

            if (placedStackData.View != null)
            {
                placedStackData.View.RemoveTopHexes(toMerge);
            }

            if (placedStackData.Count <= 0)
            {
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
        }

        private void IncreaseSpeed()
        {
            _speedMultiplier *= 1.3f;
        }
    }
}