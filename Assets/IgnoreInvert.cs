using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreInvert : MonoBehaviour
{
    [HideInInspector]
    public PlayerScript player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player ? player.direction != 0 : false)
        {
            transform.localScale = new Vector3(player.direction, transform.localScale.y, transform.localScale.z);
        }
    }

    void LateUpdate()
    {
        if (player ? player.direction != 0 : false)
        {
            transform.localScale = new Vector3(player.direction, transform.localScale.y, transform.localScale.z);
        }
    }
}
