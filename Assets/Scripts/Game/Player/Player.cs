using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private SpriteRenderer spriteRenderer;

    [ SerializeField ] private Transform peripheralsTopPivotTransform;

    [ SerializeField ] private Transform peripheralsBottomPivotTransform;

    [ Header ( "Indicators" ) ]

    [ SerializeField ] private GameObject edgeIndicatorTop;

    [ SerializeField ] private GameObject edgeIndicatorBottom;

    private Vector3 _basePosition;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        InputManager.ScaleSideSelectedAction += SetSideToScale;
        InputManager.ScaleSideUnselectedAction += RemoveSideToScale;

        InputManager.ScalePlayerAction += Scale;

        GameManager.OnGameStartAction += Initialize;

        _basePosition = transform.position;
    }

    private void OnDestroy ( ) 
    {
        InputManager.ScaleSideSelectedAction -= SetSideToScale;
        InputManager.ScaleSideUnselectedAction -= RemoveSideToScale;

        InputManager.ScalePlayerAction -= Scale;

        GameManager.OnGameStartAction -= Initialize;
    }

    private void Initialize ( ) 
    {
        transform.position = _basePosition;

        transform.localScale = Vector3.one;
        peripheralsTopPivotTransform.localScale = Vector3.one;
        peripheralsBottomPivotTransform.localScale = Vector3.one;
    }
    
    private void RemoveSideToScale ( ) 
    {
        edgeIndicatorTop.SetActive ( false );
        edgeIndicatorBottom.SetActive ( false );
    }

    private void SetSideToScale ( int sideToScale ) 
    {
        switch ( Math.Sign ( sideToScale ) ) 
        {
            case 0:
                edgeIndicatorTop.SetActive ( false );
                edgeIndicatorBottom.SetActive ( false );
                break;

            case -1:
                edgeIndicatorTop.SetActive ( false );
                edgeIndicatorBottom.SetActive ( true );
                break;

            case 1:
                edgeIndicatorTop.SetActive ( true );
                edgeIndicatorBottom.SetActive ( false );
                break;
        }
    }

    public void Scale ( int sideToScale, int scaleValue ) 
    {
        var initialScale = transform.localScale;
        var initialPosition = transform.position;
        var scaleDirection = scaleValue > 0 ? 1 : -1;

        float scaleOffsetY = 1f * ( sideToScale != scaleDirection ? -1 : 1 );
        var isNewScaleTooLess = Mathf.Abs ( initialScale.y + scaleOffsetY ) < 1;

        float newScaleY = isNewScaleTooLess ? initialScale.y * -1f : initialScale.y + scaleOffsetY;
        float positionOffsetY = ( isNewScaleTooLess ? 0 : scaleOffsetY ) / 2.0F * sideToScale;
        
        transform.localScale = new Vector3 ( initialScale.x, newScaleY );
        transform.position = new Vector3 ( initialPosition.x, initialPosition.y + positionOffsetY );
        
        peripheralsTopPivotTransform.localScale = new Vector3 ( 1 / transform.localScale.x, Mathf.Abs ( 1 / transform.localScale.y ) );
        peripheralsBottomPivotTransform.localScale = new Vector3 ( 1 / transform.localScale.x, Mathf.Abs ( 1 / transform.localScale.y ) );

        if ( spriteRenderer.bounds.min.y <= Constants.InGameViewportVerticalRange.x || spriteRenderer.bounds.min.y >= Constants.InGameViewportVerticalRange.y || 
                        spriteRenderer.bounds.max.y <= Constants.InGameViewportVerticalRange.x || spriteRenderer.bounds.max.y >= Constants.InGameViewportVerticalRange.y ) 
        {
            transform.localScale = initialScale;
            transform.position = initialPosition;

            peripheralsTopPivotTransform.localScale = new Vector3 ( 1 / transform.localScale.x, Mathf.Abs ( 1 / transform.localScale.y ) );
            peripheralsBottomPivotTransform.localScale = new Vector3 ( 1 / transform.localScale.x, Mathf.Abs ( 1 / transform.localScale.y ) );
        }
        
        // transform.DOScale ( new Vector3 ( initialScale.x, newScaleY ), 0.25f );
        // transform.DOMove ( new Vector3 ( initialPosition.x, initialPosition.y + positionOffsetY ), 0.25f );
    }

    #endregion
}
