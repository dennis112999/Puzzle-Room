using DG.Tweening;

using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public class GameplayCameraController : MonoBehaviour
    {
        [Header("Top Down View")]
        [SerializeField] private Vector3 _topDownPosition = new Vector3(-46.14f, 1.3f, 0.37f);
        [SerializeField] private Vector3 _topDownEuler = new Vector3(0f, 180f, 0f);

        [Header("Print View")]
        [SerializeField] private Vector3 _printPosition = new Vector3(-46.14f, 3.52f, 0.3f);
        [SerializeField] private Vector3 _printEuler = new Vector3(90f, 180f, 0f);

        [Header("Transition")]
        [SerializeField] private float _transitionDuration = 2f;

        private bool _isAnimation;
        private Tween _currentTween;

        public void Initialize()
        {
            transform.SetPositionAndRotation(_printPosition, Quaternion.Euler(_printEuler));
        }

        public void SwitchToPrintView()
        {
            if (_isAnimation) return;

            _isAnimation = true;
            GameController.Instance.BeginModeTransition();
            MoveTo(_printPosition, Quaternion.Euler(_printEuler), GameState.PrintMode);
        }

        public void SwitchToTopDownView()
        {
            if (_isAnimation) return;

            _isAnimation = true;
            GameController.Instance.BeginModeTransition();
            MoveTo(_topDownPosition, Quaternion.Euler(_topDownEuler), GameState.TopDownMode);
        }

        private void MoveTo(Vector3 position, Quaternion rotation, GameState nextState)
        {
            _currentTween?.Kill();

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(position, _transitionDuration));
            seq.Join(transform.DORotateQuaternion(rotation, _transitionDuration));
            seq.OnComplete(() => 
                ApplyGameState(nextState)
            );

            _currentTween = seq;
        }

        private void ApplyGameState(GameState state)
        {
           if (state == GameState.TopDownMode)
           {
               GameController.Instance.EnterTopDownMode();   
           }
           else if (state == GameState.PrintMode)
           {   
               GameController.Instance.EnterPrintMode();
           }

            _isAnimation = false;
        }
    }
}
