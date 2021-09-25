using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TrackJunction
{
    public Vector2 Position;
    public bool Null;
    public TrackSegment From;
    public float TValue;
    public TrackJunctionIndicator Indicator;
    
    public static TrackJunction BuildNull()
    {
        TrackJunction nullJunction = new TrackJunction();
        nullJunction.Null = true;
        return nullJunction;
    }
}
