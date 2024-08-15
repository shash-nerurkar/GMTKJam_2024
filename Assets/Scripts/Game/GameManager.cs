using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Actions

    public static event Action PlayMainMenuMusicAction;

    public static event Action StopMainMenuMusicAction;

    public static event Action<Cutscene> OnCutsceneStartAction;

    public static event Action<Level> OnLevelStartAction;

    public static event Action<Level> StartBattleAction;

    public static event Action ClearLevelDataAction;

    public static event Action<float, float, Action> ShowTransitionAction;

    #endregion


    #region Fields

    private Cutscene _currentCutscene;

    private Level _currentLevel;

    #endregion
  

    #region Methods

    private void Awake ( ) 
    {
        MainMenuPanel.OnStartGameButtonPressedAction += StartNewGame;

        CutscenePanel.OnSequenceCompleteAction += OnCutsceneSequenceComplete;

        DialogueBox.OnSequenceCompleteAction += OnDialogueSequenceComplete;
    }

    private void OnDestroy ( ) 
    {
        MainMenuPanel.OnStartGameButtonPressedAction -= StartNewGame;

        CutscenePanel.OnSequenceCompleteAction -= OnCutsceneSequenceComplete;

        DialogueBox.OnSequenceCompleteAction -= OnDialogueSequenceComplete;
    }

    private void Start ( ) => StartMainMenu ( fadeInSpeedInSeconds: 0f, fadeOutSpeedInSeconds: 1.5f );

    private void StartNewGame ( ) 
    {
        StopMainMenuMusicAction?.Invoke ( );
        
        StartCutscene ( Cutscene.Pilot );
    }
    
    private void OnCutsceneSequenceComplete ( ) 
    {
        switch ( _currentCutscene ) 
        {
            case Cutscene.Pilot:

                break;
        }
    }

    private void OnDialogueSequenceComplete ( bool isOpeningSequence ) 
    {
        if ( isOpeningSequence ) 
            StartBattleAction?.Invoke ( _currentLevel );
        else 
        {
            ClearLevelDataAction?.Invoke ( );
        }
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

    private void StartCutscene ( Cutscene cutscene, float fadeInSpeedInSeconds = 1f, float fadeOutSpeedInSeconds = 1f ) 
    {
        ShowTransitionAction?.Invoke ( fadeInSpeedInSeconds, fadeOutSpeedInSeconds, ( ) => { 
            _currentCutscene = cutscene;

            GameStateManager.ChangeGameState ( GameState.Cutscene ); 

            OnCutsceneStartAction?.Invoke ( cutscene );
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