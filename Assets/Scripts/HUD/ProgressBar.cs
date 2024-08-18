using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    #region Fields
    
    [ SerializeField ] private Slider slider;

    #endregion


    #region Methods

    public void InitProgressBar ( ) 
    {
        slider.value = 0;
    }

    public void UpdateProgressBar ( int newValue ) 
    {
        var startValue = slider.value;
        DOTween.To ( ( ) => startValue, x => slider.value = x, Mathf.Clamp ( newValue, slider.minValue, slider.maxValue ), 0.5f )
            .OnUpdate ( ( ) => { } );
    }
    
    #endregion
}
