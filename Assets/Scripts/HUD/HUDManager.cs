using UnityEngine;

public class HUDManager : MonoBehaviour
{
    #region Serialized Fields

    [ SerializeField ] private InGamePanel inGamePanel;

    [ SerializeField ] private CutscenePanel cutscenePanel;

    [ SerializeField ] private MainMenuPanel mainMenuPanel;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChanged;

        CutsceneManager.StartCutsceneAction += cutscenePanel.StartNewCutscene;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChanged;

        CutsceneManager.StartCutsceneAction -= cutscenePanel.StartNewCutscene;
    }

    private void OnGameStateChanged ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                mainMenuPanel.gameObject.SetActive ( true );

                cutscenePanel.gameObject.SetActive ( false );

                inGamePanel.gameObject.SetActive  ( false );

                break;

            case GameState.Cutscene:
                mainMenuPanel.gameObject.SetActive ( false );

                cutscenePanel.gameObject.SetActive ( true );

                inGamePanel.gameObject.SetActive  ( false );

                break;

            case GameState.InGame:
                mainMenuPanel.gameObject.SetActive ( false );

                cutscenePanel.gameObject.SetActive ( false );

                inGamePanel.gameObject.SetActive  ( true );
                
                break;
        }
    }

    #endregion
}