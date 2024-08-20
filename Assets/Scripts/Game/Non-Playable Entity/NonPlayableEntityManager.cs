using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class NonPlayableEntityManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject obstaclePrefab;

    [ SerializeField ] private GameObject collectiblePrefab;

    private IEnumerator _spawnNPEsCoroutine;

    private IEnumerator _scaleNPESpeedCoroutine;

    private int _currentDifficulty = 1;

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
        
        if ( _spawnNPEsCoroutine != null ) 
            StopCoroutine ( _spawnNPEsCoroutine );
            
        if ( _scaleNPESpeedCoroutine != null ) 
            StopCoroutine ( _scaleNPESpeedCoroutine );
    }

    private void StartGame ( ) 
    {
        for ( var i = 0; i < transform.childCount; i++ ) 
            Destroy ( transform.GetChild ( i ).gameObject );
        
        // _spawnNPEsCoroutine = SpawnNPEs ( GetRandomSpawnPattern ( _tutorialPatterns ) );
        // StartCoroutine ( _spawnNPEsCoroutine );
        
        // _scaleNPESpeedCoroutine = ScaleNPEMoveSpeed ( );
        // StartCoroutine ( _scaleNPESpeedCoroutine );
    }

    private void EndGame ( ) 
    {
        if ( _spawnNPEsCoroutine != null ) 
            StopCoroutine ( _spawnNPEsCoroutine );
            
        if ( _scaleNPESpeedCoroutine != null ) 
            StopCoroutine ( _scaleNPESpeedCoroutine );

        _currentDifficulty = 1;
        _currentMoveSpeed = 0;
        _currentLifeTime = 0;
    }


    #region NPE Spawning

    private IEnumerator SpawnNPEs ( NPESpawnPattern pattern = null ) 
    {
        pattern ??= ChoosePatternToSpawn ( );

        if ( Random.Range ( 0.0f, 1.0f ) <= 0.5f ) 
            pattern.FlipAllObstacles ( );

        // Instantiate ( obstaclePrefab, transform ).GetComponent<Obstacle> ( ) 
        //     .Initialize ( obstacleSpawnData.GapWidth, obstacleSpawnData.IsTop, _currentMoveSpeed, _currentLifeTime );

        foreach ( var obstacleSpawnData in pattern.SpawnDatas ) 
        {
            Instantiate ( obstaclePrefab, transform ).GetComponent<Obstacle> ( ) 
                .Initialize ( obstacleSpawnData.GapWidth, obstacleSpawnData.IsTop, _currentMoveSpeed, _currentLifeTime );

            yield return new WaitForSeconds ( obstacleSpawnData.DelayAfterInSeconds );
        }

        _spawnNPEsCoroutine = SpawnNPEs ( );
        StartCoroutine ( _spawnNPEsCoroutine );
    }

    private NPESpawnPattern ChoosePatternToSpawn ( ) 
    {
        switch ( _currentDifficulty ) 
        {
            case 1:
                _currentDifficulty = 2;
                return GetRandomSpawnPattern ( _difficulty1Patterns );
            
            case 2:
                _currentDifficulty = 3;
                return GetRandomSpawnPattern ( _difficulty2Patterns );
            
            case 3:
                _currentDifficulty = 1;
                return GetRandomSpawnPattern ( _difficulty3Patterns );
        }

        return GetRandomSpawnPattern ( _tutorialPatterns );
    }

    #endregion


    #region NPE Speed Management

    private IEnumerator ScaleNPEMoveSpeed ( ) 
    {
        while ( true ) 
        {
            SetObstaclesMoveSpeed ( NPEInitialMoveSpeed );

            yield return new WaitForSeconds ( 30 );
        }
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