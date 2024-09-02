using System;
using InputCustom;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Action

    public static event Action ToggleGameStateAction;

    public static event Action<int> ScaleSideSelectedAction;

    public static event Action ScaleSideUnselectedAction;

    public static event Action<int, int> ScalePlayerAction;

    #endregion


    #region Fields

    private InputActionsDefault _inputActionsDefault;

    #endregion

    
    #region Private Methods
    
    private void Awake ( ) 
    {
        _inputActionsDefault = new InputActionsDefault();
        
        GameManager.OnGameStartAction += EnableInGameActions;
        GameManager.OnGameEndAction += DisableInGameActions;

        HUDManager.OnResumePressedAction += EnableInGameActions;
        HUDManager.OnPausePressedAction += OnPausePressedAction;

        _inputActionsDefault.Global.Enable ( );
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction += EnableInGameActions;
        GameManager.OnGameEndAction += DisableInGameActions;

        HUDManager.OnResumePressedAction -= EnableInGameActions;
        HUDManager.OnPausePressedAction -= OnPausePressedAction;
    }

    private void EnableInGameActions ( ) 
    {
        ScaleSideUnselectedAction?.Invoke ( );

        _inputActionsDefault.InGame.Enable ( );
    }

    private void OnPausePressedAction ( ) 
    {
        ScaleSideSelectedAction?.Invoke ( 2 );
        DisableInGameActions ( );
    }

    private void DisableInGameActions ( ) => _inputActionsDefault.InGame.Disable ( );

    private void Start ( ) 
    {
        _inputActionsDefault.Global.ToggleGameState.started += ctx => ToggleGameStateAction?.Invoke ( );

        _inputActionsDefault.InGame.SetScaleSide.started += ctx => ScaleSideSelectedAction?.Invoke ( ( int ) ctx.ReadValue<float> ( ) );
        _inputActionsDefault.InGame.SetScaleSide.canceled += ctx => ScaleSideUnselectedAction?.Invoke ( );
    }

    private void Update ( ) 
    {
        var scaleSide = _inputActionsDefault.InGame.SetScaleSide.ReadValue<float> ( );
        var scaleDirection = _inputActionsDefault.InGame.SetScaleDirection.ReadValue<float> ( );

        if ( scaleSide != 0 && scaleDirection != 0 ) 
            ScalePlayerAction?.Invoke ( ( int ) scaleSide, ( int ) scaleDirection );
    }

    #endregion
}
