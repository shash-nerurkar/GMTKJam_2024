using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    #region Actions

    public static event Action OnStartPressedAction;

    public static event Action OnPausePressedAction;

    public static event Action OnResumePressedAction;

    public static event Action<float> AdjustVolumeAAction;

    #endregion
    

    #region Fields

    [ Header ( "Top Panel" ) ]

    [ SerializeField ] private GameObject pauseBlockPanel;

    [ SerializeField ] private BackgroundVisualizer visualizer;
    
    [ Header ( "Start button" ) ]

    [ SerializeField ] private Image startButtonIcon;

    [ SerializeField ] private Sprite playIcon;

    [ SerializeField ] private Sprite pauseIcon;

    [ SerializeField ] private Sprite restartIcon;
    
    [ Header ( "Heart button" ) ]

    [ SerializeField ] private Image heartButtonIcon;

    [ SerializeField ] private Sprite heartEmptyIcon;

    [ SerializeField ] private Sprite heartFilledIcon;

    [ Header ( "Music Progress" ) ]

    [ SerializeField ] private Slider musicProgressSlider;

    [ Header ( "Time display" ) ]

    [ SerializeField ] private TextMeshProUGUI timeDisplayLabel;

    [ Header ( "Transition" ) ]

    [ SerializeField ] private Transition transition;

    private enum StartButtonState
    {
        Start,
        Pause,
        Play
    }
    private StartButtonState _startButtonState;

    private IEnumerator _timeDisplayCoroutine;
    private int _currentTimeInSeconds;

    #endregion

    
    #region Methods


    #region Public

    public void OnStartButtonPressed ( ) 
    {

        switch ( _startButtonState ) 
        {
            case StartButtonState.Start:
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                _currentTimeInSeconds = 0;
                _timeDisplayCoroutine = RunTimer ( );
                StartCoroutine ( _timeDisplayCoroutine );

                OnStartPressedAction?.Invoke ( );
                pauseBlockPanel.SetActive ( false );

                break;

            case StartButtonState.Pause:
                SoundManager.Instance.Play ( SoundType.UIClicked );
                
                _startButtonState = StartButtonState.Play;
                startButtonIcon.sprite = playIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StopCoroutine ( _timeDisplayCoroutine );
                
                OnPausePressedAction?.Invoke ( );
                pauseBlockPanel.SetActive ( true );

                break;

            case StartButtonState.Play:
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StartCoroutine ( _timeDisplayCoroutine );
                
                OnResumePressedAction?.Invoke ( );
                pauseBlockPanel.SetActive ( false );
            
                break;
        }
    }

    public void OnVolumeBarValueChanged ( float newValue ) => AdjustVolumeAAction?.Invoke ( newValue );

    public void OnPreviousButtonPressed ( ) { }

    public void OnNextButtonPressed ( ) { }

    public void OnHeartButtonPressed ( ) => heartButtonIcon.sprite = heartButtonIcon.sprite.Equals ( heartEmptyIcon ) ? heartFilledIcon : heartEmptyIcon;

    public void OnStatsForNerdsButtonPressed ( ) { }

    #endregion


    private void Awake ( ) 
    {
        // GameManager.ShowTransitionAction += transition.FadeIn;
        GameManager.OnGameEndAction += SetStateToRestart;
    }
    
    private void OnDestroy ( ) 
    {
        // GameManager.ShowTransitionAction -= transition.FadeIn;
        GameManager.OnGameEndAction -= SetStateToRestart;
    }

    private void Start ( ) => musicProgressSlider.value = 0;

    private void SetStateToRestart ( ) 
    {
        _startButtonState = StartButtonState.Start;
        startButtonIcon.sprite = restartIcon;
        startButtonIcon.color = Constants.ThemeGreenColor;

        StopCoroutine ( _timeDisplayCoroutine );
        
        pauseBlockPanel.SetActive ( true );
    }

    private IEnumerator RunTimer ( ) 
    {
        while ( true ) 
        {
            ++_currentTimeInSeconds;
            UpdateTimerDisplay ( );

            yield return new WaitForSeconds ( 1 );
        }
    }

    private void UpdateTimerDisplay ( ) 
    {
        var hours = _currentTimeInSeconds / 3600;
        var minutes = ( _currentTimeInSeconds / 60 ) % 60;
        var seconds = _currentTimeInSeconds % 60;

        timeDisplayLabel.text =     ( hours < 10 ? "0" : "" ) + hours + ":" + 
                                    ( minutes < 10 ? "0" : "" ) + minutes + ":" + 
                                    ( seconds < 10 ? "0" : "" ) + seconds;
    }

    #endregion
}