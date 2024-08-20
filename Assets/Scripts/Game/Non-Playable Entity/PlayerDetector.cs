using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    #region 

    private SoundType stingerType;

    #endregion

    #region Methods

    public void Initialize ( SoundType stingerType ) => this.stingerType = stingerType;

    private void OnTriggerEnter2D ( Collider2D other ) 
    {
        if ( other.GetComponent<Player> ( ) != null ) 
            SoundManager.Instance.Play ( stingerType );
    }

    #endregion
}
