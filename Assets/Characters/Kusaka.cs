using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kusaka : Fighter
{
    public Vector2 launchVelocity;
    public bool kineticLeap;
    public bool overclocked;
    [Range(0, 1)]
    public float energy;
    public GameObject glowL;
    public GameObject glowR;
    public ParticleSystem lightning;
    public Material neutralFace;
    public Material overclockedFace;
    public GameObject overclockedAura;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void FighterUpdate(PlayerScript player)
    {
        base.FighterUpdate(player);

        if (player.multiplayer.endOfMatch || player.multiplayer.endOfRound)
        {
            overclocked = false;
        }

        if (overclocked)
        {
            model.face.material = overclockedFace;
            if (player.damage + (Time.deltaTime * 2) <= 99.9f)
            {
                player.Damage(Time.deltaTime * 2);
            }
            energy = Mathf.MoveTowards(energy, 1f, Time.deltaTime / 10);
        }
        else
        {
            model.SetEmotion();
        }
        overclockedAura.SetActive(overclocked);

        if (player.getupTest && player.input.GetAttack() && player.input.GetSpecial())
        {
            SetAttack("Getup Special");
        }

        if (player.gettingUp && energy >= 0.25f && player.input.GetSpecial() && player.killer != null)
        {
            if (player.damage < 99)
            {
                player.damage = Mathf.MoveTowards(player.damage, 99, 20);
            }
            energy = Mathf.MoveTowards(energy, 0f, 0.25f);

            SetAttack("Getup Special");
        }

        ParticleSystem.EmissionModule emission = lightning.emission;
        emission.rateOverTime = 10f * energy;
        emission.rateOverDistance = 2.5f * energy;

        if (player.grounded)
        {
            kineticLeap = false;
        }

        if (player.model.animator.GetCurrentAnimatorStateInfo(0).IsName("Getup Special"))
        {
            CalculateLaunch();
            player.FaceDirection(player.killer.transform.position.x);
        }
        if (player.model.animator.GetCurrentAnimatorStateInfo(0).IsName("Getup Special Launch"))
        {
            player.velocity = launchVelocity;
            player.grounded = false;
        }
        if (player.model.animator.GetCurrentAnimatorStateInfo(0).IsName("Forward Special"))
        {
            kineticLeap = true;
        }

        if (hitPlayer && !currentMove.name.Contains("Special") && Mathf.Abs(player.input.GetLeftStickX()) > 0.25f && player.input.GetSpecialDown())
        {
            SetAttack("Forward Special");
        }
    }

    public void LateUpdate()
    {
        if (kineticLeap && !player.model.animator.GetCurrentAnimatorStateInfo(0).IsName("Side Special"))
        {
            glowL.SetActive(true);
            glowR.SetActive(true);
        }
        else
        {
            glowL.SetActive(false);
            glowR.SetActive(false);
        }
    }

    public void CalculateLaunch()
    {
        if (player.killer != null)
        {
            launchVelocity = (player.killer.transform.position - player.transform.position).normalized * 25;
        }
    }

    public override void OnHit(string moveName)
    {
        base.OnHit(moveName);

        energy = Mathf.MoveTowards(energy, 1f, 0.05f);

        if (moveName == "Getup Special Launch")
        {
            player.velocity = Vector2.up * 15;
            player.grounded = false;
            player.model.animator.Play("Launched");
        }
    }

    public override void OnSetAttack()
    {
        base.OnSetAttack();

        if (currentMove.name == "Up Special")
        {
            currentMove.hitboxes[0].damage *= 1 + energy;
            currentMove.hitboxes[0].unscaledKnockback *= 1 + energy;
            energy = 0;
        }
    }

    public override bool CanSetAttack(string attackName)
    {
        bool canSetAttack = true;

        if (attackName == "5B" || attackName == "Neutral Special")
        {
            if (player.tumbling || player.landing || player.gettingUp)
            {
                canSetAttack = false;
            }
        }
        if (attackName == "6B" || attackName == "Forward Special")
        {
            if (energy >= 0.25f)
            {
                energy = Mathf.MoveTowards(energy, 0, 0.25f);
            }
            else
            {
                canSetAttack = false;
            }
        }
        if (attackName == "2B" || attackName == "Down Special")
        {
            if (overclocked)
            {
                overclocked = false;
                canSetAttack = false;
            }
            else
            {
                canSetAttack = true;
            }
        }

        return canSetAttack;
    }

    public override bool OnDamage(Entity other)
    {
        bool cancel = false;

        if (currentMove != null ? currentMove.name == "Neutral Special" : false)
        {
            if ((other.transform.position.x > transform.position.x && player.direction == 1) || (other.transform.position.x < transform.position.x && player.direction == -1))
            {
                cancel = true;
                other.hitstop = 1f;
                player.hitstop = 0.125f;
                energy = Mathf.MoveTowards(energy, 1f, 0.25f);
                SetAttack("Neutral Special Counter");
            }
        }

        return cancel;
    }

    public void Overclock()
    {
        if (!overclocked)
        {
            overclocked = true;
        }
        else
        {
            overclocked = false;
        }
    }

    public override void ResetFighter()
    {
        energy = 0;
        overclocked = false;
    }
}
