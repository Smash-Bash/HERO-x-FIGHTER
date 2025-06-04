using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : Entity
{
    public PlayerModel model;
    public Fighter fighter;
    public PlayerInput input;
    public Entity opponent;
    [Range(1, 4)]
    public int playerID;
    [Min(0)]
    public int stocks;
    [Min(0)]
    public float health = 0;
    public bool grounded;
    public bool unconscious;
    public Vector2 velocity;
    public bool sprinting;
    public int doubleJumps;
    public bool fastFall = true;
    [Range(-1f, 1f)]
    public int direction;
    public float scale;
    public float blastCooldown;
    public bool pratfall;
    public bool tumbling;
    public bool landing;
    public bool gettingUp;
    public bool shielding;
    public bool dodging;
    public bool blockStop;
    public bool hasWallJump;

    public bool launched;

    public float shieldAmount = 1;

    [Header("Ledges")]
    public Transform mantleObject;
    public bool mantling;
    public float mantleMaxHeight = 2f;
    public float mantleLength = 0.5f;
    public Vector3 mantleStart;
    public Vector3 mantleEnd;
    public Vector3 mantleHandPlacement;

    [Header("Spawn Info")]
    public Vector3 spawnPoint;
    public int spawnDirection;

    [Header("References")]
    public CharacterController controller;
    public HeadsUpDisplay hud;
    public ParticleSystem launchEffect;
    public ParticleSystem victoryEffect;
    public GameObject blockEffect;
    public GameObject lowBreakEffect;
    public GameObject overheadBreakEffect;
    public MeshRenderer shield;

    [Header("Debug")]
    public bool getupTest;
    public bool escapeTest;
    public bool aerialAttackTest;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        multiplayer = FindObjectOfType<MultiplayerManager>();
        spawnPoint = transform.position;
        spawnDirection = direction;
        if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform)
        {
            stocks = multiplayer.stocks;
        }
        else
        {
            stocks = 1;
        }
        playerID = Mathf.Clamp(playerID, 1, 4);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        model.gameObject.SetActive(multiplayer.cutscenes ? !multiplayer.cutscenes.isActiveAndEnabled : true);
        if (multiplayer.universalTimeStop > 0 || Time.timeScale == 0 || (multiplayer.cutscenes ? multiplayer.cutscenes.isActiveAndEnabled : false))
        {
            return;
        }

        if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform)
        {
            opponent = null;
        }
        else if (opponent == null)
        {
            foreach (PlayerScript currentPlayer in FindObjectsOfType<PlayerScript>())
            {
                if (currentPlayer != this)
                {
                    opponent = currentPlayer;
                    break;
                }
            }
        }
        else
        {
            if (opponent.free && opponent.hitstun <= 0 && opponent.hitstop <= 0 && opponent.unactionable <= 0)
            {
                combo = 0;
            }
        }

        ParticleSystem.EmissionModule emission = launchEffect.emission;
        if (unactionable > 0 && launched)
        {
            emission.rateOverDistance = 10;
        }
        else
        {
            emission.rateOverDistance = 0;
        }
        ParticleSystem.MainModule main = launchEffect.main;
        main.startColor = multiplayer.colors[playerID - 1];

        if (!grounded)
        {
            if (launched && (hitstun - hitCombo > 0 || multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform))
            {
                TestForBounce();
            }
        }
        else
        {
            hasWallJump = true;
            unactionable = 0;
            if (launched && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
            {
                model.animator.Play("Land", 0);
            }
            launched = false;
        }
        if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)
        {
            if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Launched"))
            {
                //unactionable = Mathf.Max(unactionable, 0.25f);
            }
        }
        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
        {
            launched = false;
            velocity.x = 0f;
            velocity.y = -2.5f;
            hitCombo = 0;
            unactionable = 0;
        }

        if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)
        {
            stocks = Mathf.Max(stocks, 1);
        }

        model.animator.SetBool("Shielding", input.GetShield() && fighter.currentMove == null && hitstun <= 0 && hitstop <= 0 && unactionable <= 0 && !dodging && grounded && shieldAmount > 0 && multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform);

        model.animator.SetBool("Launched", launched);
        grounded = controller.isGrounded;
        unconscious = unconscious || (damage >= health && multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional);
        invincible = model.invincible;
        input.enabled = !(unconscious || multiplayer.endOfRound);
        model.animator.SetBool("Alive", !unconscious);
        model.animator.SetBool("TimeStop", Time.timeScale == 0);
        model.animator.SetBool("EndOfRound", multiplayer.endOfRound && !multiplayer.endOfMatch);
        model.animator.SetBool("EndOfMatch", multiplayer.endOfMatch && Time.timeScale > 0);
        blastCooldown -= Time.deltaTime;

        if (unconscious)
        {
            if (!grounded)
            {
                launched = true;
            }
            else if (hitstun <= 0 && !model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land"))
            {
                model.animator.Play("Land");
            }
        }
        if (!free)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * (grounded ? fighter.groundedAcceleration : fighter.aerialAcceleration));

            input.ignoreBuffers = true;
            if (input.GetAttackDown())
            {
                input.attackBuffer = 0.25f;
            }
            if (input.GetSpecialDown())
            {
                input.specialBuffer = 0.25f;
            }
            if (input.GetJumpDown())
            {
                input.jumpBuffer = 0.25f;
            }
            input.ignoreBuffers = false;

            if (fighter.currentMove != null)
            {
                if (!fighter.currentMove.airborne && !grounded)
                {
                    fighter.currentMove = null;
                    model.animator.Play("Airborne");
                }
                if (!fighter.currentMove.grounded && grounded)
                {
                    fighter.currentMove = null;
                    model.animator.Play("Grounded");
                }
            }

            if (!grounded && hitstun <= 0 && hitstop <= 0 && unactionable <= 0 && !model.suppressControl)
            {
                GetVelocityInput();
            }
        }
        else if (!model.suppressControl)
        {
            GetVelocityInput();
        }
        if (direction == 0)
        {
            direction = 1;
        }
        model.transform.localScale = new Vector3(direction, 1, 1);
        scale = transform.localScale.y;

        if (hitstop <= 0)
        {
            if (hitstun <= 0)
            {
                blockStop = false;
            }

            if (!mantling)
            {
                DefaultState();
            }
            else
            {
                LedgeState();
            }
        }

        free = unactionable <= 0 && hitstun <= 0 && (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Launched") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Back Double Jump") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Front Double Jump") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Pratfall"));

        fighter.FighterUpdate(this);
        if (escapeTest)
        {
            if (tumbling && (free || tumbling) && !pratfall && unactionable <= 0 && !landing && !gettingUp)
            {
                fighter.SetAttack("Neutral Air");
            }
        }
        if (aerialAttackTest && free && hitstun < 0 && hitstop < 0)
        {
            transform.position = new Vector3(0, 1f, 0);
            velocity = Vector2.zero;
        }

        free = unactionable <= 0 && hitstun <= 0 && (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Launched") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Back Double Jump") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Front Double Jump") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Pratfall"));
        pratfall = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Pratfall") || model.animator.GetNextAnimatorStateInfo(0).IsName("Pratfall");
        tumbling = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Launched") && unactionable <= 0 && hitstop <= 0 && hitstun <= 0;
        landing = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land");
        gettingUp = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Getup");
        shielding = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shielding") && hitstun <= 0 && hitstop <= 0 && unactionable <= 0 && !dodging && multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform;
        dodging = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge");

        float shieldSize = controller.height * shieldAmount;
        shield.transform.localScale = new Vector3(shieldSize, shieldSize, shieldSize);
        shield.gameObject.SetActive(shielding);
        if (shielding)
        {
            shieldAmount = Mathf.MoveTowards(shieldAmount, 0, Time.deltaTime / 5);
            sprinting = false;

            if (shieldAmount <= 0 || hitstun > 0 || hitstop > 0)
            {
                shieldAmount = 0;
                Damage(25);
                unactionable = 5;
                hitstun = 5;
                velocity = Vector3.up * 15;
                tumbling = true;
            }
        }
        else
        {
            shieldAmount = Mathf.MoveTowards(shieldAmount, 1, Time.deltaTime / 5);
        }

        TestForBlastZones();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        model.animator.SetBool("TimeStop", Time.timeScale == 0);

        free = unactionable <= 0 && hitstun <= 0 && (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Airborne") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Launched") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Back Double Jump") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Front Double Jump") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Pratfall"));
        pratfall = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Pratfall") || model.animator.GetNextAnimatorStateInfo(0).IsName("Pratfall");
        tumbling = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Launched");
        landing = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Land");
        gettingUp = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Getup");
        shielding = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shielding") && hitstun <= 0 && hitstop <= 0 && unactionable <= 0 && !dodging && multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform;
        dodging = model.animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge");
    }

    public void DefaultState()
    {
        if (model.alignToVelocity)
        {
            Vector2 dir = velocity * new Vector2(direction, direction);
            if (grounded)
            {
                dir.y = 0;
            }
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, (Camera.main.orthographic || free ? model.targetRotation : 90) * direction, 0);
            transform.Rotate(0, 0, angle, Space.World);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(transform.eulerAngles.y, (Camera.main.orthographic || free ? model.targetRotation : 90) * direction, Time.deltaTime * 45), 0);
        }

        if (grounded)
        {
            if (velocity.y <= 0)
            {
                velocity.y = -2.5f;
            }
            doubleJumps = fighter.maxDoubleJumps;
            fastFall = true;
        }
        else
        {
            if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional || !fastFall)
            {
                velocity.y -= fighter.gravity * Time.deltaTime;
                if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)
                {
                    velocity.y -= fighter.gravity * Time.deltaTime * Mathf.Max(0, -1 + hitCombo);
                }
            }
            else
            {
                velocity.y = Mathf.MoveTowards(velocity.y, fighter.maxGravity * -1, fighter.gravity * Time.deltaTime);
            }
        }

        Vector2 velocityToMove = velocity;

        if (model.overrideXVelocity)
        {
            velocityToMove.x = model.velocity.x * direction;
            velocity.x = model.velocity.x * direction;
        }
        else
        {
            velocityToMove.x += model.velocity.x * direction;
        }
        if (model.overrideYVelocity)
        {
            velocityToMove.y = model.velocity.y;
            velocity.y = model.velocity.y;
        }
        else
        {
            velocityToMove.y += model.velocity.y;
        }

        if ((free || shielding) && !dodging && input.GetShield() && !launched && !pratfall && (!grounded || (shielding || multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)))
        {
            TestForDodge();
        }

        if (launched)
        {
            TestForBounce();
        }

        if (launched || (grounded && input.GetLeftStickY() < -0.5f && input.GetStickFlick()) || (!grounded && input.GetLeftStickY() < -0.5f))
        {
            controller.excludeLayers = LayerMask.GetMask("Platform");
        }
        else
        {
            controller.excludeLayers = 0;
        }
        Vector3 oldPosition = transform.position - new Vector3(velocityToMove.x, velocityToMove.y, 0) * Time.deltaTime;
        if (!model.ledgeSnap || !grounded || TestForGround((velocityToMove + new Vector2(0, (grounded ? 2.5f : 0))) * Time.deltaTime))
        {
            controller.Move(new Vector3(velocityToMove.x, velocityToMove.y, 0) * Time.deltaTime);
        }
        if (model.ledgeSnap && grounded && !controller.isGrounded)
        {
            transform.SetPositionAndRotation(oldPosition, transform.rotation);
            grounded = true;
        }
        else
        {
            grounded = controller.isGrounded;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        MoveAway();
    }

    public void LedgeState()
    {
        transform.position = mantleStart;
    }

    void GetVelocityInput()
    {
        if (input.GetLeftStickY() < -0.25f && !model.crouching && grounded && free && !sprinting)
        {
            velocity.x = 0;
            model.crouching = true;
            model.animator.SetFloat("Crouching", 1);
        }

        if (model.crouching)
        {
            velocity.x = 0;
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, (sprinting ? fighter.runSpeed : fighter.walkSpeed) * input.GetLeftStickX(), Time.deltaTime * (grounded ? fighter.groundedAcceleration : fighter.aerialAcceleration));
        }

        if (grounded)
        {
            if (input.GetStickFlick() && free && (opponent == null || velocity.x * Mathf.Infinity == direction * Mathf.Infinity))
            {
                if (input.GetLeftStickX() * Mathf.Infinity != direction * Mathf.NegativeInfinity)
                {
                    velocity.x = fighter.runSpeed * input.GetLeftStickX();
                }
                sprinting = true;
            }

            if (opponent == null)
            {
                if (input.GetLeftStickX() > 0.1f && free)
                {
                    direction = 1;
                }
                else if (input.GetLeftStickX() < -0.1f && free)
                {
                    direction = -1;
                }
                else
                {
                    if (Mathf.Abs(velocity.x) < 0.5f)
                    {
                        sprinting = false;
                    }
                }
            }
            else
            {
                if (transform.position.x < opponent.transform.position.x && free)
                {
                    direction = 1;
                }
                else if (transform.position.x > opponent.transform.position.x && free)
                {
                    direction = -1;
                }

                if (Mathf.Abs(input.GetLeftStickX()) < 0.1f)
                {
                    if (Mathf.Abs(velocity.x) < 0.5f)
                    {
                        sprinting = false;
                    }
                }
                if (velocity.x * Mathf.Infinity != direction * Mathf.Infinity)
                {
                    sprinting = false;
                }
            }
        }
        else
        {
            if ((velocity.y < fighter.jumpHeight / 2 || fighter.hitAttack) && fastFall && Mathf.Abs(input.GetLeftStickX()) < 0.25f && input.GetLeftStickY() < -0.75f && input.GetStickFlick())
            {
                fastFall = false;
                if (hitstun <= 0 && hitstop > 0)
                {
                    hitstop = 0;
                }
                velocity.y = Mathf.Min(velocity.y, fighter.maxGravity * -1);
                velocity.y -= 10;
            }
        }

        if ((free || model.allowFollowup) && !TestForWallJump() && !pratfall && input.GetJumpDown() && (grounded || doubleJumps > 0))
        {
            Jump();
            input.jumpBuffer = 0;
        }
    }

    public void TestForDodge()
    {
        if ((input is GamepadInput && input.GetStickFlick()) || ((!shielding || input is KeyboardInput || multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional) && input.GetLeftStick() != Vector2.zero))
        {
            print("Dodge");
            if (Mathf.Abs(input.GetLeftStickX()) < 0.25f)
            {
                model.animator.SetFloat("DodgeDirection", 0);
                model.animator.Play("Dodge");
                free = false;
                shielding = false;
            }
            else if (opponent != null)
            {
                if ((direction == 1 && input.GetLeftStickX() >= 0f) || (direction == -1 && input.GetLeftStickX() <= 0f))
                {
                    model.animator.SetFloat("DodgeDirection", 1);
                    model.animator.Play("Dodge");
                    free = false;
                    shielding = false;
                }
                else if ((direction == 1 && input.GetLeftStickX() <= 0f) || (direction == -1 && input.GetLeftStickX() >= 0f))
                {
                    model.animator.SetFloat("DodgeDirection", -1);
                    model.animator.Play("Dodge");
                    free = false;
                    shielding = false;
                }
            }
            else
            {
                model.animator.SetFloat("DodgeDirection", -1);
                if (input.GetLeftStickX() >= 0)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }
                model.animator.Play("Dodge");
                free = false;
                shielding = false;
            }
            transform.rotation = Quaternion.Euler(0, Mathf.LerpAngle(transform.eulerAngles.y, (Camera.main.orthographic || free ? model.targetRotation : 90) * direction, Time.deltaTime * 45), 0);
        }
    }

    public bool TestForWallJump()
    {
        bool wallJumped = false;
        int testDirection = direction;

        if (velocity.x > 0)
        {
            testDirection = 1;
        }
        else if (velocity.x < 0)
        {
            testDirection = -1;
        }

        if (!grounded && hasWallJump && free)
        {
            if (Physics.Raycast(transform.position + (Vector3.right * testDirection * 0.5f * controller.radius), Vector3.right * testDirection, 1f, LayerMask.GetMask("Default")))
            {
                print("true");
                if (input.GetJumpDown())
                {
                    velocity.x = testDirection * -7.5f;
                    velocity.y = fighter.jumpHeight;
                    direction = testDirection * -1;
                    model.animator.Play("Front Double Jump");
                    wallJumped = true;
                    hasWallJump = false;
                }
            }
        }

        return wallJumped;
    }

    public bool TestForMantle()
    {
        int LayersToHit = LayerMask.GetMask("Default");
        RaycastHit hit;
        Debug.DrawLine(transform.position + new Vector3(0, mantleMaxHeight, 0) + (Vector3.right * direction * 0.625f * scale), transform.position + new Vector3(0, 0.25f, 0) + (Vector3.right * direction * 0.625f * scale), Color.red, 0);
        if (!launched)
        {
            // First Raycast: Find Object To Mantle Onto
            if (Physics.Raycast(transform.position + new Vector3(0, mantleMaxHeight * scale, 0) + (Vector3.right * direction * 0.625f * scale), Vector3.down * scale, out hit, (mantleMaxHeight - 0.25f) * scale, LayersToHit))
            {
                print("a");
                mantleObject = hit.transform;
                // Collision Check: Is it safe on the other side?
                RaycastHit[] hits = Physics.CapsuleCastAll(hit.point + new Vector3(0, 0.5f * scale, 0), hit.point + new Vector3(0, 1.5f * scale, 0), 0.45f * scale, transform.eulerAngles, scale, LayersToHit);
                if (hits.Length == 0)
                {
                    print("b");
                    hits = Physics.CapsuleCastAll(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 0.35f * scale, transform.eulerAngles, scale, LayersToHit);
                    // Second Raycast: Can the player climb up without being obstructed?
                    if (hits.Length == 0)
                    {
                        print("c");
                        mantleHandPlacement = hit.point;
                        mantleStart = transform.position;
                        mantleEnd = hit.point + new Vector3(0, 0.5f * scale, 0);

                        transform.position = mantleEnd;
                        velocity = Vector3.zero;
                        mantling = true;
                        //mantleWind.Play();
                    }
                    else
                    {
                        foreach (RaycastHit hitToSay in hits)
                        {
                            print(hitToSay.transform.name);
                        }
                    }
                }
                else
                {
                    foreach (RaycastHit hitToSay in hits)
                    {
                        print(hitToSay.transform.name);
                    }
                }
            }
        }
        return mantling;
    }

    public override void HitboxDamage(HitboxInfo hitbox, Entity attacker, Vector3 hitPoint, int direction, float additionalAngle = 0)
    {
        bool blocking = (free || blockStop || shielding) && ((multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional && Mathf.Abs(input.GetLeftStickX()) > 0.2f && input.GetLeftStickX() * Mathf.Infinity == direction * Mathf.Infinity) || (multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform && shielding));

        if (shielding)
        {
            blocking = true;
        }

        if (blocking)
        {
            if (shielding)
            {
                blocking = true;
            }
            else if (hitbox.type == HitboxType.Unblockable)
            {
                blocking = false;
            }
            else if (hitbox.type == HitboxType.Low && !model.crouching)
            {
                blocking = false;
                GameObject.Instantiate(lowBreakEffect, transform.position + model.transform.forward, model.transform.rotation);
            }
            else if (hitbox.type == HitboxType.Overhead && model.crouching)
            {
                blocking = false;
                GameObject.Instantiate(overheadBreakEffect, transform.position + model.transform.forward, model.transform.rotation);
            }
        }

        if (invincible)
        {

        }
        else if (blocking)
        {
            if (!shielding)
            {
                velocity.x = direction * 15;

                GameObject.Instantiate(blockEffect, transform.position + model.transform.forward, model.transform.rotation);

                hitstop = Mathf.Max(hitstop, hitbox.hitstop);
                hitstun = Mathf.Max(hitstun, hitbox.hitstun);
                blockStop = true;
            }
            else
            {
                shieldAmount -= hitbox.damage / 25;
            }
        }
        else if (fighter.OnDamage(attacker))
        {

        }
        else
        {
            base.HitboxDamage(hitbox, attacker, hitPoint, direction);

            doubleJumps = fighter.maxDoubleJumps;
            hasWallJump = true;

            // CREDIT: NinjaBash & https://www.reddit.com/r/godot/comments/186qz59/comment/kb9q8t1/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
            float angle = hitbox.angle * Mathf.Deg2Rad;
            if (hitbox.forwardDependentAngle)
            {
                angle -= additionalAngle * Mathf.Deg2Rad;
            }
            Vector2 knockback = new Vector2(hitbox.scaledKnockback, 0);
            if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Platform)
            {
                knockback = new Vector2(((hitbox.scaledKnockback / 100) * damage) + hitbox.unscaledKnockback, 0);
            }
            else
            {
                knockback = new Vector2((hitbox.scaledKnockback / 2) + hitbox.unscaledKnockback, 0);
            }

            var x = (Mathf.Cos(angle) * knockback.x) - (Mathf.Sin(angle) * knockback.y);
            var y = (Mathf.Sin(angle) * knockback.x) + (Mathf.Cos(angle) * knockback.y);
            Vector2 final = new Vector2(x * (hitbox.directionIndependentAngle ? 1 : direction), y);

            attacker.combo++;

            if (hitstun > 0)
            {
                velocity = Vector2.zero;
            }
            velocity += final;
            if (attacker.GetComponent<PlayerScript>() != null)
            {
                GameObject.Instantiate(attacker.GetHitEffect(), transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg));
                multiplayer.cameraScript.ShakeCamera(final / 50);
            }
            if (hitbox.guaranteeLaunch || (velocity.y > 0 || !grounded) || multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)
            {
                launched = true;
                TestForBounce();
            }
            controller.Move(Vector3.zero);

            Vector3 attractPosition = Vector3.MoveTowards(transform.position, hitPoint, hitbox.attraction);
            controller.Move(attractPosition - transform.position);

            controller.Move(Vector3.zero);

            grounded = false;

            if (hitstun > 0)
            {
                model.animator.Play("Grounded");
                model.animator.Play("Hitstun");
            }
        }
    }

    public bool TestForBounce()
    {
        bool bounced = false;
        RaycastHit hit;
        int LayersToHit = LayerMask.GetMask("Default");
        if (multiplayer.gamemode != MultiplayerManager.gamemodeType.Traditional && (velocity.magnitude > 10 + hitCombo && Physics.Raycast(transform.position, velocity, out hit, Mathf.Clamp(velocity.magnitude / 10, 1, 5), LayersToHit)))
        {
            Vector3 newVelocity = Vector3.Reflect(velocity, hit.normal) / Mathf.Max(1.125f, hit.normal.y * (Mathf.Max(0, 100 - damage) / 100) * 2);
            velocity = new Vector2(newVelocity.x, newVelocity.y);
            hitstop = Mathf.Max(hitstop, 0.05f);
            bounced = true;
            controller.Move(Vector3.zero);
        }
        return bounced;
    }

    public void TestForBlastZones()
    {

        bool pastBlastZones = false;

        if (Mathf.Abs(transform.position.x) > (multiplayer.blastZone.x / 2) + (multiplayer.activeBlastZones ? 1 : -0.5f))
        {
            pastBlastZones = true;
        }
        if ((hitstun > 0 || launched || tumbling) && transform.position.y > (multiplayer.blastZone.y / 2) + 1)
        {
            pastBlastZones = true;
        }
        if (transform.position.y < ((-1 * multiplayer.blastZone.y) / 2) - 1)
        {
            pastBlastZones = true;
        }

        if (blastCooldown > 0)
        {
            if (stocks > 1)
            {
                Vector2 dir = Vector3.zero - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.position = Vector3.zero;
                velocity = Vector3.zero;
                damage = 0;
                transform.position = new Vector3(0, 0, 0);
                blastCooldown = 0;
                ResetPlayer();
            }
        }
        else if (!unconscious && blastCooldown < 0)
        {
            controller.enabled = false;
            if (!multiplayer.activeBlastZones)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, (multiplayer.blastZone.x * -0.5f) + 0.5f, (multiplayer.blastZone.x * 0.5f) - 0.5f), transform.position.y, transform.position.z);
            }
            else if (multiplayer.activeBlastZones && pastBlastZones)
            {
                if (multiplayer.endOfMatch)
                {
                    transform.position = spawnPoint;
                    velocity = Vector3.zero;
                    damage = 0;
                    transform.position = spawnPoint;
                    ResetPlayer();
                }
                else if (stocks > 1 && !multiplayer.endOfRound)
                {
                    stocks--;
                    Vector2 dir = Vector3.zero - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Instantiate(multiplayer.blastEffectPrefab, transform.position, Quaternion.Euler(0, 0, angle - 90));
                    transform.position = Vector3.zero;
                    velocity = Vector3.zero;
                    damage = 0;
                    transform.position = new Vector3(0, 0, 0);
                    print(transform.position);
                    blastCooldown = 1;
                    ResetPlayer();
                }
                else
                {
                    stocks = 0;
                    Vector2 dir = Vector3.zero - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    Instantiate(multiplayer.blastEffectPrefab, transform.position, Quaternion.Euler(0, 0, angle - 90));
                    if (multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional)
                    {
                        damage = health;
                    }
                    unconscious = true;
                    blastCooldown = 1;
                    ResetPlayer();
                }
            }
            controller.enabled = true;
        }
    }

    public void Jump()
    {
        sprinting = false;
        velocity.x = fighter.walkSpeed * input.GetLeftStickX();
        if (!grounded)
        {
            DoubleJump();
        }
        velocity.x = Mathf.Clamp(velocity.x, fighter.walkSpeed * -1, fighter.walkSpeed);
        velocity.y = fighter.jumpHeight;
        grounded = false;
    }

    public void DoubleJump()
    {
        launched = false;
        doubleJumps--;
        fastFall = true;
        velocity.x = fighter.runSpeed * input.GetLeftStickX();
        if (input.GetLeftStickX() == 0)
        {
            model.animator.Play("Front Double Jump", 0);
        }
        else if (direction * Mathf.Infinity == input.GetLeftStickX() * Mathf.Infinity)
        {
            model.animator.Play("Front Double Jump", 0);
        }
        else
        {
            model.animator.Play("Back Double Jump", 0);
        }
        model.overrideXVelocity = false;
        model.overrideYVelocity = false;
        model.velocity = Vector2.zero;

        if (getupTest)
        {
            model.animator.Play("Launched");
        }
    }

    public void MoveAway()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position - (Vector3.up * controller.height * 0.5f), controller.radius, Vector3.up, controller.height, LayerMask.GetMask("Player"));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform != transform)
            {
                print(hit.transform.gameObject.name);

                if (hit.transform.position.x > transform.position.x)
                {
                    controller.Move((Vector3.left * Time.deltaTime * fighter.walkSpeed / 2) + (grounded ? Vector3.down * 0.25f : Vector3.zero));
                }
                if (hit.transform.position.x < transform.position.x)
                {
                    controller.Move((Vector3.right * Time.deltaTime * fighter.walkSpeed / 2) + (grounded ? Vector3.down * 0.25f : Vector3.zero));
                }
            }
        }

        if (opponent != null)
        {
            if (opponent.unactionable > 0 || opponent.hitstun > 0 || opponent.hitstop > 0)
            {
                if (Mathf.Abs(opponent.transform.position.x - transform.position.x) < 0.5f)
                {
                    if (opponent.transform.position.x > transform.position.x)
                    {
                        controller.Move((Vector3.left * Time.deltaTime * fighter.walkSpeed) + (grounded ? Vector3.down * 0.25f : Vector3.zero));
                    }
                    if (opponent.transform.position.x < transform.position.x)
                    {
                        controller.Move((Vector3.right * Time.deltaTime * fighter.walkSpeed) + (grounded ? Vector3.down * 0.25f : Vector3.zero));
                    }
                }
            }
        }
    }

    public void FaceDirection(float xPosition)
    {
        if (transform.position.x < xPosition)
        {
            direction = 1;
        }
        else if (transform.position.x > xPosition)
        {
            direction = -1;
        }
    }

    public bool TestForGround(Vector3 additionalPosition)
    {
        bool value = false;
        if (Physics.Raycast(transform.position + additionalPosition, Vector3.down, 1f * controller.height, LayerMask.GetMask("Default", "Platform")))
        {
            value = true;
        }
        return value;
    }

    public override GameObject GetHitEffect()
    {
        GameObject newHitEffect = base.GetHitEffect();

        if (fighter.currentMove != null ? fighter.currentMove.hitboxes[fighter.hitHitbox].hitEffect != null : false)
        {
            newHitEffect = fighter.currentMove.hitboxes[fighter.hitHitbox].hitEffect;
        }
        else if (fighter.hitEffect != null)
        {
            newHitEffect = fighter.hitEffect;
        }

        return newHitEffect;
    }

    public float GetMovementAngle()
    {
        Vector2 dir = velocity * new Vector2(direction, direction);
        if (grounded)
        {
            dir.y = 0;
        }
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    public void ResetPlayer()
    {
        model.animator.Play("Grounded");
        fighter.currentMove = null;
        fighter.ResetFighter();
    }
}
