using System;
using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
    #region Transition

    public static readonly Color TransitionFadeInColor = new ( 0f, 0f, 0f, 1f );

    public static readonly Color TransitionFadeOutColor = new ( 0f, 0f, 0f, 0f );

    #endregion


    #region In-Game

    public static readonly Vector2 InGameViewportVerticalRange = new ( -2.75f, 4.75f );

    #endregion


    #region Obstacle sounds

    public static readonly Dictionary<float, SoundType> GapWidthToStinger = new ( ) { 
        { 1.5f, SoundType.Stinger1 },
        { 2.5f, SoundType.Stinger1 },
        { 3.5f, SoundType.Stinger1 },
        { 4.5f, SoundType.Stinger2 },
        { 5.5f, SoundType.Stinger2 },
        { 6.5f, SoundType.Stinger3 },
        { 7.5f, SoundType.Stinger3 },
    };

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