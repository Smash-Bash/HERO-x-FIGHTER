using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public Projectile projectile;
    public Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(PlayerScript player)
    {
        Projectile newProjectile = GameObject.Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, 0));
        newProjectile.owner = player;
        newProjectile.velocity = new Vector2(velocity.x * player.direction, velocity.y);
    }
}
