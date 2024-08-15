using System;
using UnityEngine;

public class MainMenuPanel : MonoBehaviour
{
    #region Actions

    public static event Action OnStartGameButtonPressedAction;

    #endregion
    

    #region Methods

    public void StartGameButtonPressed ( ) 
    {
        SoundManager.Instance.Play ( SoundType.OnUIClicked );
        
        OnStartGameButtonPressedAction?.Invoke ( );
    }

    public void ExitGameButtonPressed ( ) 
    {
        SoundManager.Instance.Play ( SoundType.OnUIClicked );
        
        Application.Quit ( );
    }

    #endregion
}
