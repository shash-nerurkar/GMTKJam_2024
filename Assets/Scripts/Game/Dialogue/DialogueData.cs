using System;
using UnityEngine;

[ Serializable ]
public class DialogueData 
{
    [ SerializeField ] private InGameCharacter speaker;
    
    [ SerializeField ] private string text;

    public InGameCharacter Speaker => speaker;

    public string Text => text;
}
