using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class TrackJunctionIndicator : MonoBehaviour
{
    [SerializeField] private JunctionSign m_signPrefab;
    private JunctionSign m_sign;
    public JunctionSign Sign => m_sign;

    public enum Direction { WITH_TRACK, AGAINST_TRACK }
    public Direction direction = Direction.WITH_TRACK;
    public bool FacingWithTrack => direction == Direction.WITH_TRACK;
    public bool FacingAgainstTrack => direction == Direction.AGAINST_TRACK;


    public TrackSegment prev;
    private void Start()
    {
        m_sign = Instantiate(m_signPrefab);
        float z = m_sign.transform.position.z;

        Vector2 signPos;
        Vector2 tangent;
        Vector2 normalArrowDir;
        Vector2 altArrowDir;

        if (FacingWithTrack)
        {
            signPos = prev.Point(1.0f) - prev.Normal(1.0f) * TrackSegment.TRACK_WIDTH;
            tangent = prev.Tangent(1.0f);

            normalArrowDir = (prev.Next.Point(0.5f) - prev.Point(0.5f)).normalized;
            altArrowDir = (prev.NextAlt.Point(0.5f) - prev.Point(0.5f)).normalized;
        }
        else
        {
            signPos = prev.Point(0.0f) - prev.Normal(0.0f) * TrackSegment.TRACK_WIDTH;
            tangent = prev.Tangent(0.0f);

            normalArrowDir = (prev.Prev.Point(0.5f) - prev.Point(0.5f)).normalized;
            altArrowDir = (prev.PrevAlt.Point(0.5f) - prev.Point(0.5f)).normalized;
        }

        normalArrowDir = CardinalDirs.SortedBySimilarity(normalArrowDir)[0];
        altArrowDir = CardinalDirs.SortedBySimilarity(altArrowDir)[0];

        if (altArrowDir == normalArrowDir)
        {
            altArrowDir = CardinalDirs.SortedBySimilarity(altArrowDir)[1];
        }

        m_sign.dirToAlt = altArrowDir;
        m_sign.dirToNormal = normalArrowDir;

        m_sign.Deselect();

        m_sign.transform.position = new Vector3(signPos.x, signPos.y, z);
        transform.position = m_sign.transform.position;
    }

    private void FixedUpdate()
    {
        if (FacingWithTrack)
        {
            if (prev.NextIsNormal)
            {
                m_sign.ShowNormal();
            }
            else
            {
                m_sign.ShowAlt();
            }
        }
        else
        {
            if (prev.PrevIsNormal)
            {
                m_sign.ShowNormal();
            }
            else
            {
                m_sign.ShowAlt();
            }
        }
    }
}
