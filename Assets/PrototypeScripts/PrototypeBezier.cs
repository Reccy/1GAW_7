using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class PrototypeBezier : MonoBehaviour
{
    [SerializeField] private BezierPoint m_a;
    [SerializeField] private BezierPoint m_b;
    [SerializeField] private BezierPoint m_c;
    [SerializeField] private BezierPoint m_d;
    [SerializeField] private int m_size;

    private BezierCurve m_bezier;
    private Shapes.Polyline m_line;

    private List<Shapes.PolylinePoint> m_points;

    private void Awake()
    {
        m_bezier = new BezierCurve(m_a, m_b, m_c, m_d);
        m_line = GetComponent<Shapes.Polyline>();
        m_points = new List<Shapes.PolylinePoint>();
    }

    private void Update()
    {
        m_points = new List<Shapes.PolylinePoint>();

        for (int i = 0; i <= m_size; ++i)
        {
            Vector2 pos = m_bezier.Point((float)i / (float)m_size);
            m_points.Add(new Shapes.PolylinePoint(pos));
        }

        m_line.SetPoints(m_points);
    }
}
