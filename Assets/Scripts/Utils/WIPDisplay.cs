using System.Collections;
using TMPro;
using UnityEngine;

public class WIPDisplay : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private TextMeshProUGUI label;

    #endregion


    #region Methods

    private void Start ( ) => StartCoroutine ( LoopLabelText ( ) );

    IEnumerator LoopLabelText ( ) 
    {
        if ( label == null ) 
            yield break;

        while ( true ) 
        {
            label.text = " \u0009" + "Wurk In Progress" + ( Mathf.Floor ( Time.time ) % 2 == 0 ? "|" : "") + " \u0009" +
                            "\n<i><size=60%>(unless the jam hasn't begun, in which case, nothing to see OR in-progress)</size></i>";

            yield return new WaitForSeconds ( 1 );
        }
    }

    #endregion
}
