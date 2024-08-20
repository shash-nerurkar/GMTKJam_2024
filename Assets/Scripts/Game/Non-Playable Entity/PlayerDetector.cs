using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    #region Fields

    public event Action<Player> PlayerDetectedAction;

    #endregion
    

    #region Methods

    private void OnTriggerEnter2D ( Collider2D other ) 
    {
        var player = other.GetComponent<Player> ( );

        if ( player != null ) 
            PlayerDetectedAction?.Invoke ( player );
    }

    #endregion
}
