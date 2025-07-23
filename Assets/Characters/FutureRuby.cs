using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FutureRuby : Fighter
{
    public bool kineticLeap;
    [Range(0, 1)]
    public float energy;
    public GameObject glowL;
    public GameObject glowR;
    public ParticleSystem lightning;

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

        ParticleSystem.EmissionModule emission = lightning.emission;
        emission.rateOverTime = 10f * energy;
        emission.rateOverDistance = 2.5f * energy;

        if (player.grounded)
        {
            kineticLeap = false;
        }

        if (!player.pratfall)
        {
            energy = Mathf.MoveTowards(energy, 1, Time.deltaTime / 20);
        }
    }

    public void LateUpdate()
    {
        if (kineticLeap && !player.model.animator.GetCurrentAnimatorStateInfo(0).IsName("Forward Special"))
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

    public override bool CanSetAttack(string attackName)
    {
        bool canSetAttack = true;

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
        if (attackName == "A2A" || attackName == "Down Air")
        {
            InputReverse();
        }

        return canSetAttack;
    }

    public void WindLaunch()
    {
        if (player.input.GetLeftStick() != Vector2.zero)
        {
            player.velocity = player.input.GetLeftStick() * 20;
            player.FaceDirection(player.input.GetLeftStickX());
        }
        else
        {
            player.velocity = new Vector2(player.direction, 1).normalized * 20;
        }
    }
}
