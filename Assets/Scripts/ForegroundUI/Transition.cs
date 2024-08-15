using System;
using System.Threading.Tasks;
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
            .OnComplete ( async ( ) => 
            {
                onFadeInCompleteAction ( );

                await Task.Delay ( 1000 );
                
                FadeOut ( fadeOutSpeedInSeconds );
            } );
    }

    public void FadeOut ( float speedInSeconds ) 
    {
        transitionImage.color = Constants.TransitionFadeInColor;

        DOTween.To ( ( ) => transitionImage.color, x => transitionImage.color = x, Constants.TransitionFadeOutColor, speedInSeconds )
            .OnComplete ( ( ) => 
            {
                transitionImage.enabled = false;
            } );
    }

    #endregion
}
