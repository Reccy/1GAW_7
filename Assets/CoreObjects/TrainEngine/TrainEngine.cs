using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainEngine : MonoBehaviour
{
    [SerializeField] private TrackSegment m_currentSegment;
    [SerializeField] private float m_currentDistance = 0;
    private float m_currentT = 0;
    private float m_dSpeed = 0.0f;

    private enum Direction { WITH_TRACK, AGAINST_TRACK }
    [SerializeField] private Direction m_direction = Direction.WITH_TRACK;
    private bool IsFacingWithTrack => m_direction == Direction.WITH_TRACK;
    private bool IsFacingAgainstTrack => m_direction == Direction.AGAINST_TRACK;

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
        if (IsFacingWithTrack)
            m_currentDistance += m_dSpeed * Time.deltaTime;
        else
            m_currentDistance -= m_dSpeed * Time.deltaTime;

        if ((IsMovingForward && IsFacingWithTrack) || (IsMovingBackward && IsFacingAgainstTrack))
        {
            // Can't handle small segments at high speeds
            if (m_currentDistance > m_currentSegment.Length)
            {
                float carryover = m_currentDistance - m_currentSegment.Length;
                ChangeToTrackSegment(m_currentSegment.Next, carryover);
            }
        }
        else if ((IsMovingForward && IsFacingAgainstTrack) || (IsMovingBackward && IsFacingWithTrack))
        {
            // Can't handle small segments at high speeds
            if (m_currentDistance < 0)
            {
                float carryover = m_currentDistance;
                ChangeToTrackSegment(m_currentSegment.Prev, carryover);
            }
        }

        UpdatePosition();
    }

    private void ChangeToTrackSegment(TrackSegment transitioningSegment, float carryover)
    {
        if (IsFacingWithTrack)
        {
            if (IsMovingForward)
            {
                if (transitioningSegment.HasNextSegment(m_currentSegment))
                {
                    m_direction = Direction.AGAINST_TRACK;
                    m_currentDistance = transitioningSegment.Length - carryover;
                }
                else
                {
                    m_currentDistance = carryover;
                }
            }
            else if (IsMovingBackward)
            {
                if (transitioningSegment.HasPrevSegment(m_currentSegment))
                {
                    m_direction = Direction.AGAINST_TRACK;
                    m_currentDistance = carryover;
                }
                else
                {
                    m_currentDistance = transitioningSegment.Length - carryover;
                }
            }
        }
        else if (IsFacingAgainstTrack)
        {
            if (IsMovingForward)
            {
                if (transitioningSegment.HasPrevSegment(m_currentSegment))
                {
                    m_direction = Direction.WITH_TRACK;
                    m_currentDistance = carryover;
                }
                else
                {
                    m_currentDistance = transitioningSegment.Length - carryover;
                }
            }
            else if (IsMovingBackward)
            {
                if (transitioningSegment.HasNextSegment(m_currentSegment))
                {
                    m_direction = Direction.WITH_TRACK;
                    m_currentDistance = transitioningSegment.Length - carryover;
                }
                else
                {
                    m_currentDistance = carryover;
                }
            }
        }

        m_currentSegment = transitioningSegment;
    }

    private void UpdatePosition()
    {
        Vector2 pos2 = m_currentSegment.PointDist(m_currentDistance);

        transform.position = new Vector3(pos2.x, pos2.y, transform.position.z);

        m_currentT = m_currentSegment.T(m_currentDistance);

        Vector2 lookDir = GetLookDir();

        transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir);
    }

    private Vector2 GetLookDir()
    {
        if (IsFacingWithTrack)
        {
            return m_currentSegment.Tangent(m_currentT);
        }
        else
        {
            return -m_currentSegment.Tangent(m_currentT);
        }
    }
}
