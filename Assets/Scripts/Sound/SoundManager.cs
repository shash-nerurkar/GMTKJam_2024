using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Actions

    public static event Action<Music> OnPlayTrackAction;

    public static event Action OnStopTrackAction;

    #endregion


    #region Fields

    [ SerializeField ] private Sound [ ] sounds;

    [ SerializeField ] private Music [ ] musics;

    [ SerializeField ] private MusicType [ ] tracksToPlay;

    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    private int _currentTrackIndex = 0;

    private bool _isLooping;

    private IEnumerator _playNextTrackCoroutine;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        if ( _instance != null && _instance != this ) 
            Destroy ( gameObject );
        else 
            _instance = this;

        sounds.ToList ( ).ForEach ( sound => sound.Init ( gameObject.AddComponent<AudioSource> ( ) ) );

        musics.ToList ( ).ForEach ( music => music.Init ( gameObject.AddComponent<AudioSource> ( ) ) );

        GameManager.OnGameStartAction += OnGameStart;
        GameManager.OnGameEndAction += OnGameEnd;

        HUDManager.OnPausePressedAction += OnPauseButtonPressed;
        HUDManager.OnResumePressedAction += OnResumeButtonPressed;

        HUDManager.OnPreviousTrackPressedAction += OnPreviousTrackButtonPressed;
        HUDManager.OnNextTrackPressedAction += OnNextTrackButtonPressed;

        HUDManager.OnHeartTrackToggledAction += SetCurrentTrackLikeStatus;
        HUDManager.OnLoopTrackToggledAction += SetLoopStatus;
        
        HUDManager.AdjustMusicVolumeAction += AdjustMusicVolume;
        HUDManager.AdjustSFXVolumeAction += AdjustSFXVolume;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction -= OnGameStart;
        GameManager.OnGameEndAction -= OnGameEnd;

        HUDManager.OnPausePressedAction -= OnPauseButtonPressed;
        HUDManager.OnResumePressedAction -= OnResumeButtonPressed;
        HUDManager.OnPreviousTrackPressedAction -= OnPreviousTrackButtonPressed;
        HUDManager.OnNextTrackPressedAction -= OnNextTrackButtonPressed;

        HUDManager.OnHeartTrackToggledAction -= SetCurrentTrackLikeStatus;
        HUDManager.OnLoopTrackToggledAction -= SetLoopStatus;

        HUDManager.AdjustMusicVolumeAction -= AdjustMusicVolume;
        HUDManager.AdjustSFXVolumeAction -= AdjustSFXVolume;
        
        if ( _playNextTrackCoroutine != null ) 
            StopCoroutine ( _playNextTrackCoroutine );
    }

    private void OnGameStart ( ) => PlayMusic ( );

    private void OnGameEnd ( ) => StopMusic ( );

    private void OnPauseButtonPressed ( ) => musics.ToList ( ).ForEach ( music => music.Source.pitch = 0 );

    private void OnResumeButtonPressed ( ) => DOVirtual.DelayedCall ( 0.25f, ( ) => musics.ToList ( ).ForEach ( music => music.Source.pitch = 1 ) );

    private void OnPreviousTrackButtonPressed ( ) 
    {
        if ( _playNextTrackCoroutine != null ) 
            StopCoroutine ( _playNextTrackCoroutine );
        
        PlayPreviousTrack ( );
    }

    private void OnNextTrackButtonPressed ( ) 
    {
        if ( _playNextTrackCoroutine != null ) 
            StopCoroutine ( _playNextTrackCoroutine );
        
        _playNextTrackCoroutine = PlayNextTrack ( overrideLooping: true );
        StartCoroutine ( _playNextTrackCoroutine );
    }

    private void SetCurrentTrackLikeStatus ( bool likeStatus ) => Array.Find ( musics, m => m.Type == tracksToPlay [ _currentTrackIndex ] ).SetLikeStatus ( likeStatus );

    private void SetLoopStatus ( bool loopStatus ) => _isLooping = loopStatus;

    private void AdjustMusicVolume ( float fraction ) => musics.ToList ( ).ForEach ( music => music.AdjustVolumeScale ( fraction ) );

    private void AdjustSFXVolume ( float fraction ) => sounds.ToList ( ).ForEach ( sound => sound.AdjustVolumeScale ( fraction ) );


    #region Sound

    public void Play ( SoundType type, bool stopOthers = false ) 
    {
        if ( stopOthers ) 
            foreach ( var sound in sounds.Where ( s => s.IsPlaying ) ) 
                sound.Stop ( );

        Array.Find ( sounds, s => s.Type == type )?.Play ( );
    }

    public void Stop ( SoundType type ) => Array.Find ( sounds, s => s.Type == type )?.Stop ( );

    #endregion

    
    #region Music

    private void PlayMusic ( float fadeInTimeInSeconds = 0 ) 
    {
        var currentTrack = Array.Find ( musics, m => m.Type == tracksToPlay [ _currentTrackIndex ] );

        currentTrack.Play ( fadeInTimeInSeconds );

        OnPlayTrackAction?.Invoke ( currentTrack );

        _playNextTrackCoroutine = PlayNextTrack ( currentTrack.Source.clip.length / currentTrack.Pitch, 2f / currentTrack.Pitch );
        StartCoroutine ( _playNextTrackCoroutine );
    }

    private void StopMusic ( float fadeOutTimeInSeconds = 0 ) 
    {
        foreach ( var sound in sounds.Where ( s => s.IsPlaying && s.Type != SoundType.PlayerHitObstacle ) ) 
            sound.Stop ( );
            
        var currentTrack = Array.Find ( musics, m => m.Type == tracksToPlay [ _currentTrackIndex ] );

        currentTrack.Stop ( fadeOutTimeInSeconds, ( ) => OnStopTrackAction?.Invoke ( ) );
    }

    private void PlayPreviousTrack ( ) 
    { 
        var currentTrack = Array.Find ( musics, m => m.Type == tracksToPlay [ _currentTrackIndex ] );
        
        var newTrackIndex = currentTrack.Source.time > 2f 
                                    ? _currentTrackIndex 
                                    : ( _currentTrackIndex == 0 ? tracksToPlay.Length : _currentTrackIndex ) - 1;
        
        StopMusic ( );
        
        _currentTrackIndex = newTrackIndex;
        
        PlayMusic ( );
    }

    private IEnumerator PlayNextTrack ( float delayInSeconds = 0, float fadeTimeInSeconds = 0, bool overrideLooping = false ) 
    { 
        yield return new WaitForSeconds ( Mathf.Max ( 0, delayInSeconds - fadeTimeInSeconds ) );

        int newTrackIndex;
        if ( _isLooping && !overrideLooping ) 
        {
            newTrackIndex = _currentTrackIndex;
            fadeTimeInSeconds = 0;
        }
        else 
        {
            newTrackIndex = ( _currentTrackIndex == tracksToPlay.Length - 1 ? -1 : _currentTrackIndex ) + 1;
        }

        StopMusic ( fadeTimeInSeconds );

        yield return new WaitForSeconds ( fadeTimeInSeconds );
        
        _currentTrackIndex = newTrackIndex;
        
        PlayMusic ( fadeTimeInSeconds );
    }

    #endregion


    #endregion
}