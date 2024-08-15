using UnityEngine;

public class ForegroundUIManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private Transition transition;

    [ SerializeField ] private GameObject WIPDisplayPrefab;

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

    // private void Start ( ) => Instantiate ( WIPDisplayPrefab, transform );

    #endregion
}
