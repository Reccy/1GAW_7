using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrackSegment : MonoBehaviour
{
    [SerializeField] private TrackSegment m_previous;
    [SerializeField] private TrackSegment m_next;
    private BezierCurve m_curve;

    [SerializeField] private TrackSegment m_trackSegmentPrefab;
    [SerializeField] private BezierPoint m_pointA;
    [SerializeField] private BezierPoint m_pointB;
    [SerializeField] private BezierPoint m_pointC;
    [SerializeField] private BezierPoint m_pointD;
    [SerializeField] private int m_points = 16;

#if UNITY_EDITOR
    [InspectorButton("CreateNextTrackSegment")] public bool click;
#endif

    private void Awake()
    {
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);
        var p = gameObject.AddComponent<Shapes.Polyline>();
        p.Closed = false;

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i <= m_points; ++i)
        {
            float t = (float)i / (float)m_points;

            points.Add(transform.InverseTransformPoint(m_curve.Point(t)));
        }

        p.SetPoints(points);
    }

#if UNITY_EDITOR
    private void CreateNextTrackSegment()
    {
        if (m_next != null)
            return;

        TrackSegment next = Instantiate(m_trackSegmentPrefab);
        next.transform.parent = transform.parent;
        m_next = next;
        m_next.m_previous = this;

        Vector2 dir = (m_pointD.Point - m_pointC.Point).normalized;

        m_next.m_pointA.transform.position = m_pointD.transform.position;
        m_next.m_pointB.transform.position = (Vector2)m_next.m_pointA.transform.position + dir * 1;
        m_next.m_pointC.transform.position = (Vector2)m_next.m_pointA.transform.position + dir * 2;
        m_next.m_pointD.transform.position = (Vector2)m_next.m_pointA.transform.position + dir * 3;

        Selection.activeGameObject = m_next.gameObject;
    }
#endif

    private void OnDrawGizmos()
    {
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);

        for (int i = 1; i <= m_points; ++i)
        {
            float p1 = (float)(i - 1) / (float)m_points;
            float p2 = (float)i / (float)m_points;

            Debug2.DrawArrow(m_curve.Point(p1), m_curve.Point(p2), Color.red);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_pointA.Point, 0.1f);
        Gizmos.DrawWireSphere(m_pointB.Point, 0.1f);
        Gizmos.DrawWireSphere(m_pointC.Point, 0.1f);
        Gizmos.DrawWireSphere(m_pointD.Point, 0.1f);
    }
}
