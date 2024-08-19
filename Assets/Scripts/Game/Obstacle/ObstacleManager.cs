using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleManager : MonoBehaviour
{
    #region Constants

    private readonly List<ObstacleSpawnPattern> _tutorialPatterns = new ( ) { 
        new ObstacleSpawnPattern ( spawnDatas: new ( ) {
            new ( 5.5f, false, 2.5f ),
            new ( 3.5f, false, 4.5f ),
            new ( 2.5f, false, 2.5f ),
            new ( 1.5f, false, 4f ),
        } )
    };

    private readonly List<ObstacleSpawnPattern> _difficulty1Patterns = new ( ) { 
        new ObstacleSpawnPattern ( spawnDatas: new ( ) {
            new ( 1.5f, true, 1.5f ),
            new ( 2.5f, false, 1f ),
            new ( 2.5f, true, 1f ),
            new ( 1.5f, true, 4f ),
        } ),
    };

    private readonly List<ObstacleSpawnPattern> _difficulty2Patterns = new ( ) {  
        new ObstacleSpawnPattern ( spawnDatas: new ( ) {
            new ( 1.5f, false, 2.5f ),
            new ( 1.5f, true, 3.5f ),
            new ( 3.5f, false, 2f ),
            new ( 2.5f, false, 2f ),
            new ( 3.5f, true, 2f ),
            new ( 1.5f, true, 2f ),
            new ( 3.5f, false, 2f ),
            new ( 2.5f, true, 2f ),
            new ( 1.5f, true, 4f ),
        } ),
        new ObstacleSpawnPattern ( spawnDatas: new ( ) {
            new ( 1.5f, true, 2f ),
            new ( 2.5f, false, 2f ),
            new ( 1.5f, true, 2f ),
            new ( 2.5f, false, 2f ),
            new ( 2.5f, true, 2f ),
            new ( 2.5f, false, 2f ),
            new ( 2.5f, true, 2f ),
            new ( 1.5f, false, 2f ),
            new ( 1.5f, true, 4f ),
        } ),
    };

    private readonly List<ObstacleSpawnPattern> _difficulty3Patterns = new ( ) { 
        new ObstacleSpawnPattern ( spawnDatas: new ( ) {
            new ( 6.5f, false, 0f ),
            new ( 3.5f, true, 1f ),
            new ( 4.5f, false, 0f ),
            new ( 5.5f, true, 1f ),
            new ( 6.5f, true, 0f ),
            new ( 3.5f, false, 1f ),
            new ( 3.5f, true, 0f ),
            new ( 5.5f, false, 4f ),
        } ),
    };

    #endregion


    #region Fields

    [ SerializeField ] private GameObject obstaclePrefab;

    private IEnumerator _spawnObstaclesCoroutine;

    private int _currentDifficulty = 1;

    private float _obstacleMoveSpeed = 8f;

    private float _currentMoveSpeed;

    private float _currentLifeTime;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnGameStartAction += StartGame;
        GameManager.OnGameEndAction += EndGame;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction -= StartGame;
        GameManager.OnGameEndAction -= EndGame;
        
        if ( _spawnObstaclesCoroutine != null ) 
            StopCoroutine ( _spawnObstaclesCoroutine );
    }

    private void StartGame ( ) 
    {
        for ( var i = 0; i < transform.childCount; i++ ) 
            Destroy ( transform.GetChild ( i ).gameObject );

        SetObstaclesMoveSpeed ( _obstacleMoveSpeed );
        
        _spawnObstaclesCoroutine = SpawnObstacles (_tutorialPatterns [ Random.Range ( 0, _tutorialPatterns.Count ) ] );
        StartCoroutine ( _spawnObstaclesCoroutine );
    }

    private void EndGame ( ) 
    {
        if ( _spawnObstaclesCoroutine != null ) 
            StopCoroutine ( _spawnObstaclesCoroutine );

        _obstacleMoveSpeed = 7.5f;

        _currentDifficulty = 1;
    }


    #region Helpers

    private IEnumerator SpawnObstacles ( ObstacleSpawnPattern pattern = null ) 
    {
        if ( pattern == null ) 
        {
            switch ( _currentDifficulty ) 
            {
                case 1:
                    pattern = _difficulty1Patterns [ Random.Range ( 0, _difficulty1Patterns.Count ) ];
                    _currentDifficulty = 2;

                    break;
                
                case 2:
                    pattern = _difficulty2Patterns [ Random.Range ( 0, _difficulty2Patterns.Count ) ];
                    _currentDifficulty = 3;

                    break;
                
                case 3:
                    pattern = _difficulty3Patterns [ Random.Range ( 0, _difficulty3Patterns.Count ) ];
                    _currentDifficulty = 1;

                    break;
            }
        }

        if ( Random.Range ( 0.0f, 1.0f ) <= 0.5f ) 
            pattern.FlipAllObstacles ( );

        foreach ( var obstacleSpawnData in pattern.SpawnDatas ) 
        {
            Instantiate ( obstaclePrefab, transform ).GetComponent<Obstacle> ( ) 
                .Fire ( obstacleSpawnData.GapWidth, obstacleSpawnData.IsTop, _currentMoveSpeed, _currentLifeTime );

            yield return new WaitForSeconds ( obstacleSpawnData.DelayAfterInSeconds );
        }

        _spawnObstaclesCoroutine = SpawnObstacles ( );
        StartCoroutine ( _spawnObstaclesCoroutine );
    }

    private void SetObstaclesMoveSpeed ( float obstacleMoveSpeed ) 
    {
        float obstacleTravelDistance = 20;

        _currentLifeTime = obstacleTravelDistance / obstacleMoveSpeed;
        _currentMoveSpeed = obstacleMoveSpeed;
    }

    #endregion


    #endregion
}

[ Serializable ]
public class ObstacleSpawnData 
{
    [ SerializeField ] [ Range ( 1.5f, 7.5f ) ] private float gapWidth;
    [ SerializeField ] private bool isTop;
    [ SerializeField ] [ Range ( 0f, 10f ) ] private float delayAfterInSeconds;

    public ObstacleSpawnData ( float gapWidth, bool isTop, float delayAfterInSeconds ) 
    {
        this.gapWidth = gapWidth;
        this.isTop = isTop;
        this.delayAfterInSeconds = delayAfterInSeconds;
    }

    public void SwapDirection ( ) => isTop = !isTop;

    public float GapWidth => gapWidth;
    public bool IsTop => isTop;
    public float DelayAfterInSeconds => delayAfterInSeconds;
}


[ Serializable ]
public class ObstacleSpawnPattern
{
    [ SerializeField ] private readonly List<ObstacleSpawnData> spawnDatas;

    public ObstacleSpawnPattern ( List<ObstacleSpawnData> spawnDatas ) 
    {
        this.spawnDatas = spawnDatas;
    }

    public void FlipAllObstacles ( ) 
    {
        foreach ( var spawnData in spawnDatas ) 
            spawnData.SwapDirection ( );
    }

    public List<ObstacleSpawnData> SpawnDatas => spawnDatas;
}