using System;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
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
    }

    private void OnDestroy ( ) 
    {

    }


    #region Sound

    public void Play ( SoundType type ) => Array.Find ( sounds, s => s.Type == type )?.Play ( );

    public void Stop ( SoundType type ) => Array.Find ( sounds, s => s.Type == type )?.Stop ( );

    #endregion

    
    #region Music

    private void BeginMainMenuMusic ( ) => Play ( MusicType.MainMenu );

    private void StopMainMenuMusic ( ) => Stop ( MusicType.MainMenu );

    private void BeginCutsceneMusic ( ) => Play ( MusicType.Cutscene );

    private void StopCutsceneMusic ( ) => Stop ( MusicType.Cutscene );

    private void BeginInGameMusic ( ) => Play ( MusicType.InGame );

    private void StopInGameMusic ( ) => Stop ( MusicType.InGame );

    private void Play ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.FadeInPlay ( );

    private void Stop ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.FadeOutStop ( );

    #endregion


    #endregion
}