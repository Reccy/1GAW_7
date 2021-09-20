using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class BezierPoint : MonoBehaviour
{
    public Vector2 Point => transform.position;

    public delegate void TransformChanged(BezierPoint point);
    public TransformChanged OnTransformChanged;

    private void Update()
    {
        if (transform.hasChanged)
        {
            if (OnTransformChanged != null)
            {
                OnTransformChanged(this);
            }
        }
    }
}
