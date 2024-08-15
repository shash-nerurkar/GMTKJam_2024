using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    #region Actions

    public static event Action<DialogueSequenceData, bool> StartDialogueSequenceAction;

    #endregion


    #region Fields

    [ SerializeField ] private SerializedDictionary<Level, DialogueSequenceData> dialogueSequenceDatas;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnLevelStartAction += StartOpeningDialogueSequence;
    }
    
    private void OnDestroy ( ) 
    {
        GameManager.OnLevelStartAction -= StartOpeningDialogueSequence;
    }
    
    public void StartOpeningDialogueSequence ( Level level ) 
    {
        StartDialogueSequenceAction?.Invoke ( dialogueSequenceDatas.ToDictionary ( ) [ level ], true );
    }
    
    public void StartClosingDialogueSequence ( Level level ) 
    {
        StartDialogueSequenceAction?.Invoke ( dialogueSequenceDatas.ToDictionary ( ) [ level ], false );
    }

    #endregion
}
