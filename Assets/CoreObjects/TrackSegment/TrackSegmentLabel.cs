using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackSegmentLabel : MonoBehaviour
{
    private TrackSegment m_trackSegment;

    private static int m_count = 0;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            GetComponentInParent<TrackSegment>().name = $"TrackSegment({m_count++})";
            Destroy(gameObject);
        }

        m_trackSegment = GetComponentInParent<TrackSegment>();
    }

    private void Update()
    {
        transform.position = m_trackSegment.Center();
        gameObject.name = m_trackSegment.name;
    }
}
