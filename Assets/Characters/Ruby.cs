using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Ruby : Fighter
{
    public bool hasFlippies = true;
    public WindBubble windBubble;
    public WindBubble windBubblePrefab;
    public bool storedWindBubble;
    public bool hasWindSlash;
    public GameObject windCharge;

    [Header("Trail")]
    public Color trailColor;
    public float trailCooldown;

    [Header("Parkour Trail")]
    public ParticleSystem headTrail;
    public ParticleSystem torsoTrail;
    public ParticleSystem leftShoulderTrail;
    public ParticleSystem rightShoulderTrail;
    public ParticleSystem leftHandTrail;
    public ParticleSystem rightHandTrail;
    public ParticleSystem leftFootTrail;
    public ParticleSystem rightFootTrail;

    [Header("Body")]
    public Transform head;
    public Transform torso;
    public Transform handsRoot;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform feetRoot;
    public Transform leftShoulder;
    public Transform rightShoulder;

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

        windCharge.SetActive(hasWindSlash);

        if (windBubble != null)
        {
            if (player.pratfall || !hasFlippies)
            {
                if (Vector3.Distance(transform.position, windBubble.transform.position) < windBubble.transform.localScale.z / 2 || storedWindBubble)
                {
                    if (player.pratfall)
                    {
                        model.animator.Play("Airborne");
                        player.pratfall = false;
                    }
                    if (!hasFlippies)
                    {
                        hasFlippies = true;
                    }
                    storedWindBubble = false;
                }
            }
        }
        if (storedWindBubble && (player.pratfall || !hasFlippies) && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special") && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Wind Launch"))
        {
            if (Vector3.Distance(transform.position, windBubble.transform.position) < windBubble.transform.localScale.z / 2 || storedWindBubble)
            {
                if (player.pratfall)
                {
                    model.animator.Play("Airborne");
                    player.pratfall = false;
                }
                if (!hasFlippies)
                {
                    hasFlippies = true;
                }
                storedWindBubble = false;
            }
        }

        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special") && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Wind Launch"))
        {
            player.grounded = false;
            if (windBubble ? Vector3.Distance(transform.position, windBubble.transform.position) < windBubble.transform.localScale.z / 2 : false)
            {
                storedWindBubble = true;
            }
        }
        else if (!player.pratfall && player.grounded && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special") && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Wind Launch"))
        {
            storedWindBubble = false;
        }

        if (player.grounded)
        {
            hasFlippies = true;
        }

        if (trailCooldown <= 0)
        {
            trailCooldown = 0.05f;
            if (!hasFlippies)
            {
                UpdateTrail(player);
            }
        }
        else
        {
            trailCooldown -= Time.deltaTime;
        }
    }

    public override bool CustomAI(ComputerInput AI)
    {
        bool overrideBehaviour = false;

        if (windBubble == null)
        {
            AI.options.Add("Down Special");
        }
        //else if (windBubble != null && AI.target != null ? Vector3.Distance(transform.position, windBubble.transform.position) < windBubble.transform.localScale.z / 2 : false)
        else if (windBubble != null && AI.target != null ? Vector3.Distance(transform.position, windBubble.transform.position) < windBubble.transform.localScale.z / 2 && Vector3.Distance(AI.target.transform.position, windBubble.transform.position) > windBubble.transform.localScale.z / 2 : false)
        {
            AI.options.Add("Forward Special");
            if (!AI.target.grounded || AI.state == ComputerInput.ComputerState.Recovering)
            {
                AI.options.Add("Up Special");
            }
        }

        if (AI.state != ComputerInput.ComputerState.Recovering && (currentMove != null ? currentMove.name == "Up Special" : false))
        {
            AI.leftStick = (AI.target.transform.position - transform.position).normalized;

            if (player.controller.isGrounded)
            {
                AI.leftStick.y = Mathf.Max(AI.leftStick.y, 0.25f);
            }
        }

        return overrideBehaviour;
    }

    public override bool CanSetAttack(string attackName)
    {
        bool canSetAttack = true;

        if (attackName == "5B" || attackName == "Neutral Special")
        {
            if (!hasWindSlash)
            {
                canSetAttack = false;
                model.animator.Play("Wind Charge");
            }
            else
            {
                hasWindSlash = false;
            }
        }
        if (attackName == "A2A" || attackName == "Down Air")
        {
            InputReverse();
        }
        if (attackName == "6B" || attackName == "Forward Special")
        {
            if (hasFlippies)
            {
                canSetAttack = true;
                hasFlippies = false;
                player.grounded = false;
            }
            else
            {
                canSetAttack = false;
            }
        }
        if (attackName == "8B" || attackName == "Up Special")
        {
            if (windBubble != null)
            {
                if (Vector3.Distance(transform.position, windBubble.transform.position) < windBubble.transform.localScale.z)
                {
                    storedWindBubble = true;
                }
            }
        }

        return canSetAttack;
    }

    public override void OnHit(string moveName)
    {
        base.OnHit(moveName);

        if (moveName == "Down Air")
        {
            player.doubleJumps = Mathf.Max(player.doubleJumps, 1);
        }
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

    public void WindBubble()
    {
        if (windBubble != null)
        {
            windBubble.lifetime = Mathf.Min(windBubble.lifetime, 1);
        }

        windBubble = GameObject.Instantiate(windBubblePrefab, transform.position, Quaternion.identity).GetComponent<WindBubble>();
    }

    public void WindCharge()
    {
        hasWindSlash = true;
    }

    public void UpdateTrail(PlayerScript player)
    {
        Color auraColor = trailColor;

        var main = headTrail.main;
        main.startColor = auraColor;
        main = torsoTrail.main;
        main.startColor = auraColor;
        main = leftShoulderTrail.main;
        main.startColor = auraColor;
        main = rightShoulderTrail.main;
        main.startColor = auraColor;
        main = leftHandTrail.main;
        main.startColor = auraColor;
        main = rightHandTrail.main;
        main.startColor = auraColor;
        main = leftFootTrail.main;
        main.startColor = auraColor;
        main = rightFootTrail.main;
        main.startColor = auraColor;

        headTrail.transform.position = head.position;
        headTrail.transform.rotation = head.rotation;
        headTrail.transform.localScale = head.localScale;
        torsoTrail.transform.position = torso.position;
        torsoTrail.transform.rotation = torso.rotation;
        torsoTrail.transform.localScale = torso.localScale;
        leftShoulderTrail.transform.position = leftShoulder.position;
        leftShoulderTrail.transform.rotation = leftShoulder.rotation;
        leftShoulderTrail.transform.localScale = leftShoulder.localScale;
        rightShoulderTrail.transform.position = rightShoulder.position;
        rightShoulderTrail.transform.rotation = rightShoulder.rotation;
        rightShoulderTrail.transform.localScale = rightShoulder.localScale;
        leftHandTrail.transform.position = leftHand.position;
        leftHandTrail.transform.rotation = leftHand.rotation;
        leftHandTrail.transform.localScale = leftHand.localScale;
        rightHandTrail.transform.position = rightHand.position;
        rightHandTrail.transform.rotation = rightHand.rotation;
        rightHandTrail.transform.localScale = rightHand.localScale;
        leftFootTrail.transform.position = leftFoot.position;
        leftFootTrail.transform.rotation = leftFoot.rotation;
        leftFootTrail.transform.localScale = leftFoot.localScale;
        rightFootTrail.transform.position = rightFoot.position;
        rightFootTrail.transform.rotation = rightFoot.rotation;
        rightFootTrail.transform.localScale = rightFoot.localScale;

        if (head.GetComponent<MeshRenderer>().enabled)
        {
            headTrail.Emit(1);
        }
        if (torso.GetComponent<MeshRenderer>().enabled)
        {
            torsoTrail.Emit(1);
        }
        if (leftShoulder.GetComponent<MeshRenderer>().enabled)
        {
            leftShoulderTrail.Emit(1);
        }
        if (rightShoulder.GetComponent<MeshRenderer>().enabled)
        {
            rightShoulderTrail.Emit(1);
        }
        if (leftHand.GetComponent<MeshRenderer>().enabled)
        {
            leftHandTrail.Emit(1);
        }
        if (rightHand.GetComponent<MeshRenderer>().enabled)
        {
            rightHandTrail.Emit(1);
        }
        if (leftFoot.GetComponent<MeshRenderer>().enabled)
        {
            leftFootTrail.Emit(1);
        }
        if (rightFoot.GetComponent<MeshRenderer>().enabled)
        {
            rightFootTrail.Emit(1);
        }
    }

    public override void ResetFighter()
    {
        hasWindSlash = false;
    }
}
