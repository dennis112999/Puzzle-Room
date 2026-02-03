using Tools.ServiceLocator;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using PuzzleRoom.Gameplay;

namespace PuzzleRoom.UI
{
    public class GameplayUIPanel : BaseUIPanel
    {
        [Header("UI element")]
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _changeButton;

        protected override void Subscribe()
        {
            _settingButton.onClick.AddListener(OnClickOpenSettings);
            _changeButton.onClick.AddListener(OnChangeButtonClick);
        }

        public override void OnDisplay(object parameters)
        {
            SetVisible(true);
        }

        public override void OnHide()
        {
            SetVisible(false);
        }

        private void OnClickOpenSettings()
        {
            UIPanelManager.PushUIPanel<SettingsPanel>();
        }

        private void OnChangeButtonClick()
        {
            GameController.Instance.ChangeMode();
        }
    }
}