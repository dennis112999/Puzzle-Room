namespace PuzzleRoom.Gameplay
{
    public interface ISwitchSource
    {
        bool IsOn { get; }
        void TurnOn();
        void TurnOff();
    }
}
