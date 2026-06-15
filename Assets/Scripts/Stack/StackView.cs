using Config;
using Core;
using Input;
using UnityEngine;

namespace Stack
{
    public class StackView : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private DragDropSystem dragDropSystem;
        
        public StackData Data { get; private set; }
        
        public void Initialize(StackData data, GameController gameController)
        {
            Data = data;
            dragDropSystem.Initialize(gameController);
        }
        
        public void Add(int value)
        {
            Data.Count += value;
        }
        
        public void SetCount(int value)
        {
            Data.Count = value;
        }
        
        public void PlaySpawn()
        {
            transform.localScale = Vector3.zero;

            /*transform
                .DOScale(1f, 0.25f)
                .SetEase(Ease.OutBack);*/
        }
        
        public void PlayMergeImpact()
        {
            /*transform
                .DOScale(1.2f, 0.1f)
                .SetLoops(2, LoopType.Yoyo);*/
        }
        
        public void PlayDestroy()
        {
            /*Sequence seq = DOTween.Sequence();

            seq.Append(
                transform.DOScale(1.3f, 0.1f));

            seq.Append(
                transform.DOScale(0f, 0.15f));

            seq.OnComplete(() =>
            {
                Destroy(gameObject);
            });*/
        }
    }
}