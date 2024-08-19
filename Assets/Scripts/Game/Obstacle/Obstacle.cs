using System;
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

    #endregion


    #region Methods

    public void Fire ( float gapWidth, bool isTop, float moveSpeed, float lifeTime ) 
    {
        transform.SetLocalPositionAndRotation ( 
            new Vector3 ( 0, isTop ? gapWidth : -gapWidth, 0 ), 
            Quaternion.Euler ( 0, 0, isTop ? 180 : 0 )
        );

        playerDetector.Initialize ( GetStingerType ( ) );
        
        rb.velocity = new Vector2 ( -moveSpeed, 0f );
        Destroy ( gameObject, lifeTime );

        return;


        SoundType GetStingerType ( ) 
        {
            var thresholdLower = Constants.ObstacleGapWidthRange.x + ( ( Constants.ObstacleGapWidthRange.y - Constants.ObstacleGapWidthRange.x ) / 3 );
            var thresholdUpper = Constants.ObstacleGapWidthRange.x + ( ( Constants.ObstacleGapWidthRange.y - Constants.ObstacleGapWidthRange.x ) * 2 / 3 );

            if ( gapWidth >= Constants.ObstacleGapWidthRange.x && gapWidth < thresholdLower ) 
                return SoundType.Stinger1;
            else if ( gapWidth >= thresholdLower && gapWidth < thresholdUpper ) 
                return SoundType.Stinger2;
            else 
                return SoundType.Stinger3;
        }
    }

    private void OnTriggerEnter2D ( Collider2D other ) 
    {
        if ( other.GetComponent<Player> ( ) != null ) 
            OnPlayerHitAction?.Invoke ( );
    }

    #endregion
}
