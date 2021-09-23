using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
 
[SelectionBase]
public class TrackSegment : MonoBehaviour
{
    [SerializeField] private Color m_trackColor = Color.white;
    
    [Range(0, 2)]
    [SerializeField] private float m_minSpokeDistance = 0.4f;

    [Range(0, 2)]
    [SerializeField] private float m_maxSpokeDistance = 1.2f;
    
    [Range(-30, 0)]
    [SerializeField] private float m_minSpokeAngle = -10.0f;

    [Range(0, 30)]
    [SerializeField] private float m_maxSpokeAngle = 10.0f;

    [SerializeField] private TrackSegment m_previous;
    [SerializeField] private TrackSegment m_previousAlt;
    [SerializeField] private TrackSegment m_next;
    [SerializeField] private TrackSegment m_nextAlt;
    private BezierCurve m_curve;

    private enum JunctionSetting { NORMAL, ALT };
    public bool NextIsNormal => m_nextJunction == JunctionSetting.NORMAL;
    public bool NextIsAlt => m_nextJunction == JunctionSetting.ALT;
    public bool PrevIsNormal => m_prevJunction == JunctionSetting.NORMAL;
    public bool PrevIsAlt => m_prevJunction == JunctionSetting.ALT;

    [SerializeField] private JunctionSetting m_nextJunction = JunctionSetting.NORMAL;
    [SerializeField] private JunctionSetting m_prevJunction = JunctionSetting.NORMAL;

    public TrackSegment PrevNormal => m_previous;
    public TrackSegment PrevAlt => m_previousAlt;
    public TrackSegment NextNormal => m_next;
    public TrackSegment NextAlt => m_nextAlt;

    public bool HasNextSegment(TrackSegment other) => NextNormal == other || NextAlt == other;
    public bool HasPrevSegment(TrackSegment other) => PrevNormal == other || PrevAlt == other;

    private const float TRACK_WIDTH = 0.45f;
    private const float TRACK_THICKNESS = 0.05f;
    [SerializeField] private TrackJunctionIndicator m_trackJunctionPrefab;
    [SerializeField] private GameObject m_trackSpokePrefab;

    public TrackSegment Next
    {
        get
        {
            if (!NextIsJunction)
                return m_next;

            if (NextIsNormal)
            {
                return m_next;
            }
            else
            {
                return m_nextAlt;
            }
        }
    }

    public TrackSegment Prev
    {
        get
        {
            if (!PrevIsJunction)
                return m_previous;

            if (PrevIsNormal)
            {
                return m_previous;
            }
            else
            {
                return m_previousAlt;
            }
        }
    }

    public TrackJunction NextJunction()
    {
        TrackSegment segment = this;
        TrackJunction junction = new TrackJunction();

        while (!segment.NextIsJunction)
        {
            segment = segment.Next;
        }

        junction.From = segment;
        junction.Position = segment.Point(1);
        junction.TValue = 1.0f;

        return junction;
    }

    public TrackJunction PrevJunction()
    {
        TrackSegment segment = this;
        TrackJunction junction = new TrackJunction();

        while (!segment.PrevIsJunction)
        {
            segment = segment.Prev;
        }

        junction.From = segment;
        junction.Position = segment.Point(0);
        junction.TValue = 0.0f;

        return junction;
    }

    public void SwitchNext()
    {
        if (m_nextAlt == null)
            Debug.LogWarning("Switching on an non-junction");

        if (NextIsNormal)
        {
            m_nextJunction = JunctionSetting.ALT;
        }
        else
        {
            m_nextJunction = JunctionSetting.NORMAL;
        }
    }

    public void SwitchPrev()
    {
        if (m_previousAlt == null)
            Debug.LogWarning("Switching on an non-junction");

        if (PrevIsNormal)
        {
            m_prevJunction = JunctionSetting.ALT;
        }
        else
        {
            m_prevJunction = JunctionSetting.NORMAL;
        }
    }

    public bool NextIsJunction => m_nextAlt != null;
    public bool PrevIsJunction => m_previousAlt != null;

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
        // Setup track
        m_curve = new BezierCurve(m_pointA, m_pointB, m_pointC, m_pointD);

        GameObject leftTrackObj = new GameObject();
        leftTrackObj.transform.parent = gameObject.transform;
        leftTrackObj.name = "LeftTrackPolyline";
        GameObject rightTrackObj = new GameObject();
        rightTrackObj.transform.parent = gameObject.transform;
        rightTrackObj.name = "RightTrackPolyline";

        Shapes.Polyline leftTrackLine = leftTrackObj.AddComponent<Shapes.Polyline>();
        Shapes.Polyline rightTrackLine = rightTrackObj.AddComponent<Shapes.Polyline>();
        leftTrackLine.Closed = false;
        leftTrackLine.Color = m_trackColor;
        leftTrackLine.Thickness = TRACK_THICKNESS;
        rightTrackLine.Closed = false;
        rightTrackLine.Color = m_trackColor;
        rightTrackLine.Thickness = TRACK_THICKNESS;

        List<Vector2> leftPoints = new List<Vector2>();
        List<Vector2> rightPoints = new List<Vector2>();

        for (int i = 0; i <= m_detailLevel; ++i)
        {
            float t = (float)i / (float)m_detailLevel;

            Vector2 origin = m_curve.Point(t);
            Vector2 right = origin + m_curve.Normal(t) * (TRACK_WIDTH * 0.5f);
            Vector2 left = origin - m_curve.Normal(t) * (TRACK_WIDTH * 0.5f);

            leftPoints.Add(transform.InverseTransformPoint(left));
            rightPoints.Add(transform.InverseTransformPoint(right));
        }

        float d = Random.Range(m_minSpokeDistance, m_maxSpokeDistance);

        while (d < Length)
        {
            Vector2 p = PointDist(d);
            Vector2 forward = TangentDist(d);

            float rot = Random.Range(m_minSpokeAngle, m_maxSpokeAngle);
            forward = forward.RotatedDeg(rot);

            GameObject spoke = Instantiate(m_trackSpokePrefab);
            spoke.transform.parent = gameObject.transform;
            spoke.transform.position = p;
            spoke.transform.right = forward;

            float sum = Random.Range(m_minSpokeDistance, m_maxSpokeDistance);

            d += sum;
        }

        leftTrackLine.SetPoints(leftPoints);
        rightTrackLine.SetPoints(rightPoints);

        // Setup Junctions
        if (NextIsJunction)
        {
            TrackJunctionIndicator junctionObj = Instantiate(m_trackJunctionPrefab);
            junctionObj.direction = TrackJunctionIndicator.Direction.WITH_TRACK;
            junctionObj.prev = this;
        }

        if (PrevIsJunction)
        {
            TrackJunctionIndicator junctionObj = Instantiate(m_trackJunctionPrefab);
            junctionObj.direction = TrackJunctionIndicator.Direction.AGAINST_TRACK;
            junctionObj.prev = this;
        }
    }

    public float Length => m_curve.Length;

    public Vector2 Tangent(float t)
    {
        return m_curve.Tangent(t);
    }

    public Vector2 TangentDist(float t)
    {
        return m_curve.TangentDist(t);
    }

    public Vector2 Normal(float t)
    {
        return m_curve.Normal(t);
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

    public Vector2 PointDist(float dist)
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
