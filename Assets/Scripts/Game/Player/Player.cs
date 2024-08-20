using System;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields

    
    #region Serialized

    [ SerializeField ] private SpriteRenderer spriteRenderer;

    [ Header ( "Expressions" ) ]

    [ SerializeField ] private SpriteRenderer faceSpriteRenderer;

    [ SerializeField ] private Sprite expressionBaseSprite;

    [ SerializeField ] private Sprite expressionStretchSprite;

    [ SerializeField ] private Sprite expressionDyingSprite;

    [ Header ( "Peripheral Pivots" ) ]

    [ SerializeField ] private Transform peripheralsTopPivotTransform;

    [ SerializeField ] private Transform peripheralsBottomPivotTransform;

    [ Header ( "Indicators" ) ]

    [ SerializeField ] private GameObject edgeIndicatorTop;

    [ SerializeField ] private GameObject edgeIndicatorBottom;

    #endregion


    private Vector3 _basePosition;

    private float _currentScaleSensitivity = 1;

    private Sequence _scaleTweenSequence;

    private Tween _scaleExpressionTween;

    private Vector3 _currentDestinationScale;

    private Vector3 _currentDestinationPosition;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        InputManager.ScaleSideSelectedAction += SetSideToScale;
        InputManager.ScaleSideUnselectedAction += RemoveSideToScale;
        InputManager.ScalePlayerAction += Scale;

        GameManager.OnGameStartAction += Initialize;

        Obstacle.OnPlayerHitAction += Kill;

        HUDManager.AdjustSensitivityAction += AdjustScaleSensitivity;

        _basePosition = transform.position;
    }

    private void OnDestroy ( ) 
    {
        InputManager.ScaleSideSelectedAction -= SetSideToScale;
        InputManager.ScaleSideUnselectedAction -= RemoveSideToScale;
        InputManager.ScalePlayerAction -= Scale;

        GameManager.OnGameStartAction -= Initialize;

        Obstacle.OnPlayerHitAction -= Kill;

        HUDManager.AdjustSensitivityAction -= AdjustScaleSensitivity;
    }

    private void Initialize ( ) 
    {
        transform.position = _basePosition;

        faceSpriteRenderer.sprite = expressionBaseSprite;

        transform.localScale = Vector3.one;
        peripheralsTopPivotTransform.localScale = Vector3.one;
        peripheralsBottomPivotTransform.localScale = Vector3.one;
    }
    
    private void Kill ( ) 
    {
        faceSpriteRenderer.sprite = expressionDyingSprite;
        
        _scaleTweenSequence.Kill ( );
        _scaleExpressionTween.Kill ( );
    }

    private void AdjustScaleSensitivity ( float fraction ) => _currentScaleSensitivity = fraction;

    private void RemoveSideToScale ( ) 
    {
        edgeIndicatorTop.SetActive ( false );
        edgeIndicatorBottom.SetActive ( false );
    }

    private void SetSideToScale ( int sideToScale ) 
    {
        switch ( sideToScale ) 
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

            case 2:
                edgeIndicatorTop.SetActive ( true );
                edgeIndicatorBottom.SetActive ( true );
                break;
        }
    }

    public void Scale ( int sideToScale, int scaleValue ) 
    {
        // CALCULATE OFFSET
        var initialScale = _scaleTweenSequence.IsActive ( ) ? _currentDestinationScale : transform.localScale;
        var initialPosition = _scaleTweenSequence.IsActive ( ) ? _currentDestinationPosition : transform.position;
        var scaleDirection = Math.Sign ( scaleValue );
        var initialScaleFlip = Math.Sign ( initialScale.y );

        var scaleOffsetY = _currentScaleSensitivity * 1f * ( sideToScale != scaleDirection ? -1 : 1 );
        var isNewScaleTooLess = Mathf.Abs ( initialScale.y + scaleOffsetY ) < 1;

        float newScaleY;
        float positionOffsetY;
        if ( isNewScaleTooLess && Mathf.Abs ( initialScale.y ) == 1 ) 
        {
            newScaleY = initialScale.y * -1f;
            positionOffsetY = 0;
        }
        else 
        {
            if ( isNewScaleTooLess ) 
                scaleOffsetY = ( 1 - Mathf.Abs ( initialScale.y ) ) * initialScaleFlip;

            newScaleY = initialScale.y + scaleOffsetY;
            positionOffsetY = scaleOffsetY / 2.0F * sideToScale;
        }

        _currentDestinationScale = new Vector3 ( initialScale.x, newScaleY );
        _currentDestinationPosition = new Vector3 ( initialPosition.x, initialPosition.y + positionOffsetY );
        


        // IF OFFSET IS SENDING PLAYER OUT-OF-BOUNDS, ADJUST IT
        var isSideFacingAwayFromScaleDirection = 
                    ( sideToScale == -1 && initialScaleFlip ==  1 && scaleDirection ==  1 ) || 
                    ( sideToScale ==  1 && initialScaleFlip ==  1 && scaleDirection == -1 ) || 
                    ( sideToScale == -1 && initialScaleFlip == -1 && scaleDirection == -1 ) || 
                    ( sideToScale ==  1 && initialScaleFlip == -1 && scaleDirection ==  1 );

        transform.localScale = _currentDestinationScale;
        transform.position = _currentDestinationPosition;

        var newBottomLeftY = spriteRenderer.bounds.min.y;
        var newTopRightY = spriteRenderer.bounds.max.y;

        transform.localScale = initialScale;
        transform.position = initialPosition;

        if ( !isSideFacingAwayFromScaleDirection ) 
        {
            if ( newBottomLeftY < Constants.InGameViewportVerticalRange.x ) 
            {
                _currentDestinationScale = initialScale;
                _currentDestinationPosition = initialPosition + new Vector3 ( 0, Constants.InGameViewportVerticalRange.x - spriteRenderer.bounds.min.y, 0 );
            }
            else if ( newTopRightY > Constants.InGameViewportVerticalRange.y ) 
            {
                _currentDestinationScale = initialScale;
                _currentDestinationPosition = initialPosition + new Vector3 ( 0, Constants.InGameViewportVerticalRange.y - spriteRenderer.bounds.max.y, 0 );
            }
        }



        // ANIMATE
        if ( _scaleTweenSequence.IsActive ( ) ) 
            _scaleTweenSequence.Kill ( );

        _scaleTweenSequence = DOTween.Sequence ( );
        var sequenceSpeed = 0.05f;
        
        _scaleTweenSequence.Join ( transform.DOScale ( _currentDestinationScale, sequenceSpeed ) );
        _scaleTweenSequence.Join ( transform.DOMove ( _currentDestinationPosition, sequenceSpeed ) );
        _scaleTweenSequence.Join ( peripheralsTopPivotTransform.DOScale ( new Vector3 ( 1 / initialScale.x, Mathf.Abs ( 1 / _currentDestinationScale.y ) ), sequenceSpeed ) );
        _scaleTweenSequence.Join ( peripheralsBottomPivotTransform.DOScale ( new Vector3 ( 1 / initialScale.x, Mathf.Abs ( 1 / _currentDestinationScale.y ) ), sequenceSpeed ) );

        _scaleTweenSequence.Play ( );

        if ( _scaleExpressionTween.IsActive ( ) ) 
            _scaleExpressionTween.Kill ( );
        
        faceSpriteRenderer.sprite = expressionStretchSprite;
        _scaleExpressionTween = DOVirtual.DelayedCall ( 0.5f, ( ) => faceSpriteRenderer.sprite = expressionBaseSprite );
    }

    #endregion
}
