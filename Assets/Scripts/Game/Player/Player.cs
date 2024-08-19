using System;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields

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

    private Vector3 _basePosition;

    private Sequence _scaleTweenSequence;

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

        _basePosition = transform.position;
    }

    private void OnDestroy ( ) 
    {
        InputManager.ScaleSideSelectedAction -= SetSideToScale;
        InputManager.ScaleSideUnselectedAction -= RemoveSideToScale;

        InputManager.ScalePlayerAction -= Scale;

        GameManager.OnGameStartAction -= Initialize;

        Obstacle.OnPlayerHitAction += Kill;
    }

    private void Initialize ( ) 
    {
        transform.position = _basePosition;
        faceSpriteRenderer.sprite = expressionBaseSprite;

        transform.localScale = Vector3.one;
        peripheralsTopPivotTransform.localScale = Vector3.one;
        peripheralsBottomPivotTransform.localScale = Vector3.one;
    }
    
    private void Kill ( ) => faceSpriteRenderer.sprite = expressionDyingSprite;

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
        var initialScale = _scaleTweenSequence.IsActive ( ) ? _currentDestinationScale : transform.localScale;
        var initialPosition = _scaleTweenSequence.IsActive ( ) ? _currentDestinationPosition : transform.position;

        float scaleOffsetY = 1f * ( sideToScale != Math.Sign ( scaleValue ) ? -1 : 1 );
        var isNewScaleTooLess = Mathf.Abs ( initialScale.y + scaleOffsetY ) < 1;

        float newScaleY = isNewScaleTooLess ? initialScale.y * -1f : initialScale.y + scaleOffsetY;
        float positionOffsetY = ( isNewScaleTooLess ? 0 : scaleOffsetY ) / 2.0F * sideToScale;
        
        transform.localScale = new Vector3 ( initialScale.x, newScaleY );
        transform.position = new Vector3 ( initialPosition.x, initialPosition.y + positionOffsetY );

        var newBottomLeftY = spriteRenderer.bounds.min.y;
        var newTopRightY = spriteRenderer.bounds.max.y;

        transform.localScale = initialScale;
        transform.position = initialPosition;

        if ( !Constants.IsPointYInsideInGameViewport ( newBottomLeftY ) || !Constants.IsPointYInsideInGameViewport ( newTopRightY ) ) 
            return;

        _currentDestinationScale = new Vector3 ( initialScale.x, newScaleY );
        _currentDestinationPosition = new Vector3 ( initialPosition.x, initialPosition.y + positionOffsetY );

        if ( _scaleTweenSequence.IsActive ( ) ) 
            _scaleTweenSequence.Kill ( );

        _scaleTweenSequence = DOTween.Sequence ( );
        var sequenceSpeed = 0.1f;
        
        _scaleTweenSequence.Join ( transform.DOScale ( _currentDestinationScale, sequenceSpeed ) );
        _scaleTweenSequence.Join ( transform.DOMove ( _currentDestinationPosition, sequenceSpeed ) );
        _scaleTweenSequence.Join ( peripheralsTopPivotTransform.DOScale ( new Vector3 ( 1 / initialScale.x, Mathf.Abs ( 1 / newScaleY ) ), sequenceSpeed ) );
        _scaleTweenSequence.Join ( peripheralsBottomPivotTransform.DOScale ( new Vector3 ( 1 / initialScale.x, Mathf.Abs ( 1 / newScaleY ) ), sequenceSpeed ) );

        faceSpriteRenderer.sprite = expressionStretchSprite;
        _scaleTweenSequence.Play ( )
            .OnComplete ( ( ) => faceSpriteRenderer.sprite = expressionBaseSprite ) ;
    }

    #endregion
}
