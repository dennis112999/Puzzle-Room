namespace PuzzleRoom.Gameplay
{
    public enum GameState
    {
        None,
        Initializing,                          
        Pausing,            
        Win,                
        Lose,               
        Closed,             
        ChangeGameMode,
        TopDownMode,
        PrintMode,
        SwitchToPrintView,
        SwitchToTopDownView,
    }
}
