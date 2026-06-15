using System.Collections.Generic;
using Board;
using Config;
using Core;
using UnityEngine;

namespace Stack
{
    public class StackSpawner : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private StackView stackPrefab;
        [SerializeField] private HexView hexPrefab;

        [Header("Spawn Points (UI or world)")] 
        [SerializeField] private Transform[] spawnPoints;

        [Header("Config")] 
        [SerializeField] private GameController gameController;
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private ColorDatabase colorDatabase;

        private readonly List<StackView> _activeStacks = new();
        private Vector3 _hexSpawnPosition;

        public int SpawnInitialStacks()
        {
            Clear();

            foreach (var config in levelConfig.AvailableStacks)
            {
                var spawnPosition = GetSpawnPosition();
                var color = colorDatabase.GetColor(config.Color);

                StackView stack = Instantiate(
                    stackPrefab,
                    spawnPosition,
                    Quaternion.identity,
                    transform);

                stack.Initialize(new StackData
                {
                    Color = config.Color,
                    Count = config.Count
                }, gameController);

                _hexSpawnPosition = spawnPosition;

                for (int i = 0; i < config.Count; i++)
                {
                    HexView hexView = Instantiate(
                        hexPrefab,
                        _hexSpawnPosition,
                        Quaternion.identity,
                        stack.transform);

                    hexView.SetColor(color);
                    _hexSpawnPosition += new Vector3(0f, hexView.Height, 0f);
                }

                _activeStacks.Add(stack);
            }

            return _activeStacks.Count;
        }

        private Vector3 GetSpawnPosition()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return transform.position;

            var spawnPointPosition = spawnPoints[_activeStacks.Count % spawnPoints.Length].position;

            return spawnPointPosition;
        }

        public bool HasAvailableStacks()
        {
            return _activeStacks.Count > 0;
        }

        public void RemoveStack(StackView stack)
        {
            if (!_activeStacks.Contains(stack))
                return;

            _activeStacks.Remove(stack);

            Destroy(stack.gameObject);
        }

        public void Clear()
        {
            foreach (var stack in _activeStacks)
            {
                if (stack != null)
                    Destroy(stack.gameObject);
            }

            _activeStacks.Clear();
        }
    }
}