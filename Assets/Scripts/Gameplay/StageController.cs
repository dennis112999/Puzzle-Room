using UnityEngine;
using UnityEngine.Tilemaps;

namespace PuzzleRoom.Gameplay
{
    public class StageController : MonoBehaviour
    {
        [Header("Stage Reference")]
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private Tilemap _tileMap;

        [Header("Camera Reference")]
        [SerializeField] private GameObject _levelCameraPrefab;
        [SerializeField] private GameObject _tileMapCameraPrefab;

        private GameObject _levelCamera;
        private GameObject _tileMapCamera;

        private GameplayCameraController _gameplayCameraController;

        [Header("TileMap Controller Reference")]
        [SerializeField] private TileController _tileController;

        void OnDestroy()
        {
            Destroy(_levelCamera);
            Destroy(_tileMapCamera);
        }

        public void Initialize()
        {
            // Player
            _playerPrefab.Initialize();

            // Camera
            _levelCamera = Instantiate(_levelCameraPrefab);
            _tileMapCamera = Instantiate(_tileMapCameraPrefab);
            MapCamera mapCamera = _tileMapCamera.GetComponent<MapCamera>();
            mapCamera.Initialize(_playerPrefab.transform, _tileMap);
            _gameplayCameraController = Camera.main.GetComponent<GameplayCameraController>();

            // TileController
            if (_tileController != null)
            {
                _tileController.Initialize();
            }

            GameController.Instance.StateMachine.StateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.SwitchToPrintView:
                    _gameplayCameraController.SwitchToPrintView();
                    break;

                case GameState.SwitchToTopDownView:
                    _gameplayCameraController.SwitchToTopDownView();
                    break;

                case GameState.TopDownMode:
                    _gameplayCameraController.SwitchToTopDownView();
                    _playerPrefab.CanMove = true;
                    if (_tileController != null)
                    {
                        _tileController.CanInput = false;
                    }
                    break;

                case GameState.PrintMode:
                    _gameplayCameraController.SwitchToPrintView();
                    _playerPrefab.CanMove = false;
                    if (_tileController != null)
                    {
                        _tileController.CanInput = true;
                    }
                    break;

                case GameState.ChangeGameMode:
                    _playerPrefab.CanMove = false;
                    if (_tileController != null)
                    {
                        _tileController.CanInput = false;
                    }
                    break;

                case GameState.Pausing:
                    _playerPrefab.CanMove = false;

                    if (_tileController != null)
                    {
                        _tileController.CanInput = false;
                    }
                    break;
            }
        }
    }

}