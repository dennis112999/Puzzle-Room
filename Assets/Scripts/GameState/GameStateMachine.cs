using System;
using UnityEngine;

namespace PuzzleRoom.Gameplay
{
    public sealed class GameStateMachine : IDisposable
    {
        public event Action<GameState> StateChanged;

        private GameState _state = GameState.None;

        public GameState State
        {
            get => _state;
            set
            {
                if (_state == value) return;

                var prev = _state;
                _state = value;

                Notify();
                
                Debug.Log(
                    $"[GameState] {prev} -> {_state} " +
                    $"| t={Time.realtimeSinceStartup:0.000}s " +
                    $"| frame={Time.frameCount}"
                );
            }
        }

        public void Dispose()
        {
            StateChanged = null;
        }

        public void Notify()
        {
            StateChanged?.Invoke(_state);
        }
    }
}
