using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public class StageGoal : MonoBehaviour, IGoal
    {
        [SerializeField] private int _stageId;
        public int StageId => _stageId;
    }
}
