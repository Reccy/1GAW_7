using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class TrackJunctionIndicator : MonoBehaviour
{
    private Polyline m_line;

    private List<Vector2> m_points;

    public enum Direction { WITH_TRACK, AGAINST_TRACK }
    public Direction direction = Direction.WITH_TRACK;
    public bool FacingWithTrack => direction == Direction.WITH_TRACK;
    public bool FacingAgainstTrack => direction == Direction.AGAINST_TRACK;

    public TrackSegment prev;
    private void Awake()
    {
        m_line = GetComponent<Polyline>();
        m_points = new List<Vector2>();
        m_points.Add(Vector2.zero);
        m_points.Add(Vector2.zero);

        m_line.Closed = false;
    }

    private void FixedUpdate()
    {
        Point();
    }

    private void Point()
    {
        if (FacingWithTrack)
        {
            m_points[0] = prev.Point(1.0f);
            m_points[1] = prev.Next.PointDist(1.5f);
        }
        else
        {
            m_points[0] = prev.Point(0.0f);
            m_points[1] = prev.Prev.PointDist(prev.Prev.Length - 1.5f);
        }

        m_line.SetPoints(m_points);
    }
}
