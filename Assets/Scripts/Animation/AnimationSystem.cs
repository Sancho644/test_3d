using System.Collections;
using DG.Tweening;
using Merge;
using UnityEngine;

namespace Animation
{
    public class AnimationSystem : MonoBehaviour
    {
        [SerializeField] private float baseDuration = 0.5f;

        public IEnumerator PlayMerge(MergeResult result, float speedMultiplier)
        {
            var duration = baseDuration / speedMultiplier;
            var placedStackData = result.PlacedStackData;
            var targetStackData = result.TargetStackData;

            if (placedStackData == null || placedStackData.View == null
                || targetStackData == null || targetStackData.View == null)
            {
                yield break;
            }

            var placedView = placedStackData.View;
            var targetView = targetStackData.View;

            if (targetView.HexList.Count == 0)
                yield break;

            var center = targetView.HexList[^1].gameObject.transform.position;
            var hexHeight = targetView.HexList[0].Height;
            
            center = new Vector3(center.x, center.y + hexHeight, center.z);
            
            foreach (var hex in placedView.HexList)
            {
                hex.transform.DOMove(center, duration);
            }

            yield return new WaitForSeconds(duration);
        }
    }
}