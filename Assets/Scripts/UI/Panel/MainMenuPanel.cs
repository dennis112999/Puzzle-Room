using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using PuzzleRoom.Gameplay;

namespace PuzzleRoom.UI
{
    public class MainMenuPanel : BaseUIPanel
    {
        [Header("UI element")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingButton;

        protected override void Subscribe()
        {
            _startButton.onClick.AddListener(OnClickStart);
            _settingButton.onClick.AddListener(OnClickOpenSettings);
        }

        public override void OnDisplay(object parameters)
        {
            SetVisible(true);
        }

        public override void OnHide()
        {
            SetVisible(false);
        }

        private void OnClickStart()
        {
            GameController.Instance.EnterTopDownMode();
            OnCancel();
        }

        private void OnClickOpenSettings()
        {
            UIPanelManager.PushUIPanel<TitleSceneSettingsPanel>();
        }
    }
}