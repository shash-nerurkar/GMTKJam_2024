using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private Image transitionImage;

    #endregion


    #region Methods

    public void FadeIn ( float fadeInSpeedInSeconds, float fadeOutSpeedInSeconds, Action onFadeInCompleteAction ) 
    {
        transitionImage.enabled = true;

        transitionImage.color = Constants.TransitionFadeOutColor;

        DOTween.To ( ( ) => transitionImage.color, x => transitionImage.color = x, Constants.TransitionFadeInColor, fadeInSpeedInSeconds ) 
            .SetUpdate ( true ) 
            .OnComplete ( ( ) => 
            {
                onFadeInCompleteAction ( );
                
                FadeOut ( fadeOutSpeedInSeconds );
            } );
    }

    public void FadeOut ( float speedInSeconds ) 
    {
        transitionImage.color = Constants.TransitionFadeInColor;

        DOTween.To ( ( ) => transitionImage.color, x => transitionImage.color = x, Constants.TransitionFadeOutColor, speedInSeconds ) 
            .SetUpdate ( true ) 
            .SetDelay ( 1 ) 
            .OnComplete ( ( ) => 
            {
                transitionImage.enabled = false;
            } );
    }

    #endregion
}
