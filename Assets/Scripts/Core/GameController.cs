using System.Collections;
using Animation;
using Board;
using Input;
using Merge;
using Stack;
using UI;
using UnityEngine;

namespace Core
{
    public class GameController : MonoBehaviour
    {
        [Header("Refs")] 
        [SerializeField] private BoardController board;
        [SerializeField] private StackSpawner stackSpawner;
        [SerializeField] private AnimationSystem animationSystem;
        [SerializeField] private ChainReactionSystem chainSystem;
        [SerializeField] private PackshotUI packshotUI;

        [Header("State")] 
        [SerializeField] private GameState state;

        private int _remainingStacks;

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            state = GameState.WaitingInput;

            _remainingStacks = stackSpawner.SpawnInitialStacks();

            chainSystem.ResetSpeed();
        }

        public bool TryPlace(DragDropSystem drag)
        {
            if (state != GameState.WaitingInput)
                return false;

            var target = board.GetNearestEmptyCell(drag.transform.position);

            if (target == null)
            {
                drag.Return();
                return false;
            }

            board.PlaceStack(target, drag.Stack);
            drag.Stack.SetPlaced(true);

            _remainingStacks--;

            StartCoroutine(ResolveBoard());
            
            return true;
        }

        private IEnumerator ResolveBoard()
        {
            state = GameState.Resolving;

            yield return chainSystem.Resolve(board, animationSystem);

            if (CheckWin())
            {
                ShowPackshot();
                yield break;
            }

            /*if (CheckLose())
            {
                state = GameState.WaitingInput;
                yield break;
            }*/

            state = GameState.WaitingInput;
        }

        private bool CheckWin()
        {
            return
                _remainingStacks <= 0
                &&
                !chainSystem.IsResolving;
        }

        private bool CheckLose()
        {
            /*if (remainingStacks > 0)
                return false;

            var occupiedCells = board.OccupiedCells().ToList();

            if (occupiedCells.Count == 0)
                return false;

            foreach (var cell in occupiedCells)
            {
                if (cell.CurrentStacks.Count == 0)
                    continue;

                var group = board.GetNeighbors(cell)
                    .Where(n => !n.IsEmpty && n.CurrentStacks.Count > 0)
                    .Where(n => n.CurrentStacks[^1].Data.Color == cell.CurrentStacks[^1].Data.Color)
                    .Any();

                if (group)
                    return false;
            }*/

            return false;
        }

        private void ShowPackshot()
        {
            state = GameState.Packshot;

            packshotUI.Show();
        }
    }
}