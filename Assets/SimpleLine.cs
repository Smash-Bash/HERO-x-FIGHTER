using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLine : MonoBehaviour
{
    public LineRenderer line;
    public float noise;
    public Transform[] points;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (points.Length != 0)
        {
            line.positionCount = points.Length;
            int index = 0;
            foreach (Transform point in points)
            {
                if (point != null)
                {
                    line.SetPosition(index, point.position + new Vector3(Random.Range(noise, noise * -1), Random.Range(noise, noise * -1), Random.Range(noise, noise * -1)));
                    index++;
                }
            }
        }
    }

    public void LateUpdate()
    {
        if (points.Length != 0)
        {
            line.positionCount = points.Length;
            int index = 0;
            foreach (Transform point in points)
            {
                if (point != null)
                {
                    line.SetPosition(index, point.position + new Vector3(Random.Range(noise, noise * -1), Random.Range(noise, noise * -1), Random.Range(noise, noise * -1)));
                    index++;
                }
            }
        }
    }

    public void OnValidate()
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
        }
        if (points.Length != 0)
        {
            line.positionCount = points.Length;
            int index = 0;
            foreach (Transform point in points)
            {
                if (point != null)
                {
                    line.SetPosition(index, point.position);
                    index++;
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (points.Length != 0)
        {
            int index = 0;
            foreach (Transform point in points)
            {
                if (point != null)
                {
                    if (index + 1 < points.Length)
                    {
                        Gizmos.DrawLine(point.position, points[index + 1].position);
                    }
                    index++;
                }
            }
        }
    }
}
