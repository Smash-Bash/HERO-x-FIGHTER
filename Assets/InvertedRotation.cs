using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertedRotation : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(target.eulerAngles.x * -1, target.eulerAngles.y * -1, target.eulerAngles.z * -1);
    }
}
