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

        private int remainingStacks;

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            state = GameState.WaitingInput;

            remainingStacks = stackSpawner.SpawnInitialStacks();

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

            remainingStacks--;

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

            state = GameState.WaitingInput;
        }

        private bool CheckWin()
        {
            return
                remainingStacks <= 0
                &&
                !chainSystem.IsResolving;
        }

        private void ShowPackshot()
        {
            state = GameState.Packshot;

            packshotUI.Show();
        }
    }
}