using UnityEngine;

namespace Core.UI
{
    public interface IUIPanel
    {
        Transform GetTransform();

        /// <summary>
        /// Called when the panel is created (initialize)
        /// </summary>
        void OnCreated();

        /// <summary>
        /// Called when the panel is shown with parameters
        /// </summary>
        /// <param name="parameters">Optional data for display</param>
        void OnDisplay(object parameters);

        /// <summary>
        /// Called when the panel is canceled or closed
        /// </summary>
        void OnCancel();

        /// <summary>
        /// Hide the panel
        /// </summary>
        void OnHide();

        /// <summary>
        /// Resume from a paused/hidden state
        /// </summary>
        void OnResume();

        /// <summary>
        /// Set panel visibility
        /// </summary>
        /// <param name="visible"></param>
        void SetVisible(bool visible);

        /// <summary>
        /// Release resources and destroy
        /// </summary>
        void OnDelete();
    }
}