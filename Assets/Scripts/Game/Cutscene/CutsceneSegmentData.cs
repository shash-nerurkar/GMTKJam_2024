using System;
using UnityEngine;

[ Serializable ]
public class CutsceneSegmentData 
{
    [ SerializeField ] private Sprite image;
    
    [ SerializeField ] private string text;

    public Sprite Image => image;

    public string Text => text;
}
