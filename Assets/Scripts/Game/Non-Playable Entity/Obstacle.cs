using System;
using DG.Tweening;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    #region Actions

    public static event Action OnPlayerHitAction;

    #endregion


    #region Fields

    [ SerializeField ] private SpriteRenderer spriteRenderer;

    [ SerializeField ] private Rigidbody2D rb;

    [ SerializeField ] private PlayerDetector playerDetector;

    [ SerializeField ] private GameObject flareEffect;
    
    private SoundType _stingerType;

    #endregion


    #region Methods

    public void Initialize ( float gapWidth, bool isTop, float moveSpeed, float lifeTime ) 
    {
        transform.SetLocalPositionAndRotation ( 
            new Vector3 ( 0, isTop ? gapWidth : -gapWidth, 0 ), 
            Quaternion.Euler ( 0, 0, isTop ? 180 : 0 )
        );

        _stingerType = Constants.GetStingerTypeByGapWidth ( MusicType.TrackDnB, gapWidth );
        
        rb.velocity = new Vector2 ( -moveSpeed, 0f );
        Destroy ( gameObject, lifeTime );

        playerDetector.PlayerDetectedAction += OnPlayerDetected;
    }

    private void OnTriggerEnter2D ( Collider2D other ) 
    {
        if ( other.GetComponent<Player> ( ) != null ) 
            OnPlayerHitAction?.Invoke ( );
    }

    private void OnPlayerDetected ( Player player ) 
    {
        SoundManager.Instance.Play ( _stingerType );

        flareEffect.SetActive ( true );
        DOVirtual.DelayedCall ( 2f, ( ) => { 
            if ( flareEffect != null ) 
                flareEffect.SetActive ( false );
        } );
    }

    #endregion
}
