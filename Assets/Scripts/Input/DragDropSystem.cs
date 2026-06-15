using Core;
using Stack;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class DragDropSystem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Vector3 dragOffset;

        public StackView Stack;

        private GameController _gameController;

        private Vector3 _startPosition;
        private Vector3 _mouseOffset;
        private Plane _dragPlane;
        private Camera _cam;
        private bool _isPlaced;

        public void Initialize(GameController gameController)
        {
            _gameController = gameController;
        }

        private void Awake()
        {
            _cam = Camera.main;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isPlaced)
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
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isPlaced)
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
            if (_isPlaced)
            {
                return;
            }

            if (_gameController.TryPlace(this))
            {
                _isPlaced = true;
            }
        }

        public void Return()
        {
            transform.position = _startPosition;
        }
    }
}