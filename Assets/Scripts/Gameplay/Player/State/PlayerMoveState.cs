using DG.Tweening;
using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public sealed class PlayerMoveState : IState
    {
        private readonly Player _p;

        private Vector2 _inputDir;

        public PlayerMoveState(Player player)
        {
            _p = player;
        }

        public void Enter() { }

        public void Tick()
        {
            if (!_p.CanMove)
            {
                _inputDir = Vector2.zero;
                return;
            }

            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            _inputDir = new Vector2(x, y);
        }

        public void FixedTick()
        {
            if (_p.IsDead) return;

            Vector2 dir = _inputDir;
            if (!_p.CanMove || dir == Vector2.zero)
            {
                _p.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutElastic);
                return;
            }

            _p.Rb.MovePosition(_p.Rb.position + _inputDir * _p.Speed * Time.fixedDeltaTime);

            ApplyMovementAnimation();
            CheckPlayerInArea();
        }

        void ApplyMovementAnimation()
        {
            Sequence sequence = DOTween.Sequence();

            if (_inputDir.y != 0)
            {
                sequence.Append(_p.transform.DOScaleY(1.2f, 0.2f).SetEase(Ease.OutElastic));
                sequence.Join(_p.transform.DOScaleX(0.8f, 0.2f).SetEase(Ease.OutElastic));
            }
            else if (_inputDir.x != 0)
            {
                sequence.Append(_p.transform.DOScaleX(1.2f, 0.2f).SetEase(Ease.OutElastic));
                sequence.Join(_p.transform.DOScaleY(0.8f, 0.2f).SetEase(Ease.OutElastic));
            }

            sequence.Append(_p.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.InOutElastic));
        }

        /// <summary>
        /// Check Player Is Inside the Tile map Area Or Not
        /// </summary>
        private void CheckPlayerInArea()
        {
            if (_p.transform.position.x < _p.AreaBounds.min.x || _p.transform.position.x > _p.AreaBounds.max.x ||
                _p.transform.position.y < _p.AreaBounds.min.y || _p.transform.position.y > _p.AreaBounds.max.y)
            {
                _p.EnterDeadState();
            }
        }

        public void Exit() { }
    }
}
