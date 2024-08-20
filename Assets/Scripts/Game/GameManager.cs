using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Actions

    public static event Action OnGameStartAction;

    public static event Action OnGameEndAction;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        HUDManager.OnStartPressedAction += StartGame;
        HUDManager.OnPausePressedAction += PauseGame;
        HUDManager.OnResumePressedAction += ResumeGame;

        Obstacle.OnPlayerHitAction += EndGame;
    }

    private void OnDestroy ( ) 
    {
        HUDManager.OnStartPressedAction -= StartGame;
        HUDManager.OnPausePressedAction -= PauseGame;
        HUDManager.OnResumePressedAction -= ResumeGame;

        Obstacle.OnPlayerHitAction -= EndGame;
    }

    private void Start ( ) => Time.timeScale = 0f;

    private void StartGame ( ) 
    {
        Time.timeScale = 1f;

        OnGameStartAction?.Invoke ( );
    }

    private void PauseGame ( ) => Time.timeScale = 0f;

    private void ResumeGame ( ) => Time.timeScale = 1f;

    private void EndGame ( ) 
    {
        OnGameEndAction?.Invoke ( );

        Time.timeScale = 0f;
    }

    #endregion
}