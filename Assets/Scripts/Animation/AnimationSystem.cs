using System.Collections;
using System;
using Config;
using DG.Tweening;
using Merge;
using UnityEngine;

namespace Animation
{
    public class AnimationSystem : MonoBehaviour
    {
        [SerializeField] private float baseDuration = 0.5f;
        [SerializeField] private float peakHeight = 2f;
        [SerializeField] private GameConfig gameConfig;

        public IEnumerator PlayMerge(MergeResult result, float speedMultiplier)
        {
            var placedStackData = result.PlacedStackData;
            var targetStackData = result.TargetStackData;

            if (placedStackData == null || placedStackData.View == null
                || targetStackData == null || targetStackData.View == null)
            {
                yield break;
            }

            var placedView = placedStackData.View;
            var targetView = targetStackData.View;

            if (targetView.HexList.Count == 0 || placedView.HexList.Count == 0)
                yield break;

            var total = targetStackData.Count + placedStackData.Count;
            var toMerge = total > gameConfig.DestroyThreshold
                ? gameConfig.DestroyThreshold - targetStackData.Count
                : placedStackData.Count;

            if (toMerge <= 0)
                yield break;

            var hexHeight = targetView.HexList[0].Height;
            var baseTargetPos = targetView.HexList[^1].transform.position + Vector3.up * hexHeight;

            var mergeCount = Math.Min(toMerge, placedView.HexList.Count);
            var startIndex = placedView.HexList.Count - 1;

            for (int i = 0; i < mergeCount; i++)
            {
                var hex = placedView.HexList[startIndex - i];
                if (hex == null)
                    continue;

                var startPos = hex.transform.position;
                var targetPos = baseTargetPos + Vector3.up * (hexHeight * i);
                var peakPos = Vector3.Lerp(startPos, targetPos, 0.5f) + Vector3.up * peakHeight;

                var halfDuration = baseDuration / speedMultiplier / 2f;

                var seq = DOTween.Sequence();
                seq.Append(hex.transform.DOMove(peakPos, halfDuration).SetEase(Ease.OutQuad));
                seq.Join(hex.transform.DORotate(new Vector3(0f, 0f, 180f), halfDuration * 2f).SetEase(Ease.InOutQuad));
                seq.Append(hex.transform.DOMove(targetPos, halfDuration).SetEase(Ease.InQuad));

                yield return seq.WaitForCompletion();
            }
        }
    }
}