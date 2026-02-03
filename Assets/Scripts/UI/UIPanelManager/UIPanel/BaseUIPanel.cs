using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseUIPanel : MonoBehaviour, IUIPanel
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;

        public Canvas Canvas => _canvas;

        public Transform GetTransform()
        {
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }

        /// <summary>
        /// Called when the panel is created
        /// </summary>
        protected abstract void Subscribe();

        protected virtual void Unsubscribe()
        {
            
        }

        public virtual void OnCreated()
        {
            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();

            Subscribe();

            SetVisible(false);
        }

        public abstract void OnDisplay(object parameters);

        public virtual void OnCancel()
        {
            UIPanelManager.PopPanel(this);
        }

        public virtual void OnHide()
        {
            SetVisible(false);
        }

        public virtual void OnResume()
        {
            SetVisible(true);
        }

        public virtual void OnDelete()
        {
            Destroy(gameObject);
        }

        public void SetVisible(bool visible)
        {
            _canvas.enabled = visible;
            if (_canvas.TryGetComponent(out GraphicRaycaster raycaster))
            {
                raycaster.enabled = visible;
            }
        }
    }
}
