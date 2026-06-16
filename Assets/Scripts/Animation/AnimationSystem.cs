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
            var filledHex = result.Group.Find(x => x.CurrentStacks.Find(y => y.Data.Placed));
            if (filledHex == null)
            {
                yield break;
            }
            
            var placedStack = filledHex.CurrentStacks.Find(x => x.Data.Placed);
            var mergeHex = result.Group.Find(x => x.CurrentStacks.Find(y => !y.Data.Placed));
            var center = mergeHex.CurrentStacks[^1].HexList[^1].gameObject.transform.position;
            var hexHeight = mergeHex.CurrentStacks[^1].HexList[0].Height;
            
            center = new Vector3(center.x, center.y + hexHeight, center.z);
            
            foreach (var hex in placedStack.HexList)
            {
                hex.transform.DOMove(center, duration);
            }

            yield return new WaitForSeconds(duration);
        }
    }
}