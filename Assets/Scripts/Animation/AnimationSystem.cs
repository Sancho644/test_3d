using System.Collections;
using System.Collections.Generic;
using Board;
using Merge;
using UnityEngine;

namespace Animation
{
    public class AnimationSystem : MonoBehaviour
    {
        public float BaseDuration = 0.5f;

        public IEnumerator PlayMerge(MergeResult result, float speedMultiplier)
        {
            float duration = BaseDuration / speedMultiplier;

            Vector3 center = CalculateCenter(result.Group);

            foreach (var cell in result.Group)
            {
                //cell.CurrentStack.transform.DOMove(center, duration);
            }

            yield return new WaitForSeconds(duration);
        }

        private Vector3 CalculateCenter(List<HexCell> group)
        {
            if (group == null || group.Count == 0)
                return Vector3.zero;

            Vector3 center = Vector3.zero;

            foreach (var cell in group)
            {
                center += cell.transform.position;
            }

            return center / group.Count;
        }
    }
}