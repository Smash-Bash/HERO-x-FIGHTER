using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    public Transform target;
    public bool worldSpace;

    void Update()
    {
        if (target != null)
        {
            if (worldSpace)
            {
                transform.rotation = target.rotation;
            }
            else
            {
                transform.localRotation = target.localRotation;
            }
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (worldSpace)
            {
                transform.rotation = target.rotation;
            }
            else
            {
                transform.localRotation = target.localRotation;
            }
        }
    }
}
