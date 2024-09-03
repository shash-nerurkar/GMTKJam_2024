using System;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields

    
    #region Serialized

    [ Header ( "Body" ) ]

    [ SerializeField ] private SpriteRenderer bodySpriteRenderer;

    [ Header ( "Expression" ) ]

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
    
    private Vector3 _bodySpriteBaseSize;

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

        Collectible.OnPlayerHitAction += Collect;

        HUDManager.AdjustSensitivityAction += AdjustScaleSensitivity;
    }

    private void OnDestroy ( ) 
    {
        InputManager.ScaleSideSelectedAction -= SetSideToScale;
        InputManager.ScaleSideUnselectedAction -= RemoveSideToScale;
        InputManager.ScalePlayerAction -= Scale;

        GameManager.OnGameStartAction -= Initialize;

        Obstacle.OnPlayerHitAction -= Kill;

        Collectible.OnPlayerHitAction -= Collect;

        HUDManager.AdjustSensitivityAction -= AdjustScaleSensitivity;
        
        _scaleTweenSequence.Kill ( );
        _scaleExpressionTween.Kill ( );
    }

    private void Start ( ) 
    {
        _basePosition = transform.position;
        _bodySpriteBaseSize = bodySpriteRenderer.size;
    }

    private void Initialize ( ) 
    {
        transform.position = _basePosition;
        bodySpriteRenderer.size = _bodySpriteBaseSize;

        faceSpriteRenderer.sprite = expressionBaseSprite;

        transform.localScale = Vector3.one;
        bodySpriteRenderer.transform.localScale = Vector3.one;
        peripheralsTopPivotTransform.localScale = Vector3.one;
        peripheralsBottomPivotTransform.localScale = Vector3.one;
    }

    private void Kill ( ) 
    {
        SoundManager.Instance.Play ( SoundType.PlayerHitObstacle );

        faceSpriteRenderer.sprite = expressionDyingSprite;
        
        _scaleTweenSequence.Kill ( );
        _scaleExpressionTween.Kill ( );
    }
    
    private void Collect ( ) 
    {
        SoundManager.Instance.Play ( SoundType.PlayerHitCollectible );
    }

    private void AdjustScaleSensitivity ( float fraction ) => _currentScaleSensitivity = fraction;

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

    private void RemoveSideToScale ( ) 
    {
        edgeIndicatorTop.SetActive ( false );
        edgeIndicatorBottom.SetActive ( false );
    }

    public void Scale ( int sideToScale, int scaleValue ) 
    {
        // CALCULATE OFFSET
        var initialScale = _scaleTweenSequence.IsActive ( ) ? _currentDestinationScale : transform.localScale;
        var initialPosition = _scaleTweenSequence.IsActive ( ) ? _currentDestinationPosition : transform.position;
        var scaleDirection = Math.Sign ( scaleValue );
        var initialScaleFlip = Math.Sign ( initialScale.y );

        float scaleOffsetY = _currentScaleSensitivity * 1f * ( sideToScale != scaleDirection ? -1 : 1 );
        float positionOffsetY;

        var isNewScaleTooLess = Mathf.Abs ( initialScale.y + scaleOffsetY ) < 1;
        var isFlipping = isNewScaleTooLess && Mathf.Abs ( initialScale.y ) == 1;
        if ( isFlipping ) 
        {
            scaleOffsetY = initialScale.y * -2f;
            positionOffsetY = 0;
        }
        else 
        {
            if ( isNewScaleTooLess ) 
                scaleOffsetY = ( 1 - Mathf.Abs ( initialScale.y ) ) * initialScaleFlip;
            
            positionOffsetY = scaleOffsetY / 2 * sideToScale;
        }
        
        _currentDestinationScale = new Vector3 ( initialScale.x, initialScale.y + scaleOffsetY );
        _currentDestinationPosition = new Vector3 ( initialPosition.x, initialPosition.y + positionOffsetY );
        
        
        
        // IF OFFSET IS SENDING PLAYER OUT-OF-BOUNDS, ADJUST IT
        var isSideFacingAwayFromScaleDirection = 
            ( sideToScale == -1 && initialScaleFlip ==  1 && scaleDirection ==  1 ) || 
            ( sideToScale ==  1 && initialScaleFlip ==  1 && scaleDirection == -1 ) || 
            ( sideToScale == -1 && initialScaleFlip == -1 && scaleDirection == -1 ) || 
            ( sideToScale ==  1 && initialScaleFlip == -1 && scaleDirection ==  1 );
        
        CalculateAdjustedBounds ( _currentDestinationScale, _currentDestinationPosition, out float newBoundsMinY, out float newBoundsMaxY );

        if ( !isSideFacingAwayFromScaleDirection ) 
        {
            if ( newBoundsMinY < Constants.InGameViewportVerticalRange.x ) 
            {
                _currentDestinationScale = initialScale;
                _currentDestinationPosition = initialPosition + new Vector3 ( 0, Constants.InGameViewportVerticalRange.x - bodySpriteRenderer.bounds.min.y, 0 );
            }
            else if ( newBoundsMaxY > Constants.InGameViewportVerticalRange.y ) 
            {
                _currentDestinationScale = initialScale;
                _currentDestinationPosition = initialPosition + new Vector3 ( 0, Constants.InGameViewportVerticalRange.y - bodySpriteRenderer.bounds.max.y, 0 );
            }
        }



        // ANIMATE
        if ( _scaleTweenSequence.IsActive ( ) ) 
            _scaleTweenSequence.Kill ( );

        _scaleTweenSequence = DOTween.Sequence ( );
        var sequenceSpeed = 0.05f;
        
        var peripheralsScale = new Vector2 ( 1 / initialScale.x, Mathf.Abs ( 1 / _currentDestinationScale.y ) );
        
        _scaleTweenSequence.Join ( transform.DOScale ( _currentDestinationScale, sequenceSpeed ) );
        _scaleTweenSequence.Join ( transform.DOMove ( _currentDestinationPosition, sequenceSpeed ) );
        _scaleTweenSequence.Join ( DOTween.To ( ( ) => bodySpriteRenderer.size, x => bodySpriteRenderer.size = x, ( Vector2 ) _currentDestinationScale * _bodySpriteBaseSize, sequenceSpeed ) );

        _scaleTweenSequence.Join ( bodySpriteRenderer.transform.DOScale ( peripheralsScale, sequenceSpeed ) );
        _scaleTweenSequence.Join ( peripheralsTopPivotTransform.DOScale ( peripheralsScale, sequenceSpeed ) );
        _scaleTweenSequence.Join ( peripheralsBottomPivotTransform.DOScale ( peripheralsScale, sequenceSpeed ) );

        _scaleTweenSequence.Play ( );



        // CHANGE EXPRESSION
        if ( _scaleExpressionTween.IsActive ( ) ) 
            _scaleExpressionTween.Kill ( );
        
        faceSpriteRenderer.sprite = expressionStretchSprite;
        _scaleExpressionTween = DOVirtual.DelayedCall ( 0.5f, ( ) => faceSpriteRenderer.sprite = expressionBaseSprite );

        return;

        
        void CalculateAdjustedBounds ( Vector3 newScale, Vector3 newPosition, out float newMinY, out float newMaxY ) 
        {
            var scaleFactor = new Vector3 ( newScale.x / initialScale.x, newScale.y / initialScale.y );

            var newSize = Vector3.Scale ( bodySpriteRenderer.bounds.size, scaleFactor );
            var newCenter = newPosition + Vector3.Scale ( bodySpriteRenderer.bounds.center - initialPosition, scaleFactor );

            var oneY = newCenter.y - newSize.y / 2f;
            var twoY = newCenter.y + newSize.y / 2f;

            newMinY = Mathf.Min ( oneY, twoY );
            newMaxY = Mathf.Max ( oneY, twoY );
        }
    }

    #endregion
}
