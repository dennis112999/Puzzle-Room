using DG.Tweening;
using UnityEngine;

using Tools.ServiceLocator;
using Services.AudioService;

namespace PuzzleRoom.Gameplay
{
    public class Key : MonoBehaviour, ICollectable
    {
        [Header("Target")]
        [SerializeField] private MonoBehaviour _targetComponent;

        [Header("Animation Setting")]
        [SerializeField] private float _moveUpDistance = 2f;
        [SerializeField] private float _duration = 0.8f;

        private ISwitchable  Target => _targetComponent as ISwitchable;

        public void Collect()
        {
            ServicesManager.GetService<IAudioService>().PlaySE(SESoundData.SE.Item);
            Target?.Activate();
            PlayCollectAnimation();
        }

        private void PlayCollectAnimation()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var collider = GetComponent<Collider2D>();
            if (collider != null) 
            {
                collider.enabled = false;
            }

            if (spriteRenderer == null)
            {
                Destroy(gameObject);
                return;
            }

            var seq = DOTween.Sequence();
            seq.Append(transform.DOMoveY(transform.position.y + _moveUpDistance, _duration));
            seq.Join(spriteRenderer.DOFade(0f, _duration));
            seq.OnComplete(() => Destroy(gameObject));
        }
    }
}
