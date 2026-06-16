using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Core;
using DG.Tweening;
using Input;
using UnityEngine;

namespace Stack
{
    public class StackView : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private DragDropSystem dragDropSystem;
        [SerializeField] private HexView hexPrefab;
        [SerializeField] private ParticleSystem hexFx;

        public List<StackData> SubStacks { get; private set; } = new();
        public List<HexView> HexList { get; private set; } = new();

        private ColorDatabase _colorDatabase;

        public StackData TopSubStack => SubStacks.Count > 0 ? SubStacks[^1] : null;

        public void Initialize(List<StackData> subStacks, GameController gameController, ColorDatabase colorDatabase)
        {
            SubStacks = subStacks;
            foreach (var sub in subStacks)
                sub.View = this;

            _colorDatabase = colorDatabase;
            dragDropSystem.Initialize(gameController);
        }

        public void Initialize(StackData data, GameController gameController, ColorDatabase colorDatabase)
        {
            Initialize(new List<StackData> { data }, gameController, colorDatabase);
        }

        public void PlaySpawn()
        {
            transform.localScale = Vector3.zero;
        }

        public void PlayMergeImpact()
        {
        }

        public Tween CreateDestroySequence(float perHexDuration, Action onComplete = null)
        {
            var seq = DOTween.Sequence();

            for (int i = HexList.Count - 1; i >= 0; i--)
            {
                if (HexList[i] != null)
                {
                    seq.Append(
                        HexList[i].transform
                            .DOScale(new Vector3(0f, 1f, 0f), perHexDuration)
                            .SetEase(Ease.InBack));
                }
            }

            seq.OnComplete(() =>
            {
                PlayDestroyEffect();
                onComplete?.Invoke();
                Destroy(gameObject);
            });

            return seq;
        }

        public void SpawnHex(int countToAdd = -1)
        {
            var hexPosition = transform.position;

            if (HexList.Count > 0)
            {
                var lastHexPosition = HexList[^1].transform.position;
                hexPosition = new Vector3(lastHexPosition.x, lastHexPosition.y + hexPrefab.Height, lastHexPosition.z);
            }

            foreach (var sub in SubStacks)
            {
                var color = _colorDatabase.GetColor(sub.Color);
                var count = countToAdd >= 0 ? countToAdd : sub.Count;

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
        }

        public void SetPlaced(bool value)
        {
            foreach (var sub in SubStacks)
                sub.Placed = value;
        }

        public void SetCanDrag(bool value)
        {
            dragDropSystem.SetCanDrag(value);
        }

        public void Clear()
        {
            foreach (var sub in SubStacks)
            {
                sub.Placed = false;
                sub.Count = 0;
            }
        }

        public void RemoveTopHexes(int count)
        {
            for (int i = 0; i < count && HexList.Count > 0; i++)
            {
                var hex = HexList[^1];
                HexList.RemoveAt(HexList.Count - 1);
                Destroy(hex.gameObject);
            }
        }

        public void PlayDestroyEffect()
        {
            if (hexFx == null) return;

            var topSub = TopSubStack;
            if (topSub == null) return;

            var color = _colorDatabase.GetColor(topSub.Color);
            var main = hexFx.main;
            main.startColor = color;

            var fxObject = hexFx.gameObject;
            fxObject.transform.SetParent(null);
            hexFx.Play();

            var duration = hexFx.main.duration + hexFx.main.startLifetime.constantMax;
            Destroy(fxObject, duration);
        }
    }
}
