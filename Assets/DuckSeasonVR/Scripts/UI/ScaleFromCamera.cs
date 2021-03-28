using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFromCamera : MonoBehaviour
{
    public float maxScale = 1.0f;

    Transform cam;
    Vector3 defaultScale;

    void Start()
    {
        cam = Camera.main.transform;
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Plane plane = new Plane(cam.forward, cam.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        float d = Vector3.Distance(cam.position, transform.position);
        if (d > 8f)
        {
            transform.localScale = defaultScale * dist * maxScale;
        }
        else
        {
            transform.localScale = defaultScale;
        }
        
    }
}
