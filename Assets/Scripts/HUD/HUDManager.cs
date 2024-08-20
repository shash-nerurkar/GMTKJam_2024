using System;
using System.Collections;
using DG.Tweening;
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


    #region Serialized

    [ Header ( "Top Panel" ) ]

    [ SerializeField ] private GameObject pauseBlockPanel;

    [ SerializeField ] private BackgroundVisualizer visualizer;
    
    [ Header ( "Start button" ) ]

    [ SerializeField ] private Image startButtonIcon;

    [ SerializeField ] private Sprite playIcon;

    [ SerializeField ] private Sprite pauseIcon;

    [ SerializeField ] private Sprite restartIcon;
    
    [ Header ( "Heart button" ) ]

    [ SerializeField ] private Animator heartButtonAnimator;

    [ Header ( "Sensitivity" ) ]

    [ SerializeField ] private Slider sensitivitySlider;

    [ Header ( "Volume" ) ]

    [ SerializeField ] private Slider volumeSlider;

    [ Header ( "Progress bar" ) ]

    [ SerializeField ] private Slider musicProgressSlider;

    [ SerializeField ] private TextMeshProUGUI timeLabel;

    [ SerializeField ] private TextMeshProUGUI collectibleCountLabel;

    [ SerializeField ] private GameObject ratingStarsPanel;

    [ SerializeField ] private Image [ ] ratingStars;

    [ SerializeField ] private Sprite ratingStarEmpty;
    
    [ SerializeField ] private Sprite ratingStarFilled;

    [ Header ( "Lore Image" ) ]

    [ SerializeField ] private Image loreImage;

    [ Header ( "Transition" ) ]

    [ SerializeField ] private Transition transition;

    #endregion


    private enum StartButtonState
    {
        Start,
        Pause,
        Play
    }
    private StartButtonState _startButtonState;

    private IEnumerator _timeDisplayCoroutine;

    private int _currentTimeInSeconds;

    private Sequence _showRatingScaleUpSequence;

    private Sequence _showRatingScaleDownSequence;

    #endregion

    
    #region Methods


    #region Public

    public void OnLoreButtonClicked ( ) 
    {
        SoundManager.Instance.Play ( SoundType.PlayClicked );

        transition.FadeIn ( 1f, 1f, ( ) => loreImage.gameObject.SetActive ( false ) );
    }

    public void OnStartButtonPressed ( ) 
    {

        switch ( _startButtonState ) 
        {
            case StartButtonState.Start:
                SoundManager.Instance.Play ( SoundType.GameStarted );
                
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                _timeDisplayCoroutine = RunTimer ( );
                StartCoroutine ( _timeDisplayCoroutine );

                UpdateCollectibleCountDisplay ( 0, 0 );
                HideRating ( );

                pauseBlockPanel.SetActive ( false );

                OnStartPressedAction?.Invoke ( );

                break;

            case StartButtonState.Pause:
                SoundManager.Instance.Play ( SoundType.GamePaused );
                
                _startButtonState = StartButtonState.Play;
                startButtonIcon.sprite = playIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StopCoroutine ( _timeDisplayCoroutine );

                pauseBlockPanel.SetActive ( true );
                
                OnPausePressedAction?.Invoke ( );

                break;

            case StartButtonState.Play:
                SoundManager.Instance.Play ( SoundType.GameResumed );
                
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

    public void OnHeartButtonPressed ( ) => heartButtonAnimator.SetInteger ( "IsFilled", heartButtonAnimator.GetInteger ( "IsFilled" ).Equals ( 1 ) ? -1 : 1 );

    public void OnStatsForNerdsButtonPressed ( ) { }

    #endregion


    private void Awake ( ) 
    {
        GameManager.OnGameEndAction += OnGameEnd;

        NonPlayableEntityManager.ShowRatingAction += ShowRating;
        NonPlayableEntityManager.OnCollectibleCollectedAction += UpdateCollectibleCountDisplay;
    }
    
    private void OnDestroy ( ) 
    {
        GameManager.OnGameEndAction -= OnGameEnd;

        NonPlayableEntityManager.ShowRatingAction -= ShowRating;
        NonPlayableEntityManager.OnCollectibleCollectedAction -= UpdateCollectibleCountDisplay;

        if ( _showRatingScaleUpSequence.IsActive ( ) ) 
            _showRatingScaleUpSequence.Kill ( );

        if ( _showRatingScaleDownSequence.IsActive ( ) ) 
            _showRatingScaleDownSequence.Kill ( );
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

        timeLabel.text =     ( hours < 10 ? "0" : "" ) + hours + ":" + 
                                    ( minutes < 10 ? "0" : "" ) + minutes + ":" + 
                                    ( seconds < 10 ? "0" : "" ) + seconds;
    }

    private void ShowRating ( float NPESpeedScale, float NPESpeedMaxScale, int collectibleCollectedCount, int collectibleSpawnCount ) 
    {
        ratingStarsPanel.SetActive ( true );

        var rating = 0.0f;
        rating += 4f * ( ( NPESpeedScale - 1.0f ) / ( NPESpeedMaxScale - 1.0f ) );
        rating += 1f * ( collectibleSpawnCount == 0 ? 0 : ( ( float ) collectibleCollectedCount / collectibleSpawnCount ) );

        _showRatingScaleUpSequence = DOTween.Sequence ( );
        _showRatingScaleDownSequence = DOTween.Sequence ( );
        var sequenceSpeed = 0.5f;
        
        for ( int i = 0; i < rating; i++ ) 
        {
            ratingStars [ i ].sprite = ratingStarFilled;

            _showRatingScaleUpSequence.Join ( 
                ratingStars [ i ].transform.DOScale ( 1.5f, sequenceSpeed ) 
                    .SetUpdate ( true ) 
            );

            _showRatingScaleDownSequence.Join ( 
                ratingStars [ i ].transform.DOScale ( 1f, sequenceSpeed ) 
                .SetUpdate ( true ) 
            );
        }
        
        _showRatingScaleUpSequence.Play ( )
            .SetUpdate ( true ) 
            .SetDelay ( 1.0f );
        
         _showRatingScaleDownSequence.Play ( )
            .SetUpdate ( true )
            .SetDelay ( 2.0f );
    }

    private void HideRating ( ) 
    {
        ratingStarsPanel.SetActive ( false );

        if ( _showRatingScaleUpSequence.IsActive ( )  ) 
            _showRatingScaleUpSequence.Kill ( );
        
        if ( _showRatingScaleDownSequence.IsActive ( )  ) 
            _showRatingScaleDownSequence.Kill ( );

        for ( int i = 0; i < ratingStars.Length; i++ ) 
        {
            ratingStars [ i ].transform.localScale = Vector3.one;
            ratingStars [ i ].sprite = ratingStarEmpty;
        }
    }

    private void UpdateCollectibleCountDisplay ( int collectedCollectibleCount, int totalCollectibleCount ) 
    {
        collectibleCountLabel.text = $"{collectedCollectibleCount}/{totalCollectibleCount}";
    }

    #endregion


    #endregion
}