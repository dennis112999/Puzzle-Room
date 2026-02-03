using UnityEngine;
using UnityEngine.UI;

using Tools.ServiceLocator;
using Services.AudioService;

using Core.UI;
using PuzzleRoom.Gameplay;
using Services.EventBus;
using PuzzleRoom.Event;
using Unity.VisualScripting;

namespace PuzzleRoom.UI
{
    public class SettingsPanel : BaseUIPanel
    {
        private IAudioService _audioService;

        [Header("UI element")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _returnToTitleButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Slider _bGMvolumeSlider;
        [SerializeField] private Slider _sEvolumeSlider;

        public override void OnCancel()
        {
            base.OnCancel();

            GameController.Instance.ResumeGameplay();
        }

        protected override void Subscribe()
        {
            _backButton.onClick.AddListener(OnBackClick);
            _restartButton.onClick.AddListener(OnRestartClick);
            _returnToTitleButton.onClick.AddListener(OnReturnToTitleClick);
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
            OnCancel();
        }

        private void OnRestartClick()
        {
            ServicesManager.GetService<IEventBus>().Publish<RequestReLoadStageEvent>(new RequestReLoadStageEvent());
            OnCancel();
        }

        private void OnReturnToTitleClick()
        {
            ServicesManager.GetService<IEventBus>().Publish<RequestLoadStageEvent>(new RequestLoadStageEvent(0));
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