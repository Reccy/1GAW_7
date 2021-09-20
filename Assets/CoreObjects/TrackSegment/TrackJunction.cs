using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackJunction : MonoBehaviour
{
    [SerializeField] private TrackSegment m_previous;
    [SerializeField] private TrackSegment m_nextLeft;
    [SerializeField] private TrackSegment m_nextRight;
}
