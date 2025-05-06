using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    public bool shrinkOverLifetime;
    public bool explodeOnLifetimeEnd = true;
    public bool explodeOnTerrainHit = true;
    public Vector2 velocityOverLifetime;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        velocity += velocityOverLifetime * Time.deltaTime;
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(velocity.x, velocity.y, 0), out hit, Time.deltaTime * 10, ~0, QueryTriggerInteraction.Ignore);
        if (hit.transform != null)
        {
            print("hit");
            if (hit.transform.gameObject.name.Contains("Shield"))
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.TransformDirection(Vector3.forward), hit.normal));
            }
            else if (hit.transform.GetComponentInParent<Entity>() ? hit.transform.GetComponentInParent<Entity>() != owner : true)
            {
                if (hit.transform.GetComponentInParent<Entity>() != null)
                {
                    if (hit.transform.GetComponentInParent<Entity>() != owner)
                    {
                        //Explode();
                    }
                }
                else
                {
                    if (explodeOnTerrainHit)
                    {
                        Explode();
                    }
                }
            }
        }
        if (lifetime <= 1 && shrinkOverLifetime)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1 - lifetime);
        }
        if (lifetime <= 0)
        {
            if (explodeOnLifetimeEnd)
            {
                Explode();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        if (hitstop <= 0)
        {
            transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
        }
    }

    public override void Explode()
    {
        base.Explode();
    }
}
