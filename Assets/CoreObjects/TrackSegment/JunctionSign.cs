using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunctionSign : MonoBehaviour
{
    [SerializeField] private Transform m_arrow;
    [SerializeField] private Transform m_arrowDeselected;
    [SerializeField] private GameObject m_selectedGlow;

    public Vector2 dirToNormal;
    public Vector2 dirToAlt;

    public void Select()
    {
        m_selectedGlow.SetActive(true);
    }

    public void Deselect()
    {
        m_selectedGlow.SetActive(false);
    }

    public void ShowNormal()
    {
        m_arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToNormal);
        m_arrowDeselected.transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToAlt);
    }

    public void ShowAlt()
    {
        m_arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToAlt);
        m_arrowDeselected.transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToNormal);
    }
}
