using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    public PlayerScript player;
    [Min(0)]
    public float radius = 0;
    [Min(0)]
    public float length = 0;
    [Min(0)]
    public float damageMultiplier = 1;
    public List<GameObject> currentlyHitThings;
    private float scale;
    [Header("Animation")]
    public int hitboxNum;

    public UnityEvent onHit;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.parent.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        scale = 1;
    }

    public List<Entity> Attack(Entity owner)
    {
        int LayersToIgnore = ~(1 << LayerMask.NameToLayer("No Collision") | 1 << LayerMask.NameToLayer("Player Attack Hitbox") | 1 << LayerMask.NameToLayer("2D Section") | 1 << LayerMask.NameToLayer("Viewmodel"));
        Collider[] hits;
        Vector3 depthMultiplier = new Vector3(1, 1, 0);
        if (length != 0)
        {
            hits = Physics.OverlapCapsule(Vector3.Scale(depthMultiplier, transform.position), Vector3.Scale(depthMultiplier, transform.position + (transform.forward * length)), radius * scale, LayersToIgnore);
        }
        else
        {
            hits = Physics.OverlapSphere(Vector3.Scale(depthMultiplier, transform.position), radius * scale, LayersToIgnore);
        }
        List<Entity> hitEntities = new List<Entity>();
        foreach (Collider hit in hits)
        {
            if (!currentlyHitThings.Contains(hit.gameObject))
            {
                if (hit.GetComponent<Entity>() != null && hit.gameObject != owner.gameObject)
                {
                    hitEntities.Add(hit.GetComponent<Entity>());
                }
                currentlyHitThings.Add(hit.gameObject);
            }
        }
        return hitEntities;
    }

    public void OnDrawGizmos()
    {
        if (isActiveAndEnabled)
        {
            //Vector3 depthMultiplier = new Vector3(1, 1, 0);
            Vector3 depthMultiplier = Vector3.one;
            Color oldColor = Gizmos.color;
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawWireSphere(Vector3.Scale(depthMultiplier, transform.position), radius);
            Gizmos.DrawWireSphere(Vector3.Scale(depthMultiplier, transform.position + (transform.forward * length)), radius);
            Gizmos.DrawSphere(Vector3.Scale(depthMultiplier, transform.position), radius);
            Gizmos.DrawSphere(Vector3.Scale(depthMultiplier, transform.position + (transform.forward * length)), radius);
            Gizmos.color = oldColor;

            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }

    public void OnDisable()
    {
        currentlyHitThings.Clear();
    }
}
