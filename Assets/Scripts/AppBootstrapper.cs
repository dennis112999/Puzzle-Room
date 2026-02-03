using DG.Tweening;
using UnityEngine;

using PuzzleRoom.Gameplay;

using Core.UI;
using Core.UI.Data;

using Tools.ServiceLocator;
using Services.EventBus;
using Services.AudioService;

namespace PuzzleRoom
{
    public class AppBootstrapper : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private GameObject _audioServiceGO;
        private AudioService _audioService;

        [Header("UI")]
        [SerializeField] private UIPanelDatabase _uIPanelDatabase;

        [Header("Flow")]
        [SerializeField] private StageFlowController _stageFlow;

        private void Awake()
        {
            InitSettings();
            InitSystems();
            InitUIAndTween();

            _stageFlow.Initialize();
        }

        private void OnDestroy()
        {
            GameController.Instance.Dispose();
            ServicesManager.GetService<IEventBus>().Clear();
            ServicesManager.DisposeAll();
        }

        private void InitSettings()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void InitSystems()
        {
            GameController.Instance.Initialize();
            ServicesManager.RegisterService<IEventBus>(new EventBusService());
            _audioService = Instantiate(_audioServiceGO).GetComponent<AudioService>();
            ServicesManager.RegisterService<IAudioService>(_audioService);
        }

        private void InitUIAndTween()
        {
            UIPanelManager.InitEntryPoint(_uIPanelDatabase);

            DOTween.Init();
            DOTween.defaultAutoPlay = AutoPlay.All;
        }
    }
}
