using System;

public static class GameStateManager
{
    #region Actions

    public static event Action<GameState> OnGameStateChangeAction;

    #endregion


    #region Fields

    public static GameState CurrentGameState { get; private set; }

    #endregion
    

    #region Methods

    public static void ChangeGameState ( GameState newGameState ) 
    {
        CurrentGameState = newGameState;
        
        OnGameStateChangeAction?.Invoke ( CurrentGameState );
    }
    
    #endregion

}
