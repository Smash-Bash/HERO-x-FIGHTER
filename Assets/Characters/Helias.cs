using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helias : Fighter
{
    public bool hasFloat;
    public bool floating;
    public float floatAmount;
    public float solarJudgementCharge;
    public Transform leftHand;
    public LightningBlast lightningPrefab;

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

        if (player.free && player.grounded)
        {
            hasFloat = true;
            floatAmount = 1;
        }

        if (player.free && floatAmount > 0 && (hasFloat || floating) && player.input.GetJump() && !player.grounded && player.velocity.y <= 0)
        {
            hasFloat = false;
            floating = true;
            floatAmount -= Time.deltaTime;

            player.velocity.y = 0f;
        }
        if (!player.input.GetJump() || !player.free || player.pratfall || player.hitstun > 0)
        {
            floating = false;
        }

        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Solar Judgement Charge"))
        {
            solarJudgementCharge += Time.deltaTime / 3;

            if (player.input.GetShield())
            {
                player.TestForDodge();
            }
        }
        else if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Solar Judgement"))
        {

        }
        else
        {
            
        }
        model.animator.SetFloat("SolarCharge", solarJudgementCharge);

        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Down Special") && model.allowFollowup && player.input.GetSpecial())
        {
            model.animator.Play("Down Special Cancel");
            player.input.specialBuffer = 0;
        }
    }

    public override void OnProjectileSpawn(int ID)
    {
        base.OnProjectileSpawn(ID);

        if (ID == 2)
        {
            solarJudgementCharge = 0;
        }
    }

    public void FireLightning()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(player.direction, 0, 0), out hit, 25, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject != player.gameObject)
            {
                LightningBlast newBlast = GameObject.Instantiate(lightningPrefab);
                newBlast.SetPositions(leftHand.transform.position, hit.point);

                if (hit.transform.GetComponent<Entity>() != null)
                {
                    hit.transform.GetComponent<Entity>().HitboxDamage(currentMove.hitboxes[0], player, player.direction);
                }
            }
            else
            {
                LightningBlast newBlast = GameObject.Instantiate(lightningPrefab);
                newBlast.SetPositions(leftHand.transform.position, hit.point);
            }
        }
        else
        {
            LightningBlast newBlast = GameObject.Instantiate(lightningPrefab);
            newBlast.SetPositions(leftHand.transform.position, transform.position + new Vector3(player.direction * 25, 0, 0));
        }
    }
}
