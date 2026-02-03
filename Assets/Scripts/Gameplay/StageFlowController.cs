using System.Collections.Generic;

using UnityEngine;

using PuzzleRoom.UI;
using PuzzleRoom.Event;

using Core.UI;

using Tools.ServiceLocator;

using Services.EventBus;
using Services.AudioService;
using Unity.VisualScripting;

namespace PuzzleRoom.Gameplay
{
    public sealed class StageFlowController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _roomPrefab;
        [SerializeField] private GameObject _mainCameraPrefab;
        [SerializeField] private GameObject _titleStagePrefab;
        [SerializeField] private List<GameObject> _stagePrefabs;

        [Header("Transition")]
        [SerializeField] private FadeTransitionManager _fade;

        private Transform _roomRoot;
        private Transform _currentStage;

        private GameObject _tabletPlane;

        private GameplayCameraController _cameraController;

        public int CurrentStageId { get; private set; } = 0;

        public void Initialize()
        {   
            EnsureRoomAndCamera();
            SubscribeGameState();

            LoadTitle(true);
        }

        private void OnDestroy()
        {
            UnsubscribeGameState();
        }

        private void SubscribeGameState()
        {
            if (GameController.Instance?.StateMachine == null) return;
            GameController.Instance.StateMachine.StateChanged += OnGameStateChanged;
            ServicesManager.GetService<IEventBus>().Subscribe<RequestLoadStageEvent>(OnRequestLoadStage);
            ServicesManager.GetService<IEventBus>().Subscribe<RequestReLoadStageEvent>(OnRequestReloadStage);
        }

        private void UnsubscribeGameState()
        {
            if (GameController.Instance?.StateMachine == null) return;
            GameController.Instance.StateMachine.StateChanged -= OnGameStateChanged;
            ServicesManager.GetService<IEventBus>().Unsubscribe<RequestLoadStageEvent>(OnRequestLoadStage);
            ServicesManager.GetService<IEventBus>().Unsubscribe<RequestReLoadStageEvent>(OnRequestReloadStage);
        }

        private void EnsureRoomAndCamera()
        {
            _roomRoot = Instantiate(_roomPrefab).transform;
            _tabletPlane = _roomRoot.Find("TabletPlane").gameObject;

            _cameraController = Instantiate(_mainCameraPrefab).GetComponent<GameplayCameraController>();
            _cameraController?.Initialize();

            _fade = FindObjectOfType<FadeTransitionManager>(true);
        }

        private void LoadTitle(bool showMainMenu)
        {
            _tabletPlane?.SetActive(true);
            CurrentStageId = 0;

            LoadStageWithFade(_titleStagePrefab, () =>
            {
                ServicesManager.GetService<IAudioService>().PlayBGM(BGMSoundData.BGM.Title);
                if (showMainMenu)
                {
                    UIPanelManager.PushUIPanel<MainMenuPanel>();
                    return;
                }
                GameController.Instance.ResetToInitialState();
            });
        }


        private void LoadStage(int stageId)
        {
            _tabletPlane?.SetActive(false);
            var prefab = GetStagePrefab(stageId);
            if (prefab == null)
            {
                Debug.LogError($"Stage prefab missing. stageId={stageId}");
                return;
            }

            CurrentStageId = stageId;
            LoadStageWithFade(prefab, () => UIPanelManager.PushUIPanel<GameplayUIPanel>());
            GameController.Instance.ResetToInitialState();
            ServicesManager.GetService<IAudioService>().PlayBGM(BGMSoundData.BGM.Stage);
        }

        private void LoadStageWithFade(GameObject prefab, System.Action afterLoaded = null)
        {
            // No fade manager -> fallback to direct load
            if (_fade == null || _fade.IsPlaying)
            {
                LoadStagePrefab(prefab);
                afterLoaded?.Invoke();
                return;
            }

            _fade.Play(
                onBlack: () =>
                {
                    LoadStagePrefab(prefab);
                },
                onComplete: () =>
                {
                    afterLoaded?.Invoke();
                }
            );
        }

        private GameObject GetStagePrefab(int stageId)
        {
            if (stageId <= 0) return _titleStagePrefab;

            int index = stageId - 1;
            if (_stagePrefabs == null || index < 0 || index >= _stagePrefabs.Count)
                return null;

            return _stagePrefabs[index];
        }

        private void LoadStagePrefab(GameObject prefab)
        {
            if (_roomRoot == null)
            {
                Debug.LogError("Room root is null. Did you call Initialize()?");
                return;
            }

            if (_currentStage != null)
                Destroy(_currentStage.gameObject);

            _currentStage = Instantiate(prefab, _roomRoot).transform;

            var stageController = _currentStage.GetComponent<StageController>();
            stageController?.Initialize();
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Win:
                case GameState.Lose:
                    LoadTitle(false);
                    break;

                case GameState.Closed:
                    CloseGame();
                    break;
            }
        }

        private void CloseGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnRequestLoadStage(RequestLoadStageEvent e)
        {
            LoadStage(e.StageId);
        }

        private void OnRequestReloadStage(RequestReLoadStageEvent e)
        {
            LoadStage(CurrentStageId);
        }
    }
}
