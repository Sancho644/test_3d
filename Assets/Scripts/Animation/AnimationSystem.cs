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
        [SerializeField] private float staggerDelay = 0.05f;
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
            var halfDuration = baseDuration / speedMultiplier / 2f;

            var masterSeq = DOTween.Sequence();

            for (int i = 0; i < mergeCount; i++)
            {
                var hex = placedView.HexList[startIndex - i];
                if (hex == null)
                    continue;

                var startPos = hex.transform.position;
                var targetPos = baseTargetPos + Vector3.up * (hexHeight * i);
                var peakPos = Vector3.Lerp(startPos, targetPos, 0.5f) + Vector3.up * peakHeight;

                var direction = (targetPos - startPos).normalized;
                var flipAxis = Vector3.Cross(Vector3.up, direction).normalized;
                var targetRotation = Quaternion.AngleAxis(-180f, flipAxis);

                var totalDuration = halfDuration * 2f;

                var hexSeq = DOTween.Sequence();
                hexSeq.Append(hex.transform.DOMove(peakPos, halfDuration).SetEase(Ease.OutQuad));
                hexSeq.Append(hex.transform.DOMove(targetPos, halfDuration).SetEase(Ease.InQuad));
                hexSeq.Insert(0, hex.transform.DORotateQuaternion(targetRotation, totalDuration).SetEase(Ease.Linear));

                masterSeq.Insert(i * staggerDelay, hexSeq);
            }

            yield return masterSeq.WaitForCompletion();
        }
    }
}
