using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animation;
using Board;
using Config;
using Core;
using DG.Tweening;
using Stack;
using UnityEngine;

namespace Merge
{
    public class ChainReactionSystem : MonoBehaviour
    {
        public bool IsResolving { get; private set; }

        [SerializeField] private GameConfig gameConfig;

        private MergeSystem _mergeSystem;

        private float _speedMultiplier = 1f;
        private readonly HashSet<HexCell> _modifiedCells = new();

        private void Awake()
        {
            var groupFinder = new GroupFinder();
            _mergeSystem = new MergeSystem(groupFinder, gameConfig.DestroyThreshold);
        }

        public void ResetSpeed()
        {
            _speedMultiplier = 1f;
            _modifiedCells.Clear();
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

                    MergeResult merge = _mergeSystem.Check(cell, _modifiedCells);

                    if (merge == null)
                        continue;

                    foundAny = true;

                    yield return PlayMerge(merge, animationSystem);

                    ExecuteMerge(merge, board);
                    IncreaseSpeed();

                    break;
                }

                if (!foundAny)
                {
                    var toDestroy = new List<(StackData data, HexCell cell)>();

                    foreach (var cell in board.Cells)
                    {
                        if (cell == null || cell.IsEmpty)
                            continue;

                        var stackData = cell.CurrentStacks
                            .FirstOrDefault(s => s.Count >= gameConfig.DestroyThreshold);

                        if (stackData?.View != null)
                            toDestroy.Add((stackData, cell));
                    }

                    if (toDestroy.Count > 0)
                    {
                        foundAny = true;

                        var masterSeq = DOTween.Sequence();

                        foreach (var item in toDestroy)
                        {
                            var destroyDuration = gameConfig.PerHexDestroyDuration / _speedMultiplier;
                            var destroySeq = item.data.View.CreateDestroySequence(destroyDuration, SoundSystem.Instance);
                            masterSeq.Join(destroySeq);
                        }

                        yield return masterSeq.WaitForCompletion();

                        foreach (var item in toDestroy)
                        {
                            item.data.Count = 0;
                            item.data.Placed = false;
                            item.cell.CurrentStacks.Remove(item.data);
                            _modifiedCells.Add(item.cell);
                        }
                    }
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

            foreach (var cell in merge.Group)
            {
                _modifiedCells.Add(cell);
            }

            if (placedStackData.Count <= 0)
            {
                placedStackData.Count = 0;
                placedStackData.Placed = false;

                foreach (var cell in merge.Group)
                {
                    cell.CurrentStacks.Remove(placedStackData);
                }

                var view = placedStackData.View;
                if (view != null)
                {
                    var hasRemaining = false;
                    foreach (var cell in board.Cells)
                    {
                        if (cell.CurrentStacks.Any(s => s.View == view && s.Count > 0))
                        {
                            hasRemaining = true;
                            break;
                        }
                    }

                    if (!hasRemaining)
                    {
                        view.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void IncreaseSpeed()
        {
            _speedMultiplier *= gameConfig.SpeedMultiplier;
        }
    }
}