using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    private LineSegment m_lineA;
    private LineSegment m_lineB;
    private LineSegment m_lineC;

    private LineSegment m_lineD;
    private LineSegment m_lineE;
    
    private LineSegment m_lineF;

    private Vector2 m_pointA;
    private Vector2 m_pointB;
    private Vector2 m_pointC;
    private Vector2 m_pointD;

    public BezierCurve(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        m_pointA = pointA;
        m_pointB = pointB;
        m_pointC = pointC;
        m_pointD = pointD;

        Init(pointA, pointB, pointC, pointD);
    }

    public BezierCurve(BezierPoint pointA, BezierPoint pointB, BezierPoint pointC, BezierPoint pointD)
    {
        m_pointA = pointA.Point;
        m_pointB = pointB.Point;
        m_pointC = pointC.Point;
        m_pointD = pointD.Point;

        pointA.OnTransformChanged += OnBezierPointAChanged;
        pointB.OnTransformChanged += OnBezierPointBChanged;
        pointC.OnTransformChanged += OnBezierPointCChanged;
        pointD.OnTransformChanged += OnBezierPointDChanged;

        Init(pointA.Point, pointB.Point, pointC.Point, pointD.Point);
    }

    private void Init(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        m_lineA = new LineSegment(pointA, pointB);
        m_lineB = new LineSegment(pointB, pointC);
        m_lineC = new LineSegment(pointC, pointD);

        m_lineD = new LineSegment(pointA, pointB);
        m_lineE = new LineSegment(pointB, pointC);

        m_lineF = new LineSegment(pointA, pointB);
    }

    private void OnBezierPointAChanged(BezierPoint point)
    {
        m_pointA = point.Point;

        m_lineA.Begin = m_pointA;
    }

    private void OnBezierPointBChanged(BezierPoint point)
    {
        m_pointB = point.Point;

        m_lineA.End = m_pointB;
        m_lineB.Begin = m_pointB;
    }

    private void OnBezierPointCChanged(BezierPoint point)
    {
        m_pointC = point.Point;

        m_lineB.End = m_pointC;
        m_lineC.Begin = m_pointC;
    }
    
    private void OnBezierPointDChanged(BezierPoint point)
    {
        m_pointD = point.Point;

        m_lineC.End = m_pointD;
    }

    public Vector2 Point(float t)
    {
        m_lineD.Begin = m_lineA.Point(t);
        m_lineD.End = m_lineB.Point(t);

        m_lineE.Begin = m_lineB.Point(t);
        m_lineE.End = m_lineC.Point(t);

        m_lineF.Begin = m_lineD.Point(t);
        m_lineF.End = m_lineE.Point(t);

        return m_lineF.Point(t);
    }
}
