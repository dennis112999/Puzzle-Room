using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public interface IMovable
    {
        public void Push(Vector2 direction);
    }
}