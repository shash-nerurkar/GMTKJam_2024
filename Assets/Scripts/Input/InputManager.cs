using InputCustom;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Actions

    #endregion


    #region Fields

    private InputActionsDefault _inputActionsDefault;

    #endregion

    
    #region Private Methods
    
    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;
        
        _inputActionsDefault = new InputActionsDefault();
        
        SetupInGameActions ( );
    }
    
    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;
    }

    private void OnGameStateChange ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                _inputActionsDefault.Global.Disable ( );
                _inputActionsDefault.InGame.Disable ( );

                break;

            case GameState.Cutscene:
                _inputActionsDefault.Global.Enable ( );
                _inputActionsDefault.InGame.Disable ( );

                break;

            case GameState.InGame:
                _inputActionsDefault.Global.Enable ( );

                break;
        }
    }

    private void SetupInGameActions ( ) 
    {

    }

    #endregion
}
