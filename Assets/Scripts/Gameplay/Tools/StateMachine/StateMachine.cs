namespace PuzzleRoom.Gameplay
{
    public class StateMachine
    {
        private IState _current;

        public void ChangeState(IState next)
        {
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        public void Tick() => _current?.Tick();
        public void FixedTick() => _current?.FixedTick();
    }
}