using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private TextMeshProUGUI label;

    #endregion


    #region Methods

    public void Initialize ( string amount, int textSize, Color textColor ) 
    {
        label.text = amount;
        label.fontSize = textSize;
        label.color = textColor;
        
        label.DOColor ( new Color ( label.color.r, label.color.g, label.color.b, 0 ), 1f ) 
            .SetEase ( Ease.InCirc );

        transform.DOScale ( new Vector3 ( 1.5f, 1.5f, 1.5f ), 0.25f );
        
        transform.DOMove ( transform.position + new Vector3 ( 0, 1 ), 1.5f ) 
            .SetEase ( Ease.InCirc ) 
            .OnComplete ( ( ) => Destroy ( gameObject ) );
    }

    #endregion
}
