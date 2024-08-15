using UnityEngine;

[ System.Serializable ] 
public class Sound : Audio
{
    #region Fields

    [ SerializeField ] private SoundType type;
    public SoundType Type => type;

    #endregion
}