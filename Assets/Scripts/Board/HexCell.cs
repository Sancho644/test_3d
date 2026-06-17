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
        [field:SerializeField] public List<StackData> CurrentStacks { get; private set; } = new();
        
        [Header("Start Stack")] 
        [SerializeField] private bool spawnOnStart;
        [SerializeField] private StackView stackPrefab;
        [SerializeField] private List<StackSpawnSettings> stackSpawnSettings;
        
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
            var stackPosition = new Vector3(
                transform.position.x,  
                transform.position.y + cellHeight, 
                transform.position.z);
            
            foreach (var spawnSetting in stackSpawnSettings)
            {
                if (CurrentStacks.Count != 0)
                {
                    var lastStack = CurrentStacks[^1];
                    var lastView = lastStack.View;
                    if (lastView != null && lastView.HexList.Count > 0)
                    {
                        var lastHexPosition = lastView.HexList[^1].gameObject.transform.position;
                        stackPosition = new Vector3(lastHexPosition.x, lastHexPosition.y + lastView.HexList[^1].Height, lastHexPosition.z);
                    }
                }
                
                var data = new StackData
                {
                    Color = spawnSetting.ColorType,
                    Count = spawnSetting.Count
                };

                StackView stack = Instantiate(
                    stackPrefab,
                    stackPosition,
                    Quaternion.identity,
                    transform);

                stack.Initialize(new List<StackData> { data }, _gameController, _colorDatabase);
                stack.SpawnHex();
                stack.SetCanDrag(false);

                CurrentStacks.Add(data);
            }
        }
    }
}