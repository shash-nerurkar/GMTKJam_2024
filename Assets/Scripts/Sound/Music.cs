using UnityEngine;

[ System.Serializable ] 
public class Music : Audio
{
    #region Fields

    [ SerializeField ] private MusicType type;
    public MusicType Type => type;

    [ SerializeField ] private string name;
    public string Name => name;

    [ SerializeField ] private bool isLiked;
    public bool IsLiked => isLiked;

    public bool SetLikeStatus ( bool isLiked ) => this.isLiked = isLiked;

    #endregion
}