using System.Collections.Generic;

using PuzzleRoom.Data;

using TMPro;

using UnityEngine;
using UnityEngine.Tilemaps;

using Tools.ServiceLocator;
using Services.AudioService;

namespace PuzzleRoom.Gameplay
{
    public sealed class TileController : MonoBehaviour
    {
        [Header("Section Settings")]
        [SerializeField] private int _sectionSize = 8;
        [SerializeField] private int _bigSectionSize = 16;

        [Header("Tilemaps")]
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private Tile _wallTile;

        [Header("Gameplay")]
        [SerializeField] private int _maxStep = 10;
        [SerializeField] private GameObject _movementTMPGO;
        private TextMeshPro _stepText;

        [Header("References")]
        [SerializeField] private GameObject _player;
        [SerializeField] private List<GameObject> _movableObjects = new();

        [Header("Stage Presets")]
        [SerializeField] private StagePresetSO _stagePreset;

        private readonly List<TilemapSectionData> _sections = new();
        private IAudioService _audioService;

        private int _sectionsPerBigRow;
        private Vector2Int _tilemapBottomLeftCell;

        private Vector2Int _emptySectionCell;

        private int _currentStep;
        public bool CanInput;
        private bool _canMoveTile;

        void OnDestroy()
        {
            Destroy(_stepText);
        }

        private void Update()
        {
            if (!CanInput || !_canMoveTile) return;

            if (Input.GetKeyDown(KeyCode.D)) TryMoveEmpty(Vector2Int.right);
            else if (Input.GetKeyDown(KeyCode.W)) TryMoveEmpty(Vector2Int.up);
            else if (Input.GetKeyDown(KeyCode.A)) TryMoveEmpty(Vector2Int.left);
            else if (Input.GetKeyDown(KeyCode.S)) TryMoveEmpty(Vector2Int.down);
        }

        #region Init

        public void Initialize()
        {
            _sectionsPerBigRow = _bigSectionSize / _sectionSize;

            _stepText = Instantiate(_movementTMPGO).GetComponent<TextMeshPro>();
            _audioService = ServicesManager.GetService<IAudioService>();
            InitializeStage();
        }

        private void InitializeStage()
        {
            BuildSectionCache();

            _currentStep = 0;
            _canMoveTile = _currentStep < _maxStep;
            UpdateStepText();

            ApplyStagePreset();
        }

        private void BuildSectionCache()
        {
            _sections.Clear();

            _tilemapBottomLeftCell = GetTilemapBottomLeftCell(_tilemap);

            int bigRows = _tilemap.size.y / _bigSectionSize;
            int bigCols = _tilemap.size.x / _bigSectionSize;

            for (int bigRow = 0; bigRow < bigRows; bigRow++)
            {
                for (int bigCol = 0; bigCol < bigCols; bigCol++)
                {
                    for (int row = 0; row < _sectionsPerBigRow; row++)
                    {
                        for (int col = 0; col < _sectionsPerBigRow; col++)
                        {
                            int startX = bigCol * _bigSectionSize + col * _sectionSize + _tilemapBottomLeftCell.x;
                            int startY = bigRow * _bigSectionSize + row * _sectionSize + _tilemapBottomLeftCell.y;

                            _sections.Add(new TilemapSectionData(startX, startY, _sectionSize, _tilemap));
                        }
                    }
                }
            }
        }

        private static Vector2Int GetTilemapBottomLeftCell(Tilemap tilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            return new Vector2Int(bounds.xMin, bounds.yMin);
        }

        private void ApplyStagePreset()
        {
            ClearSectionAsEmpty(_stagePreset.emptySectionIndex);

            foreach (var move in _stagePreset.initialMoves)
            {
                TryMoveEmpty(StagePresetSO.ToVector2Int(move), true);
            }
        }

        #endregion

        #region Sections

        private void ClearSectionAsEmpty(int sectionIndex)
        {
            if (!IsValidSection(sectionIndex))
            {
                Debug.LogError("Section index out of range.");
                return;
            }

            TilemapSectionData section = _sections[sectionIndex];
            section.ClearTiles(_tilemap);

            // Convert sectionIndex to cell position of the section bottom-left
            int sectionsPerRow = _bigSectionSize / _sectionSize;

            int row = sectionIndex / sectionsPerRow;
            int col = sectionIndex % sectionsPerRow;

            int cellX = (col * _sectionSize) + _tilemapBottomLeftCell.x;
            int cellY = (row * _sectionSize) + _tilemapBottomLeftCell.y;

            _emptySectionCell = new Vector2Int(cellX, cellY);

            FillWallTiles(cellX, cellY);
            DrawOrMoveBorder(cellX, cellY);
        }

        private void FillWallTiles(int startX, int startY)
        {
            if (_wallTilemap == null || _wallTile == null) return;

            for (int x = 0; x < _sectionSize; x++)
            {
                for (int y = 0; y < _sectionSize; y++)
                {
                    _wallTilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), _wallTile);
                }
            }
        }

        private void DrawOrMoveBorder(int startX, int startY)
        {
            var lineRenderer = _wallTilemap.GetComponentInChildren<LineRenderer>();
            if (lineRenderer == null)
            {
                var border = CreateBorderObject();
                border.transform.SetParent(_wallTilemap.transform, false);
                lineRenderer = border.GetComponent<LineRenderer>();
            }

            Vector3[] corners = new Vector3[5];
            corners[0] = _wallTilemap.CellToWorld(new Vector3Int(startX, startY, 0));
            corners[1] = _wallTilemap.CellToWorld(new Vector3Int(startX + _sectionSize, startY, 0));
            corners[2] = _wallTilemap.CellToWorld(new Vector3Int(startX + _sectionSize, startY + _sectionSize, 0));
            corners[3] = _wallTilemap.CellToWorld(new Vector3Int(startX, startY + _sectionSize, 0));
            corners[4] = corners[0];

            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);
        }

        private GameObject CreateBorderObject()
        {
            GameObject border = new GameObject("SectionBorder");
            LineRenderer lr = border.AddComponent<LineRenderer>();

            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;

            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
            
            lr.sortingOrder = 3;

            border.layer = 7;

            return border;
        }

        private bool IsValidSection(int sectionIndex)
        {
            return sectionIndex >= 0 && sectionIndex < _sections.Count;
        }

        #endregion

        #region Movement

        private void TryMoveEmpty(Vector2Int direction, bool isInit = false)
        {
            int emptySectionIndex = CalculateSectionIndex(_emptySectionCell);
            Vector2Int targetEmptyCell = _emptySectionCell + direction * _sectionSize;

            if (!IsCellInsideTilemap(targetEmptyCell))
                return;

            int targetSectionIndex = CalculateSectionIndex(targetEmptyCell);
            if (!IsValidSection(targetSectionIndex))
                return;

            if (!isInit && IsPlayerInSection(targetSectionIndex))
            {
                _audioService.PlaySE(SESoundData.SE.Wrong);
                return;
            }

            SwapSectionTiles(_tilemap, emptySectionIndex, targetSectionIndex);
            SwapSectionTiles(_wallTilemap, targetSectionIndex, emptySectionIndex);

            MoveObjectsInSection(targetSectionIndex, direction);

            _emptySectionCell = targetEmptyCell;

            if (!isInit)
            {
                _currentStep++;
                _currentStep = Mathf.Clamp(_currentStep, 0, _maxStep);

                _canMoveTile = _currentStep < _maxStep;
                UpdateStepText();
            }
        }

        private bool IsCellInsideTilemap(Vector2Int cell)
        {
            Vector2Int topRightCell = _tilemapBottomLeftCell + new Vector2Int(_tilemap.size.x, _tilemap.size.y) - Vector2Int.one;

            return cell.x >= _tilemapBottomLeftCell.x &&
                   cell.x <= topRightCell.x &&
                   cell.y >= _tilemapBottomLeftCell.y &&
                   cell.y <= topRightCell.y;
        }

        private int CalculateSectionIndex(Vector2Int cell)
        {
            Vector2Int relative = cell - _tilemapBottomLeftCell;

            int row = ((relative.y % _bigSectionSize) + _bigSectionSize) % _bigSectionSize / _sectionSize;
            int col = ((relative.x % _bigSectionSize) + _bigSectionSize) % _bigSectionSize / _sectionSize;

            return row * _sectionsPerBigRow + col;
        }

        private void SwapSectionTiles(Tilemap tilemap, int aIndex, int bIndex)
        {
            if (tilemap == null) return;

            TilemapSectionData a = _sections[aIndex];
            TilemapSectionData b = _sections[bIndex];

            for (int x = 0; x < _sectionSize; x++)
            {
                for (int y = 0; y < _sectionSize; y++)
                {
                    Vector3Int aCell = new Vector3Int(a.StartX + x, a.StartY + y, 0);
                    Vector3Int bCell = new Vector3Int(b.StartX + x, b.StartY + y, 0);

                    TileBase aTile = tilemap.GetTile(aCell);
                    TileBase bTile = tilemap.GetTile(bCell);

                    tilemap.SetTile(aCell, bTile);
                    tilemap.SetTile(bCell, aTile);
                }
            }

            MoveSectionGameObject(aIndex, bIndex);

            if (tilemap == _wallTilemap)
                DrawOrMoveBorder(a.StartX, a.StartY);
        }

        #endregion

        #region Objects / Player

        private void MoveObjectsInSection(int sectionIndex, Vector2Int direction)
        {
            if (_movableObjects == null) return;

            for (int i = 0; i < _movableObjects.Count; i++)
            {
                var obj = _movableObjects[i];
                if (obj == null) continue;

                if (!IsWorldObjectInSection(sectionIndex, obj.transform.position))
                    continue;

                obj.transform.position -= new Vector3(direction.x * _sectionSize, direction.y * _sectionSize, 0);
            }
        }

        private bool IsPlayerInSection(int sectionIndex)
        {
            if (_player == null) return false;
            return IsWorldObjectInSection(sectionIndex, _player.transform.position);
        }

        private bool IsWorldObjectInSection(int sectionIndex, Vector3 worldPos)
        {
            Vector3Int cell = _tilemap.WorldToCell(worldPos);
            Vector2Int relative = new Vector2Int(cell.x - _tilemapBottomLeftCell.x, cell.y - _tilemapBottomLeftCell.y);

            int row = relative.y / _sectionSize;
            int col = relative.x / _sectionSize;

            int index = row * _sectionsPerBigRow + col;
            return index == sectionIndex;
        }

        private void MoveSectionGameObject(int fromSectionIndex, int toSectionIndex)
        {
            TilemapSectionData from = _sections[fromSectionIndex];
            TilemapSectionData to = _sections[toSectionIndex];

            if (from.sectionGameObject == null)
                return;

            Vector3 toWorld = _tilemap.CellToWorld(new Vector3Int(to.StartX, to.StartY, 0));
            from.sectionGameObject.transform.position = toWorld;

            to.sectionGameObject = from.sectionGameObject;
            from.sectionGameObject = null;
        }

        #endregion

        #region UI

        private void UpdateStepText()
        {
            if (_stepText == null) return;
            _stepText.text = $"{_currentStep} / {_maxStep}";
        }

        #endregion
    }
}
