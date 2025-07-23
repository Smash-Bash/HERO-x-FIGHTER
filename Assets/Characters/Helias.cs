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

        if (floatAmount > 0 && (hasFloat || floating) && player.input.GetJump() && !player.grounded && player.velocity.y <= 0 && !player.pratfall)
        {
            hasFloat = false;
            floating = true;
            floatAmount -= Time.deltaTime;

            player.velocity.y = Mathf.Max(player.velocity.y, 0);
        }
        if (!player.input.GetJump() || player.pratfall || player.hitstun > 0)
        {
            floating = false;
        }

        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Solar Judgement Charge"))
        {
            solarJudgementCharge += Time.deltaTime / 3;

            if (player.input.GetShield())
            {
                player.airDodges = Mathf.Max(player.airDodges, 1);
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

    public override bool CustomAI(ComputerInput AI)
    {
        bool overrideBehaviour = false;

        if ((AI.state != ComputerInput.ComputerState.Recovering || transform.position.y > -2) && model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special"))
        {
            AI.overrideSpecial = Vector3.Distance(transform.position, AI.target.transform.position) > 2.5f;
        }
        else if (AI.state != ComputerInput.ComputerState.Recovering && model.animator.GetCurrentAnimatorStateInfo(0).IsName("Solar Judgement Charge"))
        {
            AI.overrideSpecial = true;
            overrideBehaviour = true;
            AI.leftStick = new Vector2(Random.Range(-1, 1), 0);
            if ((!player.InFront(AI.target.transform.position) && Vector3.Distance(transform.position, AI.target.transform.position) < 7.5f) || Vector3.Distance(transform.position, AI.target.transform.position) < 2.5f)
            {
                AI.overrideShield = true;
                player.TestForDodge();
            }
            else if (solarJudgementCharge >= 1)
            {
                AI.overrideSpecial = true;
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 2, new Vector3(player.direction, -1, 0), 10, LayerMask.GetMask("Player"));
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.GetComponent<PlayerScript>() != null && hit.collider.GetComponent<PlayerScript>() != player)
                    {
                        AI.overrideSpecial = false;
                    }
                }
            }
        }

        return overrideBehaviour;
    }

    public override void OnDamageRecieved(float damage)
    {
        base.OnDamageDealt(damage);

        if (solarJudgementCharge < 0.5)
        {
            solarJudgementCharge = Mathf.MoveTowards(solarJudgementCharge, 0.5f, damage / 100);
        }
    }

    public override void OnProjectileSpawn(int ID)
    {
        base.OnProjectileSpawn(ID);

        if (ID == 3)
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
                    hit.transform.GetComponent<Entity>().HitboxDamage(currentMove.hitboxes[0], player, hit.transform.position, player.direction);
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

    public override void ResetFighter()
    {
        solarJudgementCharge = 0;
    }
}
