using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Combat")]
    public float damage;
    [Tooltip("The stun period after being hit by an enemy attack.")]
    public float hitstun;
    [Tooltip("The freeze period after hitting, or being hit by an enemy.")]
    public float hitstop;
    [Tooltip("The unactionable period after being hit by an enemy attack.")]
    public float unactionable;
    public bool free = true;
    public bool invincible;
    public int combo;
    public float hitCombo;
    public MultiplayerManager multiplayer;
    public Entity killer;

    // Start is called before the first frame update
    public virtual void Start()
    {
        multiplayer = FindObjectOfType<MultiplayerManager>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (free)
        {
            //hitCombo = Mathf.Lerp(hitCombo, 0, Time.deltaTime * (free ? 1 : 0.01f));
            hitCombo = Mathf.MoveTowards(hitCombo, 0, Time.deltaTime * (free && hitstun < 0 && unactionable < 0 ? 1f : 0.1f));
        }

        if (multiplayer.universalTimeStop > 0)
        {
            return;
        }
    }

    public virtual void LateUpdate()
    {
        hitstun -= Time.deltaTime;
        hitstop -= Time.deltaTime;
        unactionable -= Time.deltaTime + (Time.deltaTime * hitCombo);
    }

    public virtual void Damage(float damageValue)
    {
        damage += damageValue;
    }

    public virtual void HitboxDamage(HitboxInfo hitbox, Entity attacker, int direction, float additionalAngle = 0)
    {
        if (true && !invincible)
        {
            hitstun = Mathf.Max(hitstun, hitbox.hitstun) / Mathf.Max(1f + hitCombo, 1f);
        }
        else if (false && multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)
        {
            hitstun = Mathf.Max(hitstun, hitbox.hitstun * 5f) / Mathf.Max(1f + hitCombo, 1f);
        }
        else if (false)
        {
            hitstun = Mathf.Max(hitstun, hitbox.hitstun * 2.5f) / Mathf.Max(1f + hitCombo, 1f);
        }
        if (!invincible)
        {
            hitstop = Mathf.Max(hitstop, hitbox.hitstop);
        }

        killer = attacker;
        
        Damage(hitbox.damage);

        if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform)
        {
            float knockback = ((hitbox.scaledKnockback / 100) * damage) + hitbox.unscaledKnockback;
            unactionable = Mathf.Max(knockback / 25f, damage / 100f);
        }
        else
        {
            float knockback = (hitbox.scaledKnockback / 2) + hitbox.unscaledKnockback;
            unactionable = Mathf.Max(knockback / 25f, damage / 100f);
        }
        unactionable = Mathf.Max(unactionable, hitstun);

        hitCombo += unactionable / 10;
    }

    public virtual GameObject GetHitEffect()
    {
        return multiplayer.hitEffect;
    }
}
