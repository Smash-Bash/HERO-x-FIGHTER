using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public PlayerScript owner;
    public GameObject spawnOnHit;
    public Vector2 velocity;
    public float lifetime;
    public bool faceDirection = true;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        lifetime -= Time.deltaTime;
        if (faceDirection && velocity != Vector2.zero)
        {
            Vector2 dir = velocity * new Vector2(1, 1);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 90, 0);
            transform.Rotate(0, 0, angle, Space.World);
        }
    }

    public virtual void Explode()
    {
        Destroy(gameObject);
    }
}
