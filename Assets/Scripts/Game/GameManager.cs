using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Actions

    public static event Action PlayMainMenuMusicAction;

    public static event Action StopMainMenuMusicAction;

    public static event Action<Level> OnLevelStartAction;

    public static event Action ClearLevelDataAction;

    public static event Action<float, float, Action> ShowTransitionAction;

    #endregion


    #region Fields

    private Level _currentLevel;

    #endregion
  

    #region Methods

    private void Awake ( ) 
    {
        MainMenuPanel.OnStartGameButtonPressedAction += StartNewGame;
    }

    private void OnDestroy ( ) 
    {
        MainMenuPanel.OnStartGameButtonPressedAction -= StartNewGame;
    }

    private void Start ( ) => StartMainMenu ( fadeInSpeedInSeconds: 0f, fadeOutSpeedInSeconds: 1.5f );

    private void StartNewGame ( ) 
    {
        StopMainMenuMusicAction?.Invoke ( );
    }
    
    private void OnGameEnd ( Level level, bool didPlayerWin ) 
    {
        ClearLevelDataAction?.Invoke ( );
    }

    private void StartMainMenu ( float fadeInSpeedInSeconds, float fadeOutSpeedInSeconds ) 
    {
        PlayMainMenuMusicAction?.Invoke ( );

        ShowTransitionAction?.Invoke ( fadeInSpeedInSeconds, fadeOutSpeedInSeconds, ( ) => {
            GameStateManager.ChangeGameState ( GameState.MainMenu );
        } );
    }

    private void StartLevel ( Level level, float fadeInSpeedInSeconds = 1f, float fadeOutSpeedInSeconds = 1f ) 
    {
        ShowTransitionAction?.Invoke ( fadeInSpeedInSeconds, fadeOutSpeedInSeconds, ( ) => { 
            _currentLevel = level;

            GameStateManager.ChangeGameState ( GameState.InGame );

            OnLevelStartAction?.Invoke ( level );
        } );
    }

    #endregion
}