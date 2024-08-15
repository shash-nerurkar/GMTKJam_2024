using System;
using System.Collections.ObjectModel;
using UnityEngine;


[ CreateAssetMenu ( fileName = "CutsceneSequenceData", menuName = "Cutscene/Cutscene Sequence Data", order = 1 ) ]
public class CutsceneSequenceData : ScriptableObject 
{
    #region Serialized Fields
    
    [ SerializeField ] private CutsceneSegmentData [ ] segments;
    
    #endregion

    
    #region Public Fields
    
    public ReadOnlyCollection<CutsceneSegmentData> Segments => Array.AsReadOnly ( segments );
    
    #endregion
}
