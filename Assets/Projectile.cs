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
    public float hitstop;
    public bool faceDirection = true;
    public Hitbox[] hitboxes;
    public Move move;

    // Start is called before the first frame update
    public virtual void Start()
    {
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.player = owner;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        lifetime -= Time.deltaTime;
        hitstop -= Time.deltaTime;
        if (faceDirection && velocity != Vector2.zero)
        {
            Vector2 dir = velocity * new Vector2(1, 1);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 90, 0);
            transform.Rotate(0, 0, angle, Space.World);
        }

        if (hitstop <= 0)
        {
            TestForHits();
        }
    }

    public virtual void Explode()
    {
        if (spawnOnHit != null)
        {
            GameObject newObject = GameObject.Instantiate(spawnOnHit, transform.position, Quaternion.identity);
            newObject.transform.localScale = transform.localScale;
        }
        Destroy(gameObject);
    }

    public void TestForHits()
    {
        if (hitboxes.Length == 0)
        {
            return;
        }



        List<Entity> hitEntities = new List<Entity>();
        foreach (Hitbox hitbox in hitboxes)
        {
            if (hitbox.isActiveAndEnabled)
            {
                List<Entity> currentHitEntities = hitbox.Attack(owner);
                foreach (Entity entity in currentHitEntities)
                {
                    if (!hitEntities.Contains(entity))
                    {
                        print(entity.gameObject);
                        hitEntities.Add(entity);
                        hitstop = move.hitboxes[hitbox.hitboxNum].hitstop;
                        entity.HitboxDamage(move.hitboxes[hitbox.hitboxNum], owner, transform.position, owner.direction, hitbox.transform.eulerAngles.x);
                        if (!entity.invincible)
                        {
                            owner.fighter.OnHit(move.name);
                        }
                    }
                }
            }
        }
    }
}
