using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
public class TrackSegment : MonoBehaviour
{
    [SerializeField] private TrackSegment m_previous;
    [SerializeField] private TrackSegment m_previousAlt;
    [SerializeField] private TrackSegment m_next;
    [SerializeField] private TrackSegment m_nextAlt;
    private BezierCurve m_curve;

    public TrackSegment Prev => m_previous;
    public TrackSegment PrevAlt => m_previousAlt;
    public TrackSegment Next => m_next;
    public TrackSegment NextAlt => m_nextAlt;

    public bool IsJunction => m_nextAlt != null;

    [SerializeField] private GameObject m_trackSegmentPrefab;
    [SerializeField] private BezierPoint m_pointA;
    [SerializeField] private BezierPoint m_pointB;
    [SerializeField] private BezierPoint m_pointC;
    [SerializeField] private BezierPoint m_pointD;
    [SerializeField] private int m_detailLevel = 32;

#if UNITY_EDITOR
    [InspectorButton("CreateNextTrackSegment", ButtonWidth = 128)] public bool addSegment;
    [InspectorButton("CreateNextAltTrackSegment", ButtonWidth = 128)] public bool addAltSegment;
#endif

    private void Awake()
    {
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);
        
        Shapes.Polyline mainPolyline = gameObject.AddComponent<Shapes.Polyline>();
        mainPolyline.Closed = false;

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i <= m_detailLevel; ++i)
        {
            float t = (float)i / (float)m_detailLevel;

            points.Add(transform.InverseTransformPoint(m_curve.Point(t)));
        }

        mainPolyline.SetPoints(points);
    }

    public float Length => m_curve.Length;

    public Vector2 Tangent(float t)
    {
        return m_curve.Tangent(t);
    }

    public float T(float dist)
    {
        return m_curve.T(dist);
    }

    public float Distance(float t)
    {
        return m_curve.Distance(t);
    }

    public Vector2 Center()
    {
#if UNITY_EDITOR
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);
#endif
        return m_curve.Point(0.5f);
    }

    public Vector2 Point(float t)
    {
#if UNITY_EDITOR
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);
#endif
        return m_curve.Point(t);
    }

    public Vector2 PointD(float dist)
    {
#if UNITY_EDITOR
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);
#endif

        return m_curve.PointDist(dist);
    }

#if UNITY_EDITOR
    private void CreateNextTrackSegment()
    {
        if (m_next != null)
            return;

        GameObject nextObj = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(m_trackSegmentPrefab)) as GameObject;

        Debug.Log(nextObj);

        TrackSegment next = nextObj.GetComponent<TrackSegment>();
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

    private void CreateNextAltTrackSegment()
    {
        if (m_nextAlt != null)
            return;

        GameObject nextObj = PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(m_trackSegmentPrefab)) as GameObject;

        Debug.Log(nextObj);

        TrackSegment next = nextObj.GetComponent<TrackSegment>();
        next.transform.parent = transform.parent;
        m_nextAlt = next;
        m_nextAlt.m_previousAlt = this;

        Vector2 dir = (m_pointD.Point - m_pointC.Point).normalized;

        m_nextAlt.m_pointA.transform.position = m_pointD.transform.position;
        m_nextAlt.m_pointB.transform.position = (Vector2)m_nextAlt.m_pointA.transform.position + dir * 1;
        m_nextAlt.m_pointC.transform.position = (Vector2)m_nextAlt.m_pointA.transform.position + dir * 2;
        m_nextAlt.m_pointD.transform.position = (Vector2)m_nextAlt.m_pointA.transform.position + dir * 3;

        Selection.activeGameObject = m_nextAlt.gameObject;
    }

    private void OnDrawGizmos()
    {
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);

        Color c = Color.red;

        if (Selection.activeGameObject == gameObject)
        {
            c = Color.green;
        }

        for (int i = 1; i <= m_detailLevel; ++i)
        {
            float p1 = (float)(i - 1) / (float)m_detailLevel;
            float p2 = (float)i / (float)m_detailLevel;

            Debug2.DrawArrow(m_curve.Point(p1), m_curve.Point(p2), c, 0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(m_pointA.Point, 0.1f);
        Gizmos.DrawWireSphere(m_pointB.Point, 0.1f);
        Gizmos.DrawWireSphere(m_pointC.Point, 0.1f);
        Gizmos.DrawWireSphere(m_pointD.Point, 0.1f);

        Gizmos.DrawLine(m_pointA.Point, m_pointB.Point);
        Gizmos.DrawLine(m_pointC.Point, m_pointD.Point);

        float length = m_curve.Length;
        float accumulated = 0;
        float step = 0.5f;

        while (accumulated < length)
        {
            Gizmos.DrawWireSphere(m_curve.PointDist(accumulated), 0.25f);

            accumulated += step;
        }
    }
#endif
}
