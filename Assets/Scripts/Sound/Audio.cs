using System;
using DG.Tweening;
using UnityEngine;


public abstract class Audio 
{
    #region Fields

    [ SerializeField ] private AudioClip clip;

    [ Range ( 0f, 1f ) ] [ SerializeField ] private float volume = 1f;

    [ Range ( .1f, 3f ) ] [ SerializeField ] private float pitch = 1f;
    public float Pitch => pitch;

    [ SerializeField ] private bool isLoop;

    [ Range ( 0f, 1f ) ] private float _volumeScale = 1f;
    
    private AudioSource source;
    public AudioSource Source => source;

    public bool IsPlaying => source.isPlaying;

    #endregion


    #region Methods

    public void Init ( AudioSource source ) 
    {
        this.source = source;

        GetVolume ( );
        
        source.clip = clip;
        source.pitch = pitch;
        source.loop = isLoop;
    }

    public void Play ( float fadeInTimeInSeconds = 0, Action onFadeCompleteAction = null ) 
    {
        source.Play ( );

        if ( fadeInTimeInSeconds > 0 ) 
            DOTween.To ( ( ) => 0, x => source.volume = x, GetVolume ( ), fadeInTimeInSeconds )
                .SetEase ( Ease.InQuad )
                .OnComplete ( ( ) => { 
                    onFadeCompleteAction?.Invoke ( );
                } );
        else 
            onFadeCompleteAction?.Invoke ( );
    }

    public void Stop ( float fadeOutTimeInSeconds = 0, Action onFadeCompleteAction = null ) 
    {
        if ( fadeOutTimeInSeconds <= 0 ) 
        {
            source.Stop ( );
            onFadeCompleteAction?.Invoke ( );
            return;
        }
        
        DOTween.To ( ( ) => source.volume, x => source.volume = x, 0, fadeOutTimeInSeconds )
            .SetEase ( Ease.OutQuad )
            .OnComplete ( ( ) => { 
                onFadeCompleteAction?.Invoke ( );

                source.Stop ( );

                source.volume = GetVolume ( );
            } );
    }

    public void Pause ( ) => source.Pause ( );

    public void Resume ( ) => source.Play ( );

    public void AdjustVolumeScale ( float fraction ) 
    {
        _volumeScale = fraction;
        source.volume = volume * fraction;
    }

    private float GetVolume ( ) => volume * _volumeScale;

    #endregion
}