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
        GameManager.OnGameEndAction += Clear;

        SoundManager.OnPlayTrackAction += StartTrack;
        SoundManager.OnStopTrackAction += StopTrack;
    }

    private void OnDestroy ( ) 
    {
        GameManager.OnGameStartAction -= Initialize;
        GameManager.OnGameEndAction -= Clear;

        SoundManager.OnPlayTrackAction -= StartTrack;
        SoundManager.OnStopTrackAction -= StopTrack;
    }

    private void Start ( ) 
    { 
        if ( Application.platform == RuntimePlatform.WebGLPlayer ) 
        {
            barCount = 50;
            barHeightMultiplier = 110;
            normalizerCoeff = 15;
            smoothingValue = 0.07f;
        }
    }
    
    private void Initialize ( ) 
    {
        _bars = new RectTransform [ barCount ];
        for ( var i = 0; i < _bars.Length; i++ ) 
        {
            _bars [ i ] = Instantiate ( barPrefab, transform ).GetComponent<RectTransform> ( );
            _bars [ i ].GetComponent<Image> ( ).color = new Color ( Constants.ThemeOrangeColor.r, Constants.ThemeOrangeColor.g, Constants.ThemeOrangeColor.b, barOpacity / 255.0f );
        }
        
        _spectrumData = new float [ Mathf.Max ( ( int ) Mathf.Pow ( 2, ( int ) Mathf.Log ( barCount + 1, 2 ) + 1 ), 64 ) ];
        _smoothedValues = new float [ barCount ];
    }

    private void Clear ( ) 
    {
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

    private void StartTrack ( Music track ) => _currentTrack = track.Source;

    private void StopTrack ( ) => _currentTrack = null;

    private void FixedUpdate ( ) 
    {
        if ( _currentTrack != null ) 
        {
            if ( Application.platform == RuntimePlatform.WebGLPlayer ) 
                FillSpectrumDataWithDummyData ( );
            else 
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

    private void FillSpectrumDataWithDummyData ( ) 
    {
        var amplitude = 0.5f;
        var frequency = 1f;
        var noiseFactor = 0.2f;

        for ( var i = 0; i < _spectrumData.Length; i++ ) 
        {
            float baseValue = Mathf.Sin ( Time.time * frequency + i * 0.5f ) * amplitude;
            float noise = Random.Range ( -noiseFactor, noiseFactor );

            _spectrumData [ i ] = Mathf.Abs ( baseValue + noise );
        }
    }

    #endregion
}
