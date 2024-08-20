using System;
using DG.Tweening;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    #region Actions

    public static event Action OnPlayerHitAction;

    #endregion


    #region Fields

    [ SerializeField ] private SpriteRenderer spriteRenderer;

    [ SerializeField ] private Rigidbody2D rb;

    [ SerializeField ] private Animator animator;

    private Tween _shakeTween;

    #endregion


    #region Methods

    private void OnDestroy ( ) 
    {
        if ( _shakeTween.IsActive (  ) ) 
            _shakeTween.Kill ( );
    }

    public void Initialize ( float positionY, float moveSpeed, float lifeTime ) 
    {
        transform.SetLocalPositionAndRotation ( 
            new Vector3 ( 0, positionY ), 
            Quaternion.Euler ( 0, 0, 0 )
        );

        // _shakeTween = spriteRenderer.transform.DOShakePosition ( 
        //     duration: 1f,
        //     strength: new Vector2 ( 0.15f, 0.15f ),
        //     vibrato: 3,
        //     randomness: 90f,
        //     fadeOut: false
        // ).SetLoops ( -1, LoopType.Restart );

        rb.velocity = new Vector2 ( -moveSpeed, 0f );
        Destroy ( gameObject, lifeTime );
    }

    public void OnCollectedAnimationComplete ( ) => Destroy ( gameObject );

    private void OnTriggerEnter2D ( Collider2D other ) 
    {
        if ( other.GetComponent<Player> ( ) != null ) 
            OnPlayerHitAction?.Invoke ( );
        
        animator.SetBool ( "IsCollected", true );
    }

    #endregion
}
