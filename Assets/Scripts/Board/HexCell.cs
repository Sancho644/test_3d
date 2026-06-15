using System.Collections.Generic;
using Config;
using Core;
using Stack;
using UnityEngine;

namespace Board
{
    public class HexCell : MonoBehaviour
    {
        [SerializeField] private float cellHeight;
        [field:SerializeField] public int Id { get; private set; }
        [field:SerializeField] public List<HexCell> Neighbors { get; private set; }
        [field:SerializeField] public List<StackView> CurrentStacks { get; private set; } = new() ;
        
        [Header("Start Stack")] 
        [SerializeField] private bool spawnOnStart;
        [SerializeField] private ColorType colorType;
        [SerializeField] [Range(0, 9)] private int count = 1;
        [SerializeField] private StackView stackPrefab;
        
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

        public void AddStacks(StackView placedStack)
        {
            var closestStack = CurrentStacks[^1];
            closestStack.Data.Count += placedStack.Data.Count;
        }

        private void SpawnInitialStacks()
        {
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
            }, _gameController, _colorDatabase);
            
            stack.SpawnHex();
            stack.SetCanDrag(false);

            CurrentStacks.Add(stack);
        }
    }
}