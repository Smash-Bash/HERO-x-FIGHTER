using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarJudgement : MonoBehaviour
{
    public BasicProjectile projectile;
    public Hitbox hitbox;
    public int multiHits;

    // Start is called before the first frame update
    void Start()
    {
        multiHits = Mathf.RoundToInt(transform.localScale.y * 2.75f) - 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitbox.currentlyHitThings.Count != 0 && projectile.hitstop < 0)
        {
            if (multiHits > 0)
            {
                multiHits--;
                foreach (GameObject hitObject in hitbox.currentlyHitThings)
                {
                    transform.position = Vector3.MoveTowards(transform.position, hitObject.transform.position, 0.125f);
                }
                hitbox.currentlyHitThings.Clear();
                transform.position += new Vector3(projectile.velocity.x * 2, projectile.velocity.y * 2, 0) * Time.deltaTime;
                projectile.velocity = Vector3.up * 5;
            }
            else
            {
                projectile.Explode();
            }
        }
    }

    void LateUpdate()
    {
        if (hitbox.currentlyHitThings.Count != 0 && projectile.hitstop < 0)
        {
            if (multiHits > 0)
            {
                multiHits--;
                hitbox.currentlyHitThings.Clear();
                transform.position += new Vector3(projectile.velocity.x * 2, projectile.velocity.y * 2, 0) * Time.deltaTime;
            }
            else
            {
                projectile.Explode();
            }
        }
    }
}
