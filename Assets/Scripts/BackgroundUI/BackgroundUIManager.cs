using UnityEngine;
using UnityEngine.UI;

public class BackgroundUIManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private Image backgroundImage;

    #endregion

    
    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;

        GameManager.OnLevelStartAction += OnLevelStart;
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;

        GameManager.OnLevelStartAction -= OnLevelStart;
    }

    private void OnGameStateChange ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                

                break;

            case GameState.Cutscene:
                backgroundImage.sprite = null;

                break;

            case GameState.InGame:
                break;
        }
    }

    private void OnLevelStart ( Level level ) 
    {
        
    }

    #endregion
}
