using System;
using System.Collections.ObjectModel;
using UnityEngine;


[ CreateAssetMenu ( fileName = "DialogueSequenceData", menuName = "Game/Dialogue Sequence Data", order = 1 ) ]
public class DialogueSequenceData : ScriptableObject 
{
    #region Serialized Fields
    
    [ SerializeField ] private DialogueData [ ] openingDialogues;
    
    [ SerializeField ] private DialogueData [ ] closingDialogues;
    
    #endregion

    
    #region Public Fields
    
    public ReadOnlyCollection<DialogueData> OpeningDialogues => Array.AsReadOnly ( openingDialogues );
    
    public ReadOnlyCollection<DialogueData> ClosingDialogues => Array.AsReadOnly ( closingDialogues );
    
    #endregion
}
