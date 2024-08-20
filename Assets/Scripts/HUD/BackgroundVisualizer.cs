using UnityEngine;
using UnityEngine.UI;

public class BackgroundVisualizer : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private GameObject barPrefab;

    [ SerializeField ] [ Range ( 0, 500 ) ] private int barCount;

    [ SerializeField ] [ Range ( 0, 255 ) ] private int barOpacity;

    [ SerializeField ] [ Range ( 0, 1000 ) ] private float barMaxHeight;

    [ SerializeField ] [ Range ( 0, 5 ) ] private float smoothingValue;

    [ SerializeField ] [ Range ( 0, 500 ) ] private float barHeightMultiplier;
    
    [ SerializeField ] [ Range ( 0, 20000 ) ] private float normalizerCoeff;

    private RectTransform [ ] _bars;

    private float [ ] _spectrumData;

    private float [ ] _smoothedValues;

    private AudioSource _currentTrack;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameManager.OnGameStartAction += Initialize;

        SoundManager.VisualizeTrackAction += StartTrack;
        SoundManager.StopVisualizingTrackAction += StopTrack;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction -= Initialize;

        SoundManager.VisualizeTrackAction -= StartTrack;
        SoundManager.StopVisualizingTrackAction -= StopTrack;
    }
    
    private void Initialize ( ) 
    {
        _spectrumData = new float [ Mathf.Min ( ( int ) Mathf.Pow ( 2, ( int ) Mathf.Log ( barCount + 1, 2 ) + 1 ), 64 ) ];
        _bars = new RectTransform [ barCount ];
        _smoothedValues = new float [ barCount ];

        for ( var i = 0; i < _bars.Length; i++ ) 
        {
            _bars [ i ] = Instantiate ( barPrefab, transform ).GetComponent<RectTransform> ( );
            _bars [ i ].GetComponent<Image> ( ).color = new Color ( Constants.ThemeOrangeColor.r, Constants.ThemeOrangeColor.g, Constants.ThemeOrangeColor.b, barOpacity / 255.0f );
        }
    }

    private void StartTrack ( AudioSource track ) 
    {
        _currentTrack = track;
    }

    private void StopTrack ( ) 
    {
        _currentTrack = null;

        if ( _bars != null ) 
        {
            foreach ( var bar in _bars ) 
            {
                if ( bar != null ) 
                    Destroy ( bar.gameObject );
            }
            
            _bars = null;
        }
        
        _spectrumData = null;
        _smoothedValues = null;
    }

    private void FixedUpdate ( ) 
    {
        if ( _currentTrack != null ) 
        {
            _currentTrack.GetSpectrumData ( _spectrumData, 0, FFTWindow.Blackman );

            var groupSize = Mathf.FloorToInt ( _spectrumData.Length / _bars.Length );
            for ( var i = 0; i < _bars.Length; i++ ) 
            {
                float averageValue = 0f;
                for ( var j = 0; j < groupSize; j++ ) 
                    averageValue += _spectrumData [ Mathf.Clamp ( i * groupSize + j, 0, _spectrumData.Length - 1 ) ];
                averageValue /= groupSize;

                _smoothedValues [ i ] = Mathf.Lerp ( 
                    _smoothedValues [ i ], 
                    Mathf.Clamp ( Mathf.Log ( ( averageValue * normalizerCoeff ) + 1 ) * barHeightMultiplier, 0, barMaxHeight ), 
                    smoothingValue 
                );

                _bars [ i ].sizeDelta = new Vector2 ( _bars [ i ].sizeDelta.x, _smoothedValues [ i ] );
            }
        }
    }

    #endregion
}
