namespace PuzzleRoom.Gameplay
{
    public class GameController
    {
        #region Singleton

        private static GameController s_instance;
        public static GameController Instance => s_instance ??= new GameController();

        #endregion

        public int StageId;

        public GameStateMachine StateMachine { get; private set; }
        public GameState State => StateMachine.State;

        private GameState _saveGameState = GameState.None;
        
        private GameController() { }

        public void Initialize()
        {
            StateMachine = new GameStateMachine();
            StateMachine.State = GameState.Initializing;
        }

        public void Dispose()
        {
            StateMachine?.Dispose();
            StateMachine = null;
        }

        public void ResetToInitialState()
        {
            StateMachine.State = GameState.Initializing;
            EnterTopDownMode();
        }

        /// <summary>
        /// Resume gameplay mode based on current mode state
        /// </summary>
        public void ResumeGameplay()
        {
            if (_saveGameState == GameState.PrintMode)
            {
                EnterPrintMode();
                return;
            }

            EnterTopDownMode();

            _saveGameState = GameState.None;
        }

        public void ChangeMode()
        {
            if(State == GameState.TopDownMode)
            {
                StateMachine.State = GameState.SwitchToPrintView;
            }
            else if(State == GameState.PrintMode)
            {
                StateMachine.State = GameState.SwitchToTopDownView;
            }
        }

        public void PauseGame()
        {
            _saveGameState = State;
            StateMachine.State = GameState.Pausing;
        }

        public void CloseGame()
        {
            StateMachine.State = GameState.Closed;
        }

        /// <summary>
        /// Called when starting camera transition
        /// </summary>
        public void BeginModeTransition()
        {
            StateMachine.State = GameState.ChangeGameMode;
        }

        public void EnterTopDownMode()
        {
            StateMachine.State = GameState.TopDownMode;
        }

        public void EnterPrintMode()
        {
            StateMachine.State = GameState.PrintMode;
        }

        public void EnterGameWin()
        {
            StateMachine.State = GameState.Win;
        }

        public void EnterGameLose()
        {
            StateMachine.State = GameState.Lose;
        }
    }
}
