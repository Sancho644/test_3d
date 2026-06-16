using System.Collections;
using DG.Tweening;
using Stack;
using UnityEngine;

namespace Core
{
    public class TutorialSystem : MonoBehaviour
    {
        [Header("Hand")]
        [SerializeField] private RectTransform hand;
        [SerializeField] private Canvas canvas;
        [SerializeField] private float handBobAmount = 30f;
        [SerializeField] private float handBobDuration = 0.5f;
        [SerializeField] private GameController gameController;

        [Header("Timing")]
        [SerializeField] private float showDelay = 2f;

        private Camera _cam;
        private StackView _targetStack;
        private Sequence _handSequence;
        private Coroutine _showRoutine;
        private bool _tutorialActive;
        private bool _tutorialFinished;

        public bool IsTutorialFinished => _tutorialFinished;

        private void Start()
        {
            if (_cam == null)
                _cam = Camera.main;
        }

        public void StartTutorial(StackView targetStack)
        {
            if (_tutorialFinished)
                return;

            _targetStack = targetStack;
            _tutorialActive = true;

            ShowHand();
        }

        public void OnStackGrabbed()
        {
            if (!_tutorialActive)
                return;

            HideHand();
            _tutorialActive = false;

            SetState(GameState.WaitingInput);
        }

        public void OnStackPlaced()
        {
            if (!_tutorialActive)
                return;

            _tutorialFinished = true;
            _tutorialActive = false;
            HideHand();
        }

        public void OnStackReturned()
        {
            if (_tutorialFinished)
                return;

            if (_showRoutine != null)
                StopCoroutine(_showRoutine);

            _showRoutine = StartCoroutine(ShowAfterDelay());
        }

        private void SetState(GameState newState)
        {
            if (gameController != null)
                gameController.state = newState;
        }

        private Vector2 WorldToCanvasPosition(Vector3 worldPos)
        {
            var screenPos = _cam.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out var localPos);
            return localPos;
        }

        private void ShowHand()
        {
            if (_targetStack == null || hand == null || canvas == null)
                return;

            hand.gameObject.SetActive(true);

            var stackScreenPos = WorldToCanvasPosition(_targetStack.transform.position);
            hand.anchoredPosition = stackScreenPos;

            _handSequence?.Kill();
            _handSequence = DOTween.Sequence();

            var targetPos = stackScreenPos + Vector2.up * handBobAmount;

            _handSequence.Append(hand.DOAnchorPos(targetPos, handBobDuration).SetEase(Ease.InOutSine));
            _handSequence.SetLoops(-1, LoopType.Yoyo);

            SetState(GameState.Tutorial);
        }

        private void HideHand()
        {
            _handSequence?.Kill();
            _handSequence = null;

            if (hand != null)
                hand.gameObject.SetActive(false);

            if (_showRoutine != null)
            {
                StopCoroutine(_showRoutine);
                _showRoutine = null;
            }
        }

        private IEnumerator ShowAfterDelay()
        {
            yield return new WaitForSeconds(showDelay);

            if (!_tutorialFinished && _targetStack != null)
            {
                _tutorialActive = true;
                ShowHand();
            }
        }

        private void OnDestroy()
        {
            _handSequence?.Kill();
        }
    }
}
