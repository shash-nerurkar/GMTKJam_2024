using UnityEngine;

[ System.Serializable ] 
public class Music : Audio
{
    #region Fields

    [ SerializeField ] private MusicType type;
    public MusicType Type => type;

    #endregion
}