using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public Projectile projectile;
    public Vector2 velocity;
    private bool spawnedThisFrame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        spawnedThisFrame = false;
    }

    public void Fire(PlayerScript player)
    {
        if (!spawnedThisFrame)
        {
            Projectile newProjectile = GameObject.Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, 0));
            newProjectile.transform.localScale = transform.localScale;
            newProjectile.owner = player;
            newProjectile.move = player.fighter.currentMove;
            newProjectile.velocity = new Vector2(velocity.x * player.direction, velocity.y);
        }
        spawnedThisFrame = true;
    }
}
