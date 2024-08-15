using DG.Tweening;
using UnityEngine;


public abstract class Audio 
{
    #region Fields

    [ SerializeField ] private AudioClip clip;

    [ Range ( 0f, 1f ) ] [ SerializeField ] private float volume = 1f;

    [ Range ( .1f, 3f ) ] [ SerializeField ] private float pitch = 1f;

    [ SerializeField ] private bool isLoop;
    
    private AudioSource source;

    #endregion


    #region Methods

    public void Init ( AudioSource source ) 
    {
        this.source = source;

        SetSource ( );
    }

    public void Play ( ) 
    {
        SetSource ( );

        source.Play ( );
    }

    public void Stop ( ) 
    {
        source.Stop ( );
    }

    public void FadeInPlay ( ) 
    {
        Play ( );

        DOTween.To ( ( ) => 0, x => source.volume = x, volume, 1.5f )
            .SetEase ( Ease.InQuad )
            .OnComplete ( ( ) => { } );
    }

    public void FadeOutStop ( ) 
    {
        DOTween.To ( ( ) => volume, x => source.volume = x, 0, 1.5f )
            .SetEase ( Ease.OutQuad )
            .OnComplete ( ( ) => Stop ( ) );
    }

    private void SetSource ( ) 
    { 
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = isLoop;
    }

    #endregion
}