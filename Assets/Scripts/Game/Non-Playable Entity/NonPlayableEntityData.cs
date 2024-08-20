using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class NonPlayableEntityManager
{
    #region NPE Spawning 

    private readonly List<NPESpawnPattern> _tutorialPatterns = new ( ) { 
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 5.5f, false, 2.5f ),
                new ( 3.5f, false, 4.5f ),
                new ( 2.5f, false, 2.5f ),
                new ( 1.5f, false, 4f ),
            },
            collectibleDatas : new ( ) 
        )
    };

    private readonly List<NPESpawnPattern> _difficulty1Patterns = new ( ) { 
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 1.5f, true, 1.5f ),
                new ( 2.5f, false, 1f ),
                new ( 2.5f, true, 1f ),
                new ( 1.5f, true, 4f ),
            },
            collectibleDatas : new ( ) {
                new ( -3, 1 ),
                new ( 1, 1 ),
                new ( 1, 0.9f ),
                new ( 2, 1 ),
            }
        )
    };

    private readonly List<NPESpawnPattern> _difficulty2Patterns = new ( ) {  
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 1.5f, false, 2.5f ),
                new ( 1.5f, true, 3.5f ),
                new ( 3.5f, false, 2f ),
                new ( 2.5f, false, 2f ),
                new ( 3.5f, true, 2f ),
                new ( 1.5f, true, 2f ),
                new ( 3.5f, false, 2f ),
                new ( 2.5f, true, 2f ),
                new ( 1.5f, true, 4f ),
            },
            collectibleDatas : new ( ) { 
                new (  3, 2.5f ),
                new ( -3, 2.7f ),
                new (  0, 2f ),
                new ( -3, 2f ),
                new ( -3, 1.5f ),
                new (  0, 2f ),
                new ( -1, 2f ),
            }
        ),
        
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 1.5f, true, 2f ),
                new ( 2.5f, false, 2f ),
                new ( 1.5f, true, 2f ),
                new ( 2.5f, false, 2f ),
                new ( 2.5f, true, 2f ),
                new ( 2.5f, false, 2f ),
                new ( 2.5f, true, 2f ),
                new ( 1.5f, false, 2f ),
                new ( 1.5f, true, 4f ),
            },
            collectibleDatas : new ( ) {
                new ( -3, 2.5f ),
                new ( 0, 2.5f ),
                new ( 2, 2.5f ),
                new ( -1, 2.5f )
            }
        )
    };

    private readonly List<NPESpawnPattern> _difficulty3Patterns = new ( ) { 
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 6.5f, false, 0f ),
                new ( 3.5f, true, 1f ),
                new ( 4.5f, false, 0f ),
                new ( 5.5f, true, 1f ),
                new ( 6.5f, true, 0f ),
                new ( 3.5f, false, 1f ),
                new ( 3.5f, true, 0f ),
                new ( 5.5f, false, 4f ),
            },
            collectibleDatas : new ( ) { }
        ),
        
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) { 
                new ( 1.5f, true, 1.5f ),
                new ( 2.5f, false, 0.5f ),
                new ( 3.5f, false, 1f ),
                new ( 2.5f, true, 1.3f ),
                new ( 1.5f, false, 1.5f ),
                new ( 1.5f, true, 1.5f ),
            },
            collectibleDatas : new ( ) { 
                new ( -3, 1 ),
                new (  0, 2.7f ),
                new (  0, 1 ),
                new (  3, 1.3f ),
                new (  3, 0.2f ),
                new (  3, 0.5f ),
                new (  3, 0.5f ),
            }
        ),
        
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) { 
                new ( 2.5f, true, 1.5f ),
                new ( 3.5f, true, 2f ),
                new ( 2.5f, true, 1.5f ),
                new ( 1.5f, true, 2.5f ),
                new ( 1.5f, true, 2f ),
                new ( 1.5f, true, 2f ),
            },
            collectibleDatas : new ( ) { 
                new ( -2, 2.5f ),
                new (  0, 2 ),
                new (  2, 2.5f ),
                new ( -1, 2.5f ),
            }
        ),

        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 1.5f, true, 1f ),
                new ( 2.5f, false, 1f ),
                new ( 1.5f, true, 1f ),
                new ( 1.5f, false, 1f ),
                new ( 1.5f, false, 1f ),
                new ( 1.5f, false, 1f ),
                new ( 1.5f, false, 1f ),
                new ( 1.5f, true, 7f ),
            },
            collectibleDatas : new ( ) {
                new ( -3, 3.7f ),
                new ( -3, 1 ),
                new ( -3, 1.1f ),
                new ( -3, 0.8f ),
                new ( -3, 1.3f ),
                new ( 3, 0 ),
                new ( 0 , 0 ),
                new ( -3, 0 ),
            }
        ),
    };

    private readonly List<NPESpawnPattern> _collectibleOnlyPatterns = new ( ) { 
        new NPESpawnPattern ( 
            obstacleDatas: new ( ) {
                new ( 3.5f, false, 0f ),
                new ( 7f, true, 2f ),
                new ( 5.5f, false, 0f ),
                new ( 5.5f, true, 2f ),
                new ( 6.5f, false, 0f ),
                new ( 4.5f, true, 2f ),
                new ( 6.5f, false, 0f ),
                new ( 4.5f, true, 3f ),
            },
            collectibleDatas : new ( ) {
                new ( 0.76f, 2 ),
                new ( -1.63f, 2 ),
                new ( 0.81f, 1.2f ),
                new ( -1.25f, 2 ),
                new ( 1.25f, 2 ),
            }
        ),
        
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
    [ SerializeField ] private List<ObstacleData> obstacleDatas;
    [ SerializeField ] private List<CollectibleData> collectibleDatas;

    public NPESpawnPattern ( List<ObstacleData> obstacleDatas, List<CollectibleData> collectibleDatas ) 
    {
        this.obstacleDatas = obstacleDatas;
        this.collectibleDatas = collectibleDatas;
    }

    public bool IsEmpty ( ) => !obstacleDatas.Any ( ) && !collectibleDatas.Any ( );

    public void Flip ( ) 
    {
        foreach ( var obstacleData in obstacleDatas ) 
            obstacleData.Flip ( );
            
        foreach ( var collectibleData in collectibleDatas ) 
            collectibleData.Flip ( );
    }

    public List<ObstacleData> ObstacleDatas => obstacleDatas;

    public List<CollectibleData> CollectibleDatas => collectibleDatas;
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

    public void Flip ( ) => isTop = !isTop;

    public float GapWidth => gapWidth;
    public bool IsTop => isTop;
    public float DelayAfterInSeconds => delayAfterInSeconds;
}


[ Serializable ]
public class CollectibleData 
{
    [ SerializeField ] [ Range ( -3.0f, 3.0f ) ] private float positionY;
    [ SerializeField ] [ Range ( 0f, 10f ) ] private float delayAfterInSeconds;

    public CollectibleData ( float positionY, float delayAfterInSeconds ) 
    {
        this.positionY = positionY;
        this.delayAfterInSeconds = delayAfterInSeconds;
    }

    public void Flip ( ) => positionY = ( 3.0f + ( -3.0f ) ) - positionY;

    public float PositionY => positionY;
    public float DelayAfterInSeconds => delayAfterInSeconds;
}
