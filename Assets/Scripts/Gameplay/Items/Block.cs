using UnityEngine;
using System.Collections;

namespace PuzzleRoom.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Block : MonoBehaviour, IMovable
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _moveDistance = 1f;

        private bool _isMoving = false;

        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Push(Vector2 direction)
        {
            if (_isMoving) return;
            StartCoroutine(MoveRoutine(direction));
        }

        private IEnumerator MoveRoutine(Vector2 direction)
        {
            _isMoving = true;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + (Vector3)(direction * _moveDistance);

            float duration = _moveDistance / _moveSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            _isMoving = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // _canMove = false;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                // _canMove = true;
            }
        }
    }
}
