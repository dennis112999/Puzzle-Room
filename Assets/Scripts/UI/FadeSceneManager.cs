using System;
using DG.Tweening;
using UnityEngine;

namespace PuzzleRoom.UI
{
    public class FadeTransitionManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Durations")]
        [SerializeField] private float _fadeOut = 0.25f;
        [SerializeField] private float _hold = 0.05f;
        [SerializeField] private float _fadeIn = 0.25f;

        private Sequence _seq;

        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponentInChildren<CanvasGroup>(true);

            if (_canvasGroup == null)
            {
                Debug.LogError("FadeTransitionManager: CanvasGroup not found.");
                enabled = false;
                return;
            }

            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Play(Action onBlack, Action onComplete = null)
        {
            if (IsPlaying) return;

            IsPlaying = true;

            _seq?.Kill();
            _canvasGroup.blocksRaycasts = true;

            _seq = DOTween.Sequence()
                .Append(_canvasGroup.DOFade(1f, _fadeOut))
                .AppendCallback(() => onBlack?.Invoke())
                .AppendInterval(_hold)
                .Append(_canvasGroup.DOFade(0f, _fadeIn))
                .AppendCallback(() =>
                {
                    _canvasGroup.blocksRaycasts = false;
                    IsPlaying = false;
                    onComplete?.Invoke();
                })
                .SetUpdate(true);
        }
    }
}
