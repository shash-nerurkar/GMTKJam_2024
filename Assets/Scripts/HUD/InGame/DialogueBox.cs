using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    #region Actions

    public static event Action<bool> OnSequenceCompleteAction;

    #endregion


    #region Fields

    [ SerializeField ] private Image speakerPortraitLeft;

    [ SerializeField ] private Image speakerPortraitRight;

    [ SerializeField ] private TMPro.TextMeshProUGUI dialogueLabel;

    private ReadOnlyCollection<DialogueData> _currentDialoguesData;

    private int _currentDialogueIndex;

    private bool _isOpeningSequence;

    #endregion


    #region Methods

    public void StartNewDialogueSequence ( DialogueSequenceData sequenceData, bool isOpeningSequence ) 
    {
        var dialogueDataToShow = isOpeningSequence ? sequenceData.OpeningDialogues : sequenceData.ClosingDialogues;

        if ( !dialogueDataToShow.Any ( ) ) 
        {
            ClearDialogueBox ( );
            OnSequenceCompleteAction?.Invoke ( isOpeningSequence );
        }
        else 
        {
            _currentDialoguesData = dialogueDataToShow;
            _currentDialogueIndex = 0;
            _isOpeningSequence = isOpeningSequence;
            ShowCurrentDialogue ( );
        }
    }

    public bool ShowNextDialogue ( ) 
    {
        if ( _currentDialoguesData == null ) 
            return false;

        if ( _currentDialogueIndex < _currentDialoguesData.Count - 1 ) 
        {
            ++_currentDialogueIndex;
            ShowCurrentDialogue ( );
        }
        else 
        {
            _currentDialoguesData = null;
            _currentDialogueIndex = -1;
            ClearDialogueBox ( );
            OnSequenceCompleteAction?.Invoke ( _isOpeningSequence );
        }

        return true;
    }

    private void ShowCurrentDialogue ( ) 
    {
        var currentDialogueData = _currentDialoguesData [ _currentDialogueIndex ];
        
        // if ( currentDialogueData.Speaker.Equals ( InGameCharacter.Player ) ) 
        // {
        //     speakerPortraitLeft.enabled = true;
        //     speakerPortraitLeft.sprite = CharacterPortraits [ currentDialogueData.Speaker ];

        //     speakerPortraitRight.enabled = false;
        //     speakerPortraitRight.sprite = null;
        // }
        // else 
        // {
        //     speakerPortraitLeft.enabled = false;
        //     speakerPortraitLeft.sprite = null;

        //     speakerPortraitRight.enabled = true;
        //     speakerPortraitRight.sprite = CharacterPortraits [ currentDialogueData.Speaker ];
        // }
        
        // dialogueLabel.text = CharacterNames [ currentDialogueData.Speaker ] + ": " + currentDialogueData.Text;
    }

    private void ClearDialogueBox ( ) 
    {
        speakerPortraitLeft.sprite = null;
        speakerPortraitLeft.enabled = false;
        
        speakerPortraitRight.sprite = null;
        speakerPortraitRight.enabled = false;

        dialogueLabel.text = "";
    }

    #endregion
}
