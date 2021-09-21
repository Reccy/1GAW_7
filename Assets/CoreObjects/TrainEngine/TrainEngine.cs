using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour
{
    [SerializeField] private TrackSegment m_currentSegment;
    [SerializeField] private float m_currentT = 0;
    public float m_currentD = 0;

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

    private void FixedUpdate()
    {
        m_currentT += Time.deltaTime;

        while (m_currentT >= 1)
        {
            m_currentSegment = m_currentSegment.Next;
            m_currentT -= 1;
        }

        m_currentD = m_currentSegment.Distance(m_currentT);

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 pos2 = m_currentSegment.Point(m_currentT);
        transform.position = new Vector3(pos2.x, pos2.y, transform.position.z);

        Vector2 lookDir = m_currentSegment.Tangent(m_currentT);

        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir);
    }
}
