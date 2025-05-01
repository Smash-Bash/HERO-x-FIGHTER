using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathmakerRuby : Fighter
{
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

    [Header("Double Jump Trail")]
    public TrailRenderer footLTrail;
    public TrailRenderer footRTrail;

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

        footLTrail.enabled = player.doubleJumps <= 0 && player.fighter.maxDoubleJumps > 0;
        footRTrail.enabled = player.doubleJumps <= 0 && player.fighter.maxDoubleJumps > 0;
        if (trailCooldown <= 0)
        {
            trailCooldown = 0.05f;
            if (player.sprinting || !player.fastFall)
            {
                UpdateTrail(player);
            }
        }
        else
        {
            trailCooldown -= Time.deltaTime;
        }

        if ((currentMove != null ? currentMove.name == "Neutral Special" : false) && player.model.allowFollowup && player.input.GetSpecial())
        {
            SetAttack("5B", true);
        }
    }

    public override void OnHit(string moveName)
    {
        base.OnHit(moveName);

        if (moveName == "Down Special")
        {
            if (player.model.animator.GetCurrentAnimatorStateInfo(0).IsName("Down Special (Loop)"))
            {
                player.velocity = Vector2.up * 15;
                player.grounded = false;
                player.model.animator.Play("Launched");
            }
        }
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
}
