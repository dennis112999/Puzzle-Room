using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public class Lock : MonoBehaviour, ISwitchable
    {
        private void Awake()
        {
            Deactivate();
        }

        #region ISwitchable

        public bool IsActive { get; private set; }

        public void Activate()
        {
            if (IsActive) return;
            gameObject.SetActive(false);
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            gameObject.SetActive(true);
        }

        #endregion
    }
}
