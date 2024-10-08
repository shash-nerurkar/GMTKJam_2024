using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class NonPlayableEntityManager : MonoBehaviour
{
    #region Actions

    public static event Action<int, int> OnCollectibleCollectedAction;

    public static event Action<float, float, int, int> ShowRatingAction;

    #endregion


    #region Fields

    [ SerializeField ] private GameObject obstaclePrefab;

    [ SerializeField ] private GameObject collectiblePrefab;

    [ SerializeField ] private NPESpawnPattern testPattern = new ( 
        obstacleDatas: new ( ) { 
            new ( 5.5f, false, 2.5f ),
            new ( 3.5f, false, 4.5f ),
            new ( 2.5f, false, 2.5f ),
            new ( 1.5f, false, 4f ),
        },
        collectibleDatas: new ( ) { 
            new ( 1f, 2f ),
            new ( 2f, 4f ),
            new ( 3f, 2f ),
            new ( 4f, 4f ),
        }
    );

    private SoundType [ ] currentTrackStingers;
    
    private IEnumerator _spawnNPEsCoroutine;

    private IEnumerator _spawnObstaclesCoroutine;

    private IEnumerator _spawnCollectiblesCoroutine;

    private IEnumerator _scaleNPESpeedCoroutine;

    private int _timeElapsedInSeconds;

    private int _currentDifficulty = 1;

    private float _currentNPESpeedScale = 1;

    private int _currentCollectibleCollectedCount;

    private int _currentCollectibleSpawnCount;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnGameStartAction += StartGame;
        GameManager.OnGameEndAction += EndGame;

        Collectible.OnPlayerHitAction += OnCollectibleCollected;

        SoundManager.OnPlayTrackAction += SetCurrentStingerList;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction -= StartGame;
        GameManager.OnGameEndAction -= EndGame;

        Collectible.OnPlayerHitAction -= OnCollectibleCollected;

        SoundManager.OnPlayTrackAction -= SetCurrentStingerList;
        
        if ( _spawnNPEsCoroutine != null ) 
            StopCoroutine ( _spawnNPEsCoroutine );
            
        if ( _spawnObstaclesCoroutine != null ) 
            StopCoroutine ( _spawnObstaclesCoroutine );
            
        if ( _spawnCollectiblesCoroutine != null ) 
            StopCoroutine ( _spawnCollectiblesCoroutine );
            
        if ( _scaleNPESpeedCoroutine != null ) 
            StopCoroutine ( _scaleNPESpeedCoroutine );
    }

    private void StartGame ( ) 
    {
        for ( var i = 0; i < transform.childCount; i++ ) 
            Destroy ( transform.GetChild ( i ).gameObject );
        
        _spawnNPEsCoroutine = SpawnNPEs ( testPattern != null && !testPattern.IsEmpty ( ) 
                                                        ? testPattern : GetRandomSpawnPattern ( _tutorialPatterns ) );
        StartCoroutine ( _spawnNPEsCoroutine );
        
        _scaleNPESpeedCoroutine = ScaleNPEMoveSpeed ( );
        StartCoroutine ( _scaleNPESpeedCoroutine );
    }

    private void EndGame ( ) 
    {
        if ( _spawnNPEsCoroutine != null ) 
            StopCoroutine ( _spawnNPEsCoroutine );
            
        if ( _spawnObstaclesCoroutine != null ) 
            StopCoroutine ( _spawnObstaclesCoroutine );
            
        if ( _spawnCollectiblesCoroutine != null ) 
            StopCoroutine ( _spawnCollectiblesCoroutine );
            
        if ( _scaleNPESpeedCoroutine != null ) 
            StopCoroutine ( _scaleNPESpeedCoroutine );

        ShowRatingAction?.Invoke ( _currentNPESpeedScale, NPEMaxSpeedScale, _currentCollectibleCollectedCount, _currentCollectibleSpawnCount );

        _timeElapsedInSeconds = 0;
        _currentDifficulty = 1;
        _currentNPESpeedScale = 1;
        _currentCollectibleCollectedCount = 0;
        _currentCollectibleSpawnCount = 0;
    }

    private void SetCurrentStingerList ( Music track ) => currentTrackStingers = Constants.TrackToStingers [ track.Type ];

    private void OnCollectibleCollected ( ) 
    {
        ++_currentCollectibleCollectedCount;

        OnCollectibleCollectedAction?.Invoke ( _currentCollectibleCollectedCount, _currentCollectibleSpawnCount );
    }


    #region NPE Spawning

    private IEnumerator SpawnNPEs ( NPESpawnPattern pattern = null, bool doNotFlip = false ) 
    {
        pattern ??= ChoosePatternToSpawn ( );

        if ( !doNotFlip && Random.Range ( 0.0f, 1.0f ) <= 0.5f ) 
            pattern.Flip ( );
        
        float currentNPEMoveSpeed = NPEInitialMoveSpeed * _currentNPESpeedScale;
        float currentNPELifeTime = 20 / currentNPEMoveSpeed;
        
        bool haveAllObstaclesSpawned = false;
        _spawnObstaclesCoroutine = SpawnObstacles ( );
        StartCoroutine ( _spawnObstaclesCoroutine );

        bool haveAllCollectiblesSpawned = false;
        _spawnCollectiblesCoroutine = SpawnCollectibles ( );
        StartCoroutine ( _spawnCollectiblesCoroutine );

        yield return new WaitUntil ( ( ) => haveAllObstaclesSpawned && haveAllCollectiblesSpawned );

        _spawnNPEsCoroutine = SpawnNPEs ( );
        StartCoroutine ( _spawnNPEsCoroutine );
    
        
        IEnumerator SpawnObstacles ( ) 
        {
            foreach ( var obstacleData in pattern.ObstacleDatas ) 
            {
                Instantiate ( obstaclePrefab, transform ).GetComponent<Obstacle> ( ) 
                    .Initialize ( obstacleData.GapWidth, obstacleData.IsTop, currentNPEMoveSpeed, currentNPELifeTime, Constants.GetStingerTypeByGapWidth ( currentTrackStingers, obstacleData.GapWidth ) );

                yield return new WaitForSeconds ( obstacleData.DelayAfterInSeconds );
            }

            haveAllObstaclesSpawned = true;
        }
        

        IEnumerator SpawnCollectibles ( ) 
        {
            foreach ( var collectibleData in pattern.CollectibleDatas ) 
            {
                Instantiate ( collectiblePrefab, transform ).GetComponent<Collectible> ( ) 
                    .Initialize ( collectibleData.PositionY, currentNPEMoveSpeed, currentNPELifeTime );
                ++_currentCollectibleSpawnCount;

                yield return new WaitForSeconds ( collectibleData.DelayAfterInSeconds );
            }

            haveAllCollectiblesSpawned = true;
        }
    }

    private NPESpawnPattern ChoosePatternToSpawn ( ) 
    {
        if ( _timeElapsedInSeconds < 60 ) 
        {
            var randomVal = Random.Range ( 0.0f, 1.0f );

            if ( randomVal < 0.4f ) 
                return GetRandomSpawnPattern ( _difficulty1Patterns );
            else if ( randomVal < 0.8f ) 
                return GetRandomSpawnPattern ( _collectibleOnlyPatterns );
            else 
                return GetRandomSpawnPattern ( _difficulty2Patterns );
        }
        else if ( _timeElapsedInSeconds < 120 ) 
        {
            var randomVal = Random.Range ( 0.0f, 1.0f );

            if ( randomVal < 0.4f ) 
                return GetRandomSpawnPattern ( _difficulty2Patterns );
            else if ( randomVal < 0.8f ) 
                return GetRandomSpawnPattern ( _collectibleOnlyPatterns );
            else 
                return GetRandomSpawnPattern ( _difficulty3Patterns );
        }
        else 
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

                    var randomVal = Random.Range ( 0.0f, 1.0f );

                    if ( randomVal < 0.5f ) 
                        return GetRandomSpawnPattern ( _collectibleOnlyPatterns );
                    else 
                        return GetRandomSpawnPattern ( _difficulty3Patterns );
            }
        }


        return GetRandomSpawnPattern ( _tutorialPatterns );
    }

    #endregion


    #region NPE Speed Scaling

    private IEnumerator ScaleNPEMoveSpeed ( ) 
    {
        var initialScaleIncrement = 1.0f / 600.0f;

        while ( true ) 
        {
            _currentNPESpeedScale = Mathf.Min ( _currentNPESpeedScale + initialScaleIncrement, NPEMaxSpeedScale );
            ++_timeElapsedInSeconds;
            
            yield return new WaitForSeconds ( 1 );
        }
    }

    #endregion


    #endregion
}