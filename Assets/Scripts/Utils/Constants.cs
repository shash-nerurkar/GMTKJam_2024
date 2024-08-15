using System;
using UnityEngine;

public static class Constants 
{
    #region Transition

    public static readonly Color TransitionFadeInColor = new ( 0f, 0f, 0f, 1f );

    public static readonly Color TransitionFadeOutColor = new ( 0f, 0f, 0f, 0f );

    #endregion


    #region Progress Bar

    public static readonly Color ProgressBarNegativeColor = new ( 1f, 0f, 0f );

    public static readonly Color ProgressBarPositiveColor = new ( 0f, 1f, 0f );

    public static readonly Color ProgressBarNeutralColor = new ( 1f, 1f, 1f );

    #endregion
}


[ Serializable ]
public enum GameState 
{
    MainMenu,
    Cutscene,
    InGame
}


[ Serializable ]
public enum Cutscene 
{
    Pilot,
}


[ Serializable ]
public enum Level 
{
    
}


[ Serializable ]
public enum InGameCharacter 
{
    Player,
    Enemy
}


[ Serializable ]
public enum SoundType
{
    OnUIClicked,
    OnDialoguePopped,
    OnCutscenePopped
}


[ Serializable ]
public enum MusicType
{
    MainMenu,
    Cutscene,
    InGame
}