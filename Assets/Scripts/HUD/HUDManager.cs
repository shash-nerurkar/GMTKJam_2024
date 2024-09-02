using System;
using System.Collections;
using System.Linq;
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

    public static event Action OnPreviousTrackPressedAction;

    public static event Action OnNextTrackPressedAction;

    public static event Action<bool> OnHeartTrackToggledAction;

    public static event Action<bool> OnLoopTrackToggledAction;

    public static event Action<float> AdjustSensitivityAction;

    public static event Action<float> AdjustMusicVolumeAction;

    public static event Action<float> AdjustSFXVolumeAction;

    #endregion
    

    #region Fields


    #region Serialized

    [ Header ( "Lore Image" ) ]

    [ SerializeField ] private Image loreImage;

    [ Header ( "Top Panel" ) ]

    [ SerializeField ] private GameObject pauseBlockPanel;
    
    [ Header ( "Buttons" ) ]

    [ SerializeField ] private Image startButtonIcon;

    [ SerializeField ] private Sprite playIcon;

    [ SerializeField ] private Sprite pauseIcon;

    [ SerializeField ] private Sprite restartIcon;

    [ SerializeField ] private Button previousTrackButton;

    [ SerializeField ] private Button nextTrackButton;
    
    [ Header ( "Current track" ) ]

    [ SerializeField ] private ScrollRect currentTrackTitleScrollRect;

    [ SerializeField ] private HorizontalLayoutGroup currentTrackTitleContentLayoutGroup;

    [ SerializeField ] private RectTransform currentTrackTitleContentTransform;

    [ SerializeField ] private TextMeshProUGUI [ ] currentTrackTitleLabels;

    [ SerializeField ] private Button heartButton;

    [ SerializeField ] private Animator heartButtonAnimator;

    [ SerializeField ] private Button SFNButton;

    [ Header ( "Progress" ) ]

    [ SerializeField ] private Slider musicProgressSlider;

    [ SerializeField ] private TextMeshProUGUI timeLabel;

    [ SerializeField ] private TextMeshProUGUI collectibleCountLabel;

    [ Header ( "Settings" ) ]

    [ SerializeField ] private Slider sensitivitySlider;

    [ SerializeField ] private Slider musicVolumeSlider;

    [ SerializeField ] private Slider sfxVolumeSlider;

    [ Header ( "Rating" ) ]

    [ SerializeField ] private GameObject ratingStarsPanel;

    [ SerializeField ] private Image [ ] ratingStars;

    [ SerializeField ] private Sprite ratingStarEmpty;
    
    [ SerializeField ] private Sprite ratingStarHalfFilled;
    
    [ SerializeField ] private Sprite ratingStarFilled;

    [ Header ( "Transition" ) ]

    [ SerializeField ] private Transition transition;

    #endregion
    

    private IEnumerator _currentTrackNameLabelMarqueeCoroutine;

    private enum StartButtonState
    {
        Start,
        Pause,
        Play
    }
    private StartButtonState _startButtonState;

    private IEnumerator _timeDisplayCoroutine;

    private IEnumerator _currentTrackProgressDisplayCoroutine;

    private float _currentRunTimeInSeconds;

    private float _currentTrackTimeInSeconds;

    private Sequence _showRatingScaleUpSequence;

    private Sequence _showRatingScaleDownSequence;

    private bool _isHeartFilled;

    #endregion

    
    #region Methods


    #region Public

    public void OnLoreButtonClicked ( ) 
    {
        SoundManager.Instance.Play ( SoundType.PlayClicked );

        transition.FadeIn ( 1f, 1f, ( ) => loreImage.gameObject.SetActive ( false ) );
    }

    public void OnPreviousButtonPressed ( ) => OnPreviousTrackPressedAction?.Invoke ( );

    public void OnNextButtonPressed ( ) => OnNextTrackPressedAction?.Invoke ( );

    public void OnStartButtonPressed ( ) 
    {
        switch ( _startButtonState ) 
        {
            case StartButtonState.Start:
                if ( _showRatingScaleUpSequence.IsActive ( ) ) 
                {
                    _showRatingScaleUpSequence.Kill ( );
                    _showRatingScaleDownSequence.Complete ( );

                    return;
                }

                SoundManager.Instance.Play ( SoundType.GameStarted, true );
                
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                UpdateCollectibleCountDisplay ( 0, 0 );
                HideRating ( );

                TogglePause ( false );

                OnStartPressedAction?.Invoke ( );

                break;

            case StartButtonState.Pause:
                SoundManager.Instance.Play ( SoundType.GamePaused, true );
                
                _startButtonState = StartButtonState.Play;
                startButtonIcon.sprite = playIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StopCoroutine ( _currentTrackProgressDisplayCoroutine );

                TogglePause ( true );
                
                OnPausePressedAction?.Invoke ( );

                break;

            case StartButtonState.Play:
                SoundManager.Instance.Play ( SoundType.GameResumed, true );
                
                _startButtonState = StartButtonState.Pause;
                startButtonIcon.sprite = pauseIcon;
                startButtonIcon.color = Constants.ThemeOrangeColor;

                StartCoroutine ( _currentTrackProgressDisplayCoroutine );

                TogglePause ( false );
                
                OnResumePressedAction?.Invoke ( );
            
                break;
        }
    }

    public void OnSensitivityBarValueChanged ( float newValue ) => AdjustSensitivityAction?.Invoke ( Mathf.Log10 ( newValue * 9 + 1 ) );

    public void OnMusicVolumeBarValueChanged ( float newValue ) => AdjustMusicVolumeAction?.Invoke ( Mathf.Log10 ( newValue * 9 + 1 ) );

    public void OnSFXVolumeBarValueChanged ( float newValue ) => AdjustSFXVolumeAction?.Invoke ( Mathf.Log10 ( newValue * 9 + 1 ) );

    public void OnHeartButtonPressed ( ) 
    {
        if ( _isHeartFilled ) 
        {
            heartButtonAnimator.SetTrigger ( Constants.ANIM_TRIGGER_HeartButtonEmpty );
        }
        else 
            heartButtonAnimator.SetTrigger ( Constants.ANIM_TRIGGER_HeartButtonFill );

        _isHeartFilled = !_isHeartFilled;

        OnHeartTrackToggledAction?.Invoke ( _isHeartFilled );
    }

    public void OnLoopButtonToggled ( bool toggleFlag ) => OnLoopTrackToggledAction?.Invoke ( toggleFlag );

    public void OnStatsForNerdsButtonPressed ( ) { }

    #endregion


    private void Awake ( ) 
    {
        InputManager.ToggleGameStateAction += OnStartButtonPressed;

        GameManager.OnGameStartAction += OnGameStart;
        GameManager.OnGameEndAction += OnGameEnd;

        SoundManager.OnPlayTrackAction += PlayCurrentTrack;
        SoundManager.OnStopTrackAction += StopCurrentTrack;

        NonPlayableEntityManager.ShowRatingAction += ShowRating;
        NonPlayableEntityManager.OnCollectibleCollectedAction += UpdateCollectibleCountDisplay;
    }
    
    private void OnDestroy ( ) 
    {
        InputManager.ToggleGameStateAction -= OnStartButtonPressed;

        GameManager.OnGameStartAction -= OnGameStart;
        GameManager.OnGameEndAction -= OnGameEnd;

        SoundManager.OnPlayTrackAction -= PlayCurrentTrack;
        SoundManager.OnStopTrackAction -= StopCurrentTrack;

        NonPlayableEntityManager.ShowRatingAction -= ShowRating;
        NonPlayableEntityManager.OnCollectibleCollectedAction -= UpdateCollectibleCountDisplay;

        if ( _showRatingScaleUpSequence.IsActive ( ) ) 
            _showRatingScaleUpSequence.Kill ( );

        if ( _showRatingScaleDownSequence.IsActive ( ) ) 
            _showRatingScaleDownSequence.Kill ( );
        
        if ( _timeDisplayCoroutine != null ) 
            StopCoroutine ( _timeDisplayCoroutine );
        
        if ( _currentTrackNameLabelMarqueeCoroutine != null ) 
            StopCoroutine ( _currentTrackNameLabelMarqueeCoroutine );
        
        if ( _currentTrackProgressDisplayCoroutine != null ) 
            StopCoroutine ( _currentTrackProgressDisplayCoroutine );
    }

    private void Start ( ) 
    {
        AdjustSensitivityAction?.Invoke ( sensitivitySlider.value );
        AdjustMusicVolumeAction?.Invoke ( musicVolumeSlider.value );
        AdjustSFXVolumeAction?.Invoke ( sfxVolumeSlider.value );
        
        musicProgressSlider.value = 0;
    }

    private void OnGameStart ( ) 
    {
        previousTrackButton.interactable = true;
        nextTrackButton.interactable = true;
    }

    private void OnGameEnd ( ) 
    {
        _startButtonState = StartButtonState.Start;
        startButtonIcon.sprite = restartIcon;
        startButtonIcon.color = Constants.ThemeGreenColor;
        
        previousTrackButton.interactable = false;
        nextTrackButton.interactable = false;

        TogglePause ( true );

        _currentRunTimeInSeconds = 0;
    }

    
    #region Pause

    private void TogglePause ( bool toggleFlag ) 
    {
        pauseBlockPanel.SetActive ( toggleFlag );

        if ( toggleFlag ) 
        {
            if ( _timeDisplayCoroutine != null ) 
                StopCoroutine ( _timeDisplayCoroutine );
        }
        else 
        {
            _timeDisplayCoroutine = RunTimer ( );
            StartCoroutine ( _timeDisplayCoroutine );
        }
    }

    #endregion


    #region Current Run Timer

    private IEnumerator RunTimer ( ) 
    {
        while ( true ) 
        {
            yield return new WaitForSeconds ( 1 );

            ++_currentRunTimeInSeconds;
            UpdateTimerDisplay ( );
        }
    }

    private void UpdateTimerDisplay ( ) 
    {
        var hours = ( int ) _currentRunTimeInSeconds / 3600;
        var minutes = ( int ) ( _currentRunTimeInSeconds / 60 ) % 60;
        var seconds = ( int ) _currentRunTimeInSeconds % 60;

        timeLabel.text =    ( hours != 0 ? ( ( hours < 10 ? "0" : "" ) + hours + ":" ) : "" ) + 
                            ( minutes < 10 ? "0" : "" ) + minutes + ":" + 
                            ( seconds < 10 ? "0" : "" ) + seconds;
    }

    #endregion


    #region Current Track

    private void PlayCurrentTrack ( Music track ) 
    {
        foreach ( var trackTitleLabel in currentTrackTitleLabels ) 
            trackTitleLabel.text = track.Name;
        DOVirtual.DelayedCall ( 0.25f, ( ) => LayoutRebuilder.ForceRebuildLayoutImmediate ( currentTrackTitleContentTransform ) );

        if ( _currentTrackNameLabelMarqueeCoroutine != null ) 
            StopCoroutine ( _currentTrackNameLabelMarqueeCoroutine );
        _currentTrackNameLabelMarqueeCoroutine = RunCurrentTrackLabelMarquee ( );
        StartCoroutine ( _currentTrackNameLabelMarqueeCoroutine );
        
        musicProgressSlider.maxValue = track.Source.clip.length;

        _isHeartFilled = track.IsLiked; 
        heartButton.interactable = true;
        heartButtonAnimator.SetTrigger ( _isHeartFilled ? Constants.ANIM_TRIGGER_HeartButtonFill : Constants.ANIM_TRIGGER_HeartButtonEmpty );

        _currentTrackProgressDisplayCoroutine = RunCurrentTrackProgressTimer ( track.Pitch );
        StartCoroutine ( _currentTrackProgressDisplayCoroutine );
    }

    private void StopCurrentTrack ( ) 
    {
        foreach ( var trackTitleLabel in currentTrackTitleLabels ) 
            trackTitleLabel.text = "";
        
        musicProgressSlider.value = 0;

        _isHeartFilled = false;
        heartButton.interactable = false;

        _currentTrackTimeInSeconds = 0;
        StopCoroutine ( _currentTrackProgressDisplayCoroutine );
    }

    private IEnumerator RunCurrentTrackProgressTimer ( float trackPitch ) 
    {
        var incrementTimeInSeconds = 0.01f;

        while ( true ) 
        {
            yield return new WaitForSeconds ( incrementTimeInSeconds );

            _currentTrackTimeInSeconds += incrementTimeInSeconds * trackPitch;
            musicProgressSlider.value = _currentTrackTimeInSeconds;
        }
    }

    private IEnumerator RunCurrentTrackLabelMarquee ( ) 
    {
        if ( !currentTrackTitleLabels.Any ( ) ) 
            yield break;

        yield return new WaitForEndOfFrame ( );

        currentTrackTitleContentTransform.anchoredPosition = Vector3.zero;

        yield return new WaitForSeconds ( 2f );
        
        var minScrolledPositionX = -currentTrackTitleLabels [ 0 ].GetComponent<RectTransform> ( ).rect.width - currentTrackTitleContentLayoutGroup.spacing;
        var maxScrolledPositionX = currentTrackTitleScrollRect.GetComponent<RectTransform> ( ).rect.width;

        while ( true ) 
        {
            if ( currentTrackTitleContentTransform.anchoredPosition.x <= minScrolledPositionX ) 
                currentTrackTitleContentTransform.anchoredPosition = Vector3.zero;
            else if ( currentTrackTitleContentTransform.anchoredPosition.x > maxScrolledPositionX ) 
                currentTrackTitleContentTransform.anchoredPosition = new Vector3 ( maxScrolledPositionX, 0 );

            currentTrackTitleScrollRect.horizontalNormalizedPosition += 0.0001f;

            yield return null;
        }
    }

    #endregion


    #region Rating

    private void ShowRating ( float NPESpeedScale, float NPESpeedMaxScale, int collectibleCollectedCount, int collectibleSpawnCount ) 
    {
        ratingStarsPanel.SetActive ( true );

        var rating = 0.0f;
        rating += 4f * ( ( NPESpeedScale - 1.0f ) / ( NPESpeedMaxScale - 1.0f ) );
        rating += 1f * ( collectibleSpawnCount == 0 ? 0 : ( ( float ) collectibleCollectedCount / collectibleSpawnCount ) );

        _showRatingScaleUpSequence = DOTween.Sequence ( );
        _showRatingScaleDownSequence = DOTween.Sequence ( );
        
        for ( int i = 0; i < rating; i++ ) 
        {
            ratingStars [ i ].sprite = ratingStarFilled;

            _showRatingScaleUpSequence.Append ( 
                ratingStars [ i ].transform.DOScale ( 1.5f, 0.5f ) 
                    .SetUpdate ( true ) 
            );

            _showRatingScaleDownSequence.Join ( 
                ratingStars [ i ].transform.DOScale ( 1f, 0.25f ) 
                .SetUpdate ( true ) 
            );
        }
        
        _showRatingScaleUpSequence.Play ( )
            .SetUpdate ( true );
        
         _showRatingScaleDownSequence.Play ( )
            .SetUpdate ( true )
            .SetDelay ( 1.0f );
    }

    private void HideRating ( ) 
    {
        ratingStarsPanel.SetActive ( false );

        for ( int i = 0; i < ratingStars.Length; i++ ) 
        {
            ratingStars [ i ].transform.localScale = Vector3.one;
            ratingStars [ i ].sprite = ratingStarEmpty;
        }
    }

    #endregion


    #region Collectible

    private void UpdateCollectibleCountDisplay ( int collectedCollectibleCount, int totalCollectibleCount ) 
    {
        collectibleCountLabel.text = $"{collectedCollectibleCount}/{totalCollectibleCount}";
    }

    #endregion


    #endregion
}