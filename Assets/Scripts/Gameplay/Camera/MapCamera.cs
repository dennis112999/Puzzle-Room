using UnityEngine;
using UnityEngine.Tilemaps;

namespace PuzzleRoom.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class MapCamera : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _smoothSpeed = 0.125f;
        [SerializeField] private Vector3 _offset;

        private Transform _target;
        private Tilemap _tilemap;

        private Camera _camera;

        private Vector2 _minBoundary;
        private Vector2 _maxBoundary;

        public void Initialize(Transform traget, Tilemap tilemap)
        {
            _target = traget;
            _tilemap = tilemap;

            _camera = GetComponent<Camera>();

            CalculateBounds();
            SnapToTarget();
        }

        private void LateUpdate()
        {
            FollowTarget();
        }

        private void CalculateBounds()
        {
            Bounds bounds = _tilemap.localBounds;

            float verticalExtent = _camera.orthographicSize;
            float horizontalExtent = verticalExtent * _camera.aspect;

            _minBoundary = new Vector2(
                bounds.min.x + horizontalExtent,
                bounds.min.y + verticalExtent
            );

            _maxBoundary = new Vector2(
                bounds.max.x - horizontalExtent,
                bounds.max.y - verticalExtent
            );
        }

        private void FollowTarget()
        {
            if (_target == null) return;

            Vector3 desiredPosition = _target.position + _offset;
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                _smoothSpeed
            );

            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, _minBoundary.x, _maxBoundary.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, _minBoundary.y, _maxBoundary.y);

            transform.position = smoothedPosition;
        }

        private void SnapToTarget()
        {
            if (_target == null) return;
            transform.position = _target.position + _offset;
        }
    }
}
