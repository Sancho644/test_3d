using System.Collections.Generic;
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
        [SerializeField] private HexView hexPrefab;

        public StackData Data { get; private set; }
        public List<HexView> HexList { get; private set; } = new();

        private ColorDatabase _colorDatabase;

        public void Initialize(StackData data, GameController gameController, ColorDatabase colorDatabase)
        {
            Data = data;
            _colorDatabase = colorDatabase;

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

        public void SpawnHex(int hexCount = 0)
        {
            var count = hexCount != 0 ? hexCount : Data.Count;
            var color = _colorDatabase.GetColor(Data.Color);
            var hexPosition = transform.position;
            if (HexList.Count > 0)
            {
                var lastHexPosition = HexList[^1].transform.position;
                hexPosition = new Vector3(lastHexPosition.x, lastHexPosition.y + hexPrefab.Height, lastHexPosition.z);
            }

            for (int i = 0; i < count; i++)
            {
                HexView hexView = Instantiate(
                    hexPrefab,
                    hexPosition,
                    Quaternion.identity,
                    transform);

                hexView.SetColor(color);
                hexPosition += new Vector3(0f, hexView.Height, 0f);
                HexList.Add(hexView);
            }
        }

        public void SetPlaced(bool value)
        {
            Data.Placed = value;
        }

        public void SetCanDrag(bool value)
        {
            dragDropSystem.SetCanDrag(value);
        }

        public void Clear()
        {
            Data.Placed = false;
            Data.Count = 0;
        }
    }
}