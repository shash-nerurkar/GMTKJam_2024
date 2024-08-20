using System;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    #region Actions

    public static event Action OnPlayerHitAction;

    #endregion


    #region Fields

    [ SerializeField ] private SpriteRenderer spriteRenderer;

    [ SerializeField ] private Rigidbody2D rb;

    #endregion


    #region Methods

    public void Initialize ( float positionY, float moveSpeed, float lifeTime ) 
    {
        transform.SetLocalPositionAndRotation ( 
            new Vector3 ( 0, positionY ), 
            Quaternion.Euler ( 0, 0, 0 )
        );
        
        rb.velocity = new Vector2 ( -moveSpeed, 0f );
        Destroy ( gameObject, lifeTime );
    }

    private void OnTriggerEnter2D ( Collider2D other ) 
    {
        if ( other.GetComponent<Player> ( ) != null ) 
            OnPlayerHitAction?.Invoke ( );
        
        Destroy ( gameObject );
    }

    #endregion
}
