using System.Collections.Generic;
using System.Linq;
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

                StackView stack = Instantiate(
                    stackPrefab,
                    spawnPosition,
                    Quaternion.identity,
                    transform);

                var subStacks = config.Segments
                    .Select(s => new StackData { Color = s.Color, Count = s.Count })
                    .ToList();

                stack.Initialize(subStacks, gameController, colorDatabase);
                stack.SpawnHex();

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