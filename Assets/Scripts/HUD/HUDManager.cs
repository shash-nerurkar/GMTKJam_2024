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

    public static event Action<float> AdjustVolumeAction;

    public static event Action<float> AdjustSensitivityAction;

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

    [ Header ( "Sensitivity" ) ]

    [ SerializeField ] private Slider sensitivitySlider;

    [ Header ( "Volume" ) ]

    [ SerializeField ] private Slider volumeSlider;

    [ Header ( "Bottom Panel" ) ]

    [ SerializeField ] private Slider musicProgressSlider;

    [ SerializeField ] private TextMeshProUGUI timeDisplayLabel;

    [ SerializeField ] private TextMeshProUGUI collectibleCountDisplayLabel;

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

    private int _currentCollectibleCount;

    #endregion

    
    #region Methods


    #region Public

    public void OnStartButtonPressed ( ) 
    {

        switch ( _startButtonState ) 
        {
            case StartButtonState.Start:
                // SoundManager.Instance.Play ( SoundType.UIClicked );
                
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                _timeDisplayCoroutine = RunTimer ( );
                StartCoroutine ( _timeDisplayCoroutine );
                UpdateCollectibleCountDisplay ( );

                pauseBlockPanel.SetActive ( false );

                OnStartPressedAction?.Invoke ( );

                break;

            case StartButtonState.Pause:
                SoundManager.Instance.Play ( SoundType.UIClicked );
                
                _startButtonState = StartButtonState.Play;
                startButtonIcon.sprite = playIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StopCoroutine ( _timeDisplayCoroutine );

                pauseBlockPanel.SetActive ( true );
                
                OnPausePressedAction?.Invoke ( );

                break;

            case StartButtonState.Play:
                // SoundManager.Instance.Play ( SoundType.UIClicked );
                
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StartCoroutine ( _timeDisplayCoroutine );

                pauseBlockPanel.SetActive ( false );
                
                OnResumePressedAction?.Invoke ( );
            
                break;
        }
    }

    public void OnVolumeBarValueChanged ( float newValue ) => AdjustVolumeAction?.Invoke ( newValue );

    public void OnSensitivityBarValueChanged ( float newValue ) => AdjustSensitivityAction?.Invoke ( newValue );

    public void OnPreviousButtonPressed ( ) { }

    public void OnNextButtonPressed ( ) { }

    public void OnHeartButtonPressed ( ) => heartButtonIcon.sprite = heartButtonIcon.sprite.Equals ( heartEmptyIcon ) ? heartFilledIcon : heartEmptyIcon;

    public void OnStatsForNerdsButtonPressed ( ) { }

    #endregion


    private void Awake ( ) 
    {
        GameManager.OnGameEndAction += OnGameEnd;

        Collectible.OnPlayerHitAction += UpdateCollectibleCountDisplay;
    }
    
    private void OnDestroy ( ) 
    {
        GameManager.OnGameEndAction -= OnGameEnd;

        Collectible.OnPlayerHitAction += UpdateCollectibleCountDisplay;
    }

    private void Start ( ) 
    {
        AdjustSensitivityAction?.Invoke ( sensitivitySlider.value );
        
        AdjustVolumeAction?.Invoke ( volumeSlider.value );
    }

    private void OnGameEnd ( ) 
    {
        _startButtonState = StartButtonState.Start;
        startButtonIcon.sprite = restartIcon;
        startButtonIcon.color = Constants.ThemeGreenColor;

        StopCoroutine ( _timeDisplayCoroutine );
        
        pauseBlockPanel.SetActive ( true );

        _currentTimeInSeconds = 0;
        _currentCollectibleCount = 0;
    }


    #region Helpers

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

    private void UpdateCollectibleCountDisplay ( ) 
    {
        collectibleCountDisplayLabel.text = $"Collectibles: {_currentCollectibleCount}";
    }

    #endregion


    #endregion
}