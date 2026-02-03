using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace PuzzleRoom.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 5f;
        public float Speed => _speed;

        [Header("Area")]
        [SerializeField] private Tilemap _tilemap;

        public Rigidbody2D Rb { get; private set; }
        public bool CanMove;
        public bool IsDead { get; private set; }

        public int StageId = 0;

        public Bounds AreaBounds { get; private set; }

        private readonly StateMachine _fsm = new StateMachine();

        // States
        private PlayerMoveState _moveState;
        private PlayerDeadState _deadState;
        private PlayerClearState _clearState;

        public void Initialize()
        {
            Rb = GetComponent<Rigidbody2D>();

            if (_tilemap != null)
            {
                AreaBounds = _tilemap.localBounds;

                if (StageId == 2)
                {
                    AreaBounds = new Bounds(
                        _tilemap.transform.position,
                        new Vector3(16, 16, 0));
                }
            }
            else
            {
                Debug.Log($"Stage id : {StageId}");
            }

            _moveState = new PlayerMoveState(this);
            _deadState = new PlayerDeadState(this);
            _clearState = new PlayerClearState(this);

            _fsm.ChangeState(_moveState);
        }

        private void Update()
        {
            _fsm.Tick();
        }

        private void FixedUpdate()
        {
            _fsm.FixedTick();
        }

        public void EnterDeadState()
        {
            if (IsDead) return;
            IsDead = true;
            _fsm.ChangeState(_deadState);
        }

        private void AnimatePlayerToGoal(Vector3 goalPos)
        {
            _clearState.SetGoal(goalPos);
            _fsm.ChangeState(_clearState);
        }

        #region Collision

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
                Rb.velocity = Vector2.zero;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent<IGoal>(out var goal))
            {
                StageId = goal.StageId;
                AnimatePlayerToGoal(col.transform.position);
            }

            if (col.TryGetComponent<ICollectable>(out var item))
            {
                item.Collect();
            }
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(AreaBounds.center, AreaBounds.size);
        }

#endif
    }
}
