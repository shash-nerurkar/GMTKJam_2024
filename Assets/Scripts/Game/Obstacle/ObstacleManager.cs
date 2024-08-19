using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject obstaclePrefab;

    [ SerializeField ] [ Range ( 1f, 20f ) ] private float obstacleMoveSpeed = 10f;

    [ SerializeField ] private List<ObstaclePairSpawnData> obstaclePairSpawnDatas = new ( ) {
        new ( 1.5f, true, 0.2f ),
        new ( 2.5f, true, 0.2f ),
        new ( 3.5f, true, 0.2f ),
        new ( 4.5f, true, 0.2f ),
        new ( 5.5f, true, 0.2f ),
        new ( 6.5f, true, 0.2f ),
        new ( 7.5f, true, 3.5f ),

        new ( 2.5f, true, 1f ),
        new ( 2.5f, false, 3.5f ),

        new ( 3.5f, false, 0.5f ),
        new ( 3.5f, true, 0.5f ),
        new ( 3.5f, false, 0.5f )
    };

    private float _currentLifeTime;

    private float _currentMoveSpeed;

    private IEnumerator _spawnObstaclesCoroutine;
    
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

        SetObstaclesMoveSpeed ( obstacleMoveSpeed );
        
        _spawnObstaclesCoroutine = SpawnObstacles ( );
        StartCoroutine ( _spawnObstaclesCoroutine );
    }

    private void EndGame ( ) 
    {
        if ( _spawnObstaclesCoroutine != null ) 
            StopCoroutine ( _spawnObstaclesCoroutine );
    }


    #region Helpers

    private IEnumerator SpawnObstacles ( ) 
    {
        foreach ( var obstaclePairSpawnData in obstaclePairSpawnDatas ) 
        {
            Instantiate ( obstaclePrefab, transform ) 
                .GetComponent<Obstacle> ( ) 
                .Fire ( obstaclePairSpawnData.GapWidth, obstaclePairSpawnData.IsTop, _currentMoveSpeed, _currentLifeTime );

            yield return new WaitForSeconds ( obstaclePairSpawnData.DelayAfterInSeconds );
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

[ Serializable ]
public class ObstaclePairSpawnData 
{
    [ SerializeField ] [ Range ( 1.5f, 7.5f ) ] private float gapWidth;
    [ SerializeField ] private bool isTop;
    [ SerializeField ] [ Range ( 0f, 10f ) ] private float delayAfterInSeconds;

    public ObstaclePairSpawnData ( float gapWidth, bool isTop, float delayAfterInSeconds ) 
    {
        this.gapWidth = gapWidth;
        this.isTop = isTop;
        this.delayAfterInSeconds = delayAfterInSeconds;
    }

    public float GapWidth => gapWidth;
    public bool IsTop => isTop;
    public float DelayAfterInSeconds => delayAfterInSeconds;
}