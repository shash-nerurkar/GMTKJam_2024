using UnityEngine;

public class ForegroundUIManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private Transition transition;

    #endregion

    
    #region Methods

    private void Awake ( ) 
    {

        GameManager.ShowTransitionAction += transition.FadeIn;
    }
    
    private void OnDestroy ( ) 
    {
        GameManager.ShowTransitionAction -= transition.FadeIn;
    }

    #endregion
}
