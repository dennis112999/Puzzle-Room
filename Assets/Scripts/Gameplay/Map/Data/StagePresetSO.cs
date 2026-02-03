using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleRoom.Data
{
    [CreateAssetMenu(fileName = "StagePreset", menuName = "PuzzleRoom/Stage/Stage Preset")]
    public sealed class StagePresetSO : ScriptableObject
    {
        [Header("Empty Section")]
        [Min(0)] public int emptySectionIndex = 0;

        [Header("Initial Moves")]
        public List<MoveDirection> initialMoves = new();

        [Serializable]
        public enum MoveDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public static Vector2Int ToVector2Int(MoveDirection dir)
        {
            return dir switch
            {
                MoveDirection.Up => Vector2Int.up,
                MoveDirection.Down => Vector2Int.down,
                MoveDirection.Left => Vector2Int.left,
                MoveDirection.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }
    }
}
