using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Constants 
{
    #region Transition

    public static readonly Color TransitionFadeInColor = new ( 0f, 0f, 0f, 1f );

    public static readonly Color TransitionFadeOutColor = new ( 0f, 0f, 0f, 0f );

    #endregion


    #region Themes

    public static readonly Color ThemeOrangeColor = new ( 0.9450981f, 0.7490196f, 0.3764706f );

    public static readonly Color ThemeGreenColor = new ( 0.003921569f, 1f, 0.1019608f );

    #endregion
    

    #region In-Game

    public static readonly Vector2 InGameViewportVerticalRange = new ( -2.55f, 4.45f );

    public static bool IsPointYInGameViewport ( float pointY ) => pointY > InGameViewportVerticalRange.x && pointY < InGameViewportVerticalRange.y;

    public static readonly Vector2 ObstacleGapWidthRange = new ( 1.5f, 7.5f );

    #endregion


    #region Soundtrack Stingers

    private static readonly Dictionary<MusicType, SoundType [ ]> TrackToStingers = new Dictionary<MusicType, SoundType [ ]> ( ) {
        { MusicType.TrackAtmospheric, new SoundType [ ] { 
            SoundType.Stinger1Atmospheric, SoundType.Stinger2Atmospheric, SoundType.Stinger3Atmospheric 
        } },
        { MusicType.TrackDnB, new SoundType [ ] { 
            SoundType.Stinger1DnB, SoundType.Stinger2DnB, SoundType.Stinger3DnB,
            SoundType.Stinger4DnB, SoundType.Stinger5DnB, SoundType.Stinger6DnB,
            SoundType.Stinger7DnB, SoundType.Stinger8DnB, SoundType.Stinger9DnB, 
        } },
    };

    public static SoundType GetStingerTypeByGapWidth ( MusicType trackType, float gapWidth ) 
    {
        var stingers = TrackToStingers [ trackType ];

        var thresholds = new List<float> { ObstacleGapWidthRange.x };
        for ( var i = 1; i < stingers.Length; i++ ) 
            thresholds.Add ( ObstacleGapWidthRange.x + ( ( ObstacleGapWidthRange.y - ObstacleGapWidthRange.x ) * i / stingers.Length ) );
        thresholds.Add ( ObstacleGapWidthRange.y );

        for ( var i = 0; i < thresholds.Count - 1; i++ ) 
            if ( gapWidth >= thresholds [ i ] && gapWidth < thresholds [ i + 1 ] ) 
                return stingers [ i ];

        return stingers.First ( );
    }

    #endregion
}


[ Serializable ]
public enum SoundType
{
    UIClicked,

    Stinger1Atmospheric,
    Stinger2Atmospheric,
    Stinger3Atmospheric,
    
    Stinger1DnB,
    Stinger2DnB,
    Stinger3DnB,
    Stinger4DnB,
    Stinger5DnB,
    Stinger6DnB,
    Stinger7DnB,
    Stinger8DnB,
    Stinger9DnB,
}


[ Serializable ]
public enum MusicType
{
    TrackAtmospheric,
    TrackDnB,
}