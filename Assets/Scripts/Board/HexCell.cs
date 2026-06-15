using System.Collections.Generic;
using Config;
using Core;
using Stack;
using UnityEngine;

namespace Board
{
    public class HexCell : MonoBehaviour
    {
        public int Id;
        [SerializeField] private float cellHeight;
        public List<HexCell> Neighbors;
        public List<StackView> CurrentStacks = new();
        
        [Header("Start Stack")] 
        [SerializeField] private bool spawnOnStart;
        [SerializeField] private ColorType colorType;
        [SerializeField] [Range(0, 9)] private int count = 1;
        [SerializeField] private StackView stackPrefab;
        [SerializeField] private HexView hexPrefab;
        
        public bool IsEmpty => CurrentStacks.Count == 0;
        public float CellHeight => cellHeight;

        private Vector3 _hexSpawnPosition;
        
        private ColorDatabase _colorDatabase;
        private GameController _gameController;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnInitialStacks();
            }
        }

        public void Initialize(ColorDatabase colorDatabase, GameController gameController)
        {
            _colorDatabase = colorDatabase;
            _gameController = gameController;
        }

        private void SpawnInitialStacks()
        {
            var color = _colorDatabase.GetColor(colorType);
            
            var stackPosition = new Vector3(
                transform.position.x,  
                transform.position.y + cellHeight, 
                transform.position.z);
            
            StackView stack = Instantiate(
                stackPrefab,
                stackPosition,
                Quaternion.identity,
                transform);

            stack.Initialize(new StackData
            {
                Color = colorType,
                Count = count
            }, _gameController);

            _hexSpawnPosition = stackPosition;

            for (int i = 0; i < count; i++)
            {
                HexView hexView = Instantiate(
                    hexPrefab,
                    _hexSpawnPosition,
                    Quaternion.identity,
                    stack.transform);

                hexView.SetColor(color);
                _hexSpawnPosition += new Vector3(0f, hexView.Height, 0f);
            }

            CurrentStacks.Add(stack);
        }
    }
}