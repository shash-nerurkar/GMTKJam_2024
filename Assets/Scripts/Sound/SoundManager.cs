using System;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Actions

    public static event Action<AudioSource> VisualizeTrackAction;

    public static event Action StopVisualizingTrackAction;

    #endregion


    #region Fields

    [ SerializeField ] private Sound [ ] sounds;

    [ SerializeField ] private Music [ ] musics;

    public static SoundManager Instance { get { return _instance; } }
    private static SoundManager _instance;

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

        GameManager.OnGameStartAction += PlayMusic;
        GameManager.OnGameEndAction += StopMusic;

        HUDManager.OnPausePressedAction += PauseMusic;
        HUDManager.OnResumePressedAction += ResumeMusic;
        HUDManager.AdjustVolumeAAction += AdjustVolume;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction -= PlayMusic;
        GameManager.OnGameEndAction -= StopMusic;

        HUDManager.OnPausePressedAction -= PauseMusic;
        HUDManager.OnResumePressedAction -= ResumeMusic;
        HUDManager.AdjustVolumeAAction -= AdjustVolume;
    }

    private void AdjustVolume ( float fraction ) 
    {
        foreach ( var sound in sounds ) 
            sound.AdjustVolume ( fraction );
            
        foreach ( var music in musics ) 
            music.AdjustVolume ( fraction );
    }


    #region Sound

    public void Play ( SoundType type ) 
    {
        foreach ( var sound in sounds.Where ( s => s.IsPlaying ) ) 
            sound.Stop ( );

        Array.Find ( sounds, s => s.Type == type )?.Play ( );
    }

    public void Stop ( SoundType type ) => Array.Find ( sounds, s => s.Type == type )?.Stop ( );

    #endregion

    
    #region Music

    private void PlayMusic ( ) 
    {
        Play ( MusicType.TrackDnB );
        
        VisualizeTrackAction?.Invoke ( GetTrackSource ( MusicType.TrackDnB ) );
    }

    private void StopMusic ( ) 
    {
        Stop ( MusicType.TrackDnB );

        foreach ( var sound in sounds.Where ( s => s.IsPlaying ) ) 
            sound.Stop ( );

        StopVisualizingTrackAction?.Invoke ( );
    }

    private void PauseMusic ( ) 
    {
        Pause ( MusicType.TrackDnB );
    }

    private void ResumeMusic ( ) 
    {
        Resume ( MusicType.TrackDnB );
    }

    private void Play ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.FadeInPlay ( );

    private void Resume ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.Play ( );

    private void Stop ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.Stop ( );

    private void Pause ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.Pause ( );

    private AudioSource GetTrackSource ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.Source;

    #endregion


    #endregion
}