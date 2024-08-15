using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ Serializable ]
public class SerializedDictionary<T1, T2> 
{
    [ SerializeField ] private SerializedKeyValuePair<T1, T2> [ ] pairs;

    public SerializedKeyValuePair<T1, T2> [ ] Pairs => pairs;
    
    public SerializedDictionary ( Dictionary<T1, T2> dictionary ) 
    {
        pairs = dictionary.Select ( keyValuePair => new SerializedKeyValuePair<T1, T2> ( keyValuePair.Key, keyValuePair.Value ) ).ToArray ( );
    }

    public SerializedDictionary ( SerializedDictionary<T1, T2> otherSerializedDictionary ) 
    {
        pairs = new SerializedKeyValuePair<T1, T2 > [ otherSerializedDictionary.Pairs.Length ];
        
        for ( var i = 0; i < otherSerializedDictionary.Pairs.Length; i++ ) 
            pairs [ i ] = new SerializedKeyValuePair<T1, T2> ( otherSerializedDictionary.Pairs [ i ].Key, otherSerializedDictionary.Pairs [ i ].Value );
    }

    public Dictionary<T1, T2> ToDictionary ( ) => pairs.ToDictionary ( pair => pair.Key, pair => pair.Value );
}

[ Serializable ]
public class SerializedKeyValuePair<T1, T2> 
{
    [ SerializeField ] private T1 key;

    [ SerializeField ] private T2 value;

    public T1 Key => key;

    public T2 Value => value;

    public SerializedKeyValuePair ( T1 key, T2 value )
    {
        this.key = key;
        this.value = value;
    }
}