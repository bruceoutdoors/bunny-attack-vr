using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendezvousPoint : MonoBehaviour
{
    public RendezvousPoint[] Points;
    [HideInInspector]
    public Vector3 position { get { return transform.position; } }
    public Mesh duck;
    public bool tiltLeft;


    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Points.Length == 0)
        {
            Gizmos.DrawWireCube(position, new Vector3(0.5f, 0.5f, 0.5f));
            
            return;
        }

        if (duck != null)
        {
            Gizmos.DrawMesh(duck, position, Quaternion.LookRotation(Camera.main.transform.position - transform.position) * Quaternion.Euler(0, 0, tiltLeft ? -30 : 30));
        }

        foreach (RendezvousPoint p in Points)
        {
            DrawArrow.ForGizmo(position, p.position - position);
        }
#endif
    }

    public RendezvousPoint GetRendezvous()
    {
        if (Points.Length == 0) return null;

        return Points[Random.Range(0, Points.Length)];
    }

    public bool IsLastPoint()
    {
        return Points.Length == 0;
    }
}
