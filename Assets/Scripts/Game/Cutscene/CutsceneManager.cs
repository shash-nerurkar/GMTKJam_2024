using System;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    #region Actions

    public static event Action<CutsceneSequenceData> StartCutsceneAction;

    #endregion


    #region Fields

    [ SerializeField ] private SerializedDictionary<Cutscene, CutsceneSequenceData> cutsceneSequenceDatas;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnCutsceneStartAction += StartNewCutscene;
    }
    
    private void OnDestroy ( ) 
    {
        GameManager.OnCutsceneStartAction -= StartNewCutscene;
    }
    
    public void StartNewCutscene ( Cutscene cutscene ) 
    {
        StartCutsceneAction?.Invoke ( cutsceneSequenceDatas.ToDictionary ( ) [ cutscene ] );
    }

    #endregion
}
