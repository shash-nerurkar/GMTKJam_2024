using UnityEngine;

public class InGamePanel : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private DialogueBox dialogueBox;
    
    #endregion

    
    #region Methods

    private void Awake ( ) 
    {
        DialogueManager.StartDialogueSequenceAction += dialogueBox.StartNewDialogueSequence;
    }
    
    private void OnDestroy ( ) 
    {
        DialogueManager.StartDialogueSequenceAction -= dialogueBox.StartNewDialogueSequence;
    }

    #endregion
}
