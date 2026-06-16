using Core;
using Stack;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class DragDropSystem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Vector3 dragOffset;
        [field:SerializeField] public StackView Stack { get; private set; }
        
        private GameController _gameController;
        private TutorialSystem _tutorialSystem;

        private Vector3 _startPosition;
        private Vector3 _mouseOffset;
        private Plane _dragPlane;
        private Camera _cam;
        private bool _canDrag = true;

        public void Initialize(GameController gameController, TutorialSystem tutorialSystem = null)
        {
            _gameController = gameController;
            _tutorialSystem = tutorialSystem;
        }

        private void Start()
        {
            if (_cam == null)
            {
                _cam = Camera.main;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_canDrag || !_gameController.CanAcceptInput)
            {
                return;
            }

            _startPosition = transform.position;

            _dragPlane = new Plane(Vector3.up, transform.position);

            var ray = _cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            if (_dragPlane.Raycast(ray, out float enter))
            {
                var hitPoint = ray.GetPoint(enter);

                _mouseOffset = transform.position - hitPoint;
            }

            if (_tutorialSystem != null)
                _tutorialSystem.OnStackGrabbed();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag)
            {
                return;
            }

            var ray = _cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            if (_dragPlane.Raycast(ray, out float enter))
            {
                var hitPoint = ray.GetPoint(enter);

                transform.position = hitPoint + _mouseOffset + dragOffset;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_canDrag)
            {
                return;
            }

            if (_gameController.TryPlace(this))
            {
                _canDrag = false;
            }
        }

        public void Return()
        {
            transform.position = _startPosition;
        }

        public void SetCanDrag(bool value)
        {
            _canDrag = value;
        }
    }
}