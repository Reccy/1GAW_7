using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackSegmentLabel : MonoBehaviour
{
    private TrackSegment m_trackSegment;

    private void Awake()
    {
        m_trackSegment = GetComponentInParent<TrackSegment>();
    }

    private void Update()
    {
        transform.position = m_trackSegment.Center();
        gameObject.name = m_trackSegment.name;
    }
}
