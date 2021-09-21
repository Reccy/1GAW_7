using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour
{
    [SerializeField] private TrackSegment m_currentSegment;
    [SerializeField] private float m_currentD = 0;
    private float m_currentT = 0;
    private float m_dSpeed = 0.0f;

    public bool IsMovingForward => m_dSpeed > 0;
    public bool IsStationary => m_dSpeed == 0;
    public bool IsMovingBackward => m_dSpeed < 0;

    private void OnValidate()
    {
        if (m_currentSegment == null)
            return;

        UpdatePosition();
    }

    private void Awake()
    {
        Debug.Log(m_currentSegment.Length);
    }

    public void Accelerate()
    {
        m_dSpeed += 5 * Time.deltaTime;
    }

    public void Decelerate()
    {
        m_dSpeed -= 5 * Time.deltaTime;
    }

    public void Brake()
    {
        if (IsMovingForward)
        {
            Decelerate();

            if (IsMovingBackward)
            {
                m_dSpeed = 0;
            }
            return;
        }

        if (IsMovingBackward)
        {
            Accelerate();

            if (IsMovingForward)
            {
                m_dSpeed = 0;
            }
            return;
        }
    }

    private void FixedUpdate()
    {
        m_currentD += m_dSpeed * Time.deltaTime;

        if (IsMovingForward)
        {
            while (m_currentD >= m_currentSegment.Length)
            {
                m_currentD = Mathf.Abs(m_currentSegment.Length - m_currentD);
                m_currentSegment = m_currentSegment.Next;
            }
        }

        if (IsMovingBackward)
        {
            while (m_currentD <= 0)
            {
                m_currentSegment = m_currentSegment.Prev;
                m_currentD = Mathf.Abs(m_currentSegment.Length - m_currentD);
            }
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 pos2 = m_currentSegment.PointD(m_currentD);
        transform.position = new Vector3(pos2.x, pos2.y, transform.position.z);

        m_currentT = m_currentSegment.T(m_currentD);

        Vector2 lookDir = m_currentSegment.Tangent(m_currentT);

        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir);
    }
}
