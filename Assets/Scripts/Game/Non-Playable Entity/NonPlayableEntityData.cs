using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class NonPlayableEntityManager
{
    #region NPE Spawning 

    private readonly List<NPESpawnPattern> _tutorialPatterns = new ( ) { 
        new NPESpawnPattern ( spawnDatas: new ( ) {
            new ( 5.5f, false, 2.5f ),
            new ( 3.5f, false, 4.5f ),
            new ( 2.5f, false, 2.5f ),
            new ( 1.5f, false, 4f ),
        } )
    };

    private readonly List<NPESpawnPattern> _difficulty1Patterns = new ( ) { 
        new NPESpawnPattern ( spawnDatas: new ( ) {
            new ( 1.5f, true, 1.5f ),
            new ( 2.5f, false, 1f ),
            new ( 2.5f, true, 1f ),
            new ( 1.5f, true, 4f ),
        } ),
    };

    private readonly List<NPESpawnPattern> _difficulty2Patterns = new ( ) {  
        new NPESpawnPattern ( spawnDatas: new ( ) {
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
        new NPESpawnPattern ( spawnDatas: new ( ) {
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

    private readonly List<NPESpawnPattern> _difficulty3Patterns = new ( ) { 
        new NPESpawnPattern ( spawnDatas: new ( ) {
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

    private NPESpawnPattern GetRandomSpawnPattern ( List<NPESpawnPattern> spawnPattern ) => spawnPattern [ Random.Range ( 0, spawnPattern.Count ) ];

    #endregion


    #region NPE Speed Scaling

    private const float NPEInitialMoveSpeed = 8f;

    #endregion
}


[ Serializable ]
public class NPESpawnPattern
{
    [ SerializeField ] private readonly List<ObstacleData> spawnDatas;

    public NPESpawnPattern ( List<ObstacleData> spawnDatas ) 
    {
        this.spawnDatas = spawnDatas;
    }

    public void FlipAllObstacles ( ) 
    {
        foreach ( var spawnData in spawnDatas ) 
            spawnData.SwapDirection ( );
    }

    public List<ObstacleData> SpawnDatas => spawnDatas;
}


[ Serializable ]
public class ObstacleData 
{
    [ SerializeField ] [ Range ( 1.5f, 7.5f ) ] private float gapWidth;
    [ SerializeField ] private bool isTop;
    [ SerializeField ] [ Range ( 0f, 10f ) ] private float delayAfterInSeconds;

    public ObstacleData ( float gapWidth, bool isTop, float delayAfterInSeconds ) 
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
public class CollectibleData 
{
    [ SerializeField ] [ Range ( 1.5f, 7.5f ) ] private float positionY;
    [ SerializeField ] [ Range ( 0f, 10f ) ] private float delayAfterInSeconds;

    public CollectibleData ( float positionY, float delayAfterInSeconds ) 
    {
        this.positionY = positionY;
        this.delayAfterInSeconds = delayAfterInSeconds;
    }

    public float PositionY => positionY;
    public float DelayAfterInSeconds => delayAfterInSeconds;
}
