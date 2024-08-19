using System;
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

    public static readonly Vector2 InGameViewportVerticalRange = new ( -2.75f, 4.75f );

    public static bool IsPointYInsideInGameViewport ( float pointY ) => pointY > InGameViewportVerticalRange.x && pointY < InGameViewportVerticalRange.y;

    public static readonly Vector2 ObstacleGapWidthRange = new ( 1.5f, 7.5f );

    #endregion
}


[ Serializable ]
public enum SoundType
{
    UIClicked,
    Stinger1,
    Stinger2,
    Stinger3
}


[ Serializable ]
public enum MusicType
{
    InGame1Atmosphere,
    InGame1Beat,
}