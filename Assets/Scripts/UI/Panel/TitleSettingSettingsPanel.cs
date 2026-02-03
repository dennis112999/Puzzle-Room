using UnityEngine;
using UnityEngine.UI;

using Tools.ServiceLocator;
using Services.AudioService;

using Core.UI;
using PuzzleRoom.Gameplay;

namespace PuzzleRoom.UI
{
    public class TitleSceneSettingsPanel : BaseUIPanel
    {
        private IAudioService _audioService;

        [Header("UI element")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Slider _bGMvolumeSlider;
        [SerializeField] private Slider _sEvolumeSlider;

        protected override void Subscribe()
        {
            _backButton.onClick.AddListener(OnBackClick);
            _exitButton.onClick.AddListener(OnExitClick);

            _bGMvolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
            _sEvolumeSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }

        public override void OnDisplay(object parameters)
        {
            GameController.Instance.PauseGame();

            _audioService = ServicesManager.GetService<IAudioService>();

            _bGMvolumeSlider.value = _audioService.BGMVolume;
            _sEvolumeSlider.value = _audioService.SEVolume;

            SetVisible(true);
        }

        private void OnBackClick()
        {
            GameController.Instance.PauseGame();
            OnCancel();
        }

        private void OnExitClick()
        {
            GameController.Instance.CloseGame();
        }

        private void OnBGMVolumeChanged(float changedValue)
        {
            _audioService.SetBgmVolume(changedValue);
        }

        private void OnSEVolumeChanged(float changedValue)
        {
            _audioService.SetSeVolume(changedValue);
        }
    }
}