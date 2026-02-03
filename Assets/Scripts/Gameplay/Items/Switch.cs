using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public class Switch : MonoBehaviour, ISwitchSource
    {
        [Header("Target")]
        [SerializeField] private MonoBehaviour _targetComponent;
        private ISwitchable Target => _targetComponent as ISwitchable;

        [Header("Visual")]
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        [Header("Trigger Filter")]
        [SerializeField] private string _requiredTag = "Player";

        private SpriteRenderer _spriteRenderer;

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = IsOn ? _onSprite : _offSprite;
        }

        #region ISwitchSource

        public bool IsOn { get; private set; }
        public void TurnOn()
        {
            if (IsOn) return;

            IsOn = true;
            Target.Activate();
            _spriteRenderer.sprite = _onSprite;
        }

        public void TurnOff()
        {
            if (!IsOn) return;

            IsOn = false;
            Target.Deactivate();
            _spriteRenderer.sprite = _offSprite;
        }

        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(_requiredTag)) return;

            TurnOn();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag(_requiredTag)) return;

            TurnOff();
        }
    }

}