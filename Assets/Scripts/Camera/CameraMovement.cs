using Events;
using Player;
using Player.ActionHandlers;
using UnityEngine;
using Utils.Singleton;

namespace Camera
{
    public class CameraMovement : DontDestroyMonoBehaviourSingleton<CameraMovement>
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float moveThreshold = .5f;
        
        private ClickHandler _clickHandler;
        private Vector3 _previousPosition;
        private Vector3 _startPosition;
        private Vector3 _borderMin;
        private Vector3 _borderMax;

        protected override void Init()
        {
            base.Init();
            
            _startPosition = transform.position;
            
            _clickHandler = ClickHandler.Instance;
            _clickHandler.DragStartEvent += OnDragStart;
            _clickHandler.DragPositionEvent += OnDragPosition;
            
            EventsController.Subscribe<EventModels.Game.TargetColorNodesFilled>(this, OnTargetColorNodesFilled);
        }

        private void OnDestroy()
        {
            _clickHandler.DragStartEvent -= OnDragStart;
            _clickHandler.DragPositionEvent -= OnDragPosition;
            
            EventsController.Unsubscribe<EventModels.Game.TargetColorNodesFilled>(OnTargetColorNodesFilled);
        }

        public void SetBorders(Vector3 min, Vector3 max)
        {
            _borderMax = max;
            _borderMin = min;
        }
        
        private void OnDragStart(Vector3 position)
        {
            _previousPosition = position;
        }

        private void OnDragPosition(Vector3 position)
        {
            if (PlayerController.PlayerState != PlayerState.Scrolling)
                return;

            TryMove(position);
        }

        private void OnTargetColorNodesFilled(EventModels.Game.TargetColorNodesFilled e)
        {
            transform.position = _startPosition;
        }

        private void TryMove(Vector3 position)
        {
            var delta = _previousPosition - position;
            
            if (delta.magnitude < moveThreshold)
                return;
            
            Vector3 targetPosition = transform.position + new Vector3(delta.x, delta.y, 0);
            
            targetPosition.x = Mathf.Clamp(targetPosition.x, _borderMin.x, _borderMax.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, _borderMin.y, _borderMax.y);
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}