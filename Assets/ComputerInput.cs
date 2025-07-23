using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputerInput : PlayerInput
{
    public Entity target;
    public ComputerState state;
    public List<string> options;
    public Vector2 leftStick;
    public string forceMove;
    public bool overrideAttack;
    public bool overrideSpecial;
    public bool overrideJump;
    public bool overrideShield;

    public enum ComputerState
    {
        Attacking, Recovering
    }


    // Start is called before the first frame update
    public virtual void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (options == null)
        {
            options = new List<string>();
        }

        if (player.multiplayer.endOfRound || player.multiplayer.endOfMatch)
        {
            return;
        }

        overrideAttack = false;
        overrideSpecial = false;
        overrideJump = false;
        overrideShield = false;

        UpdateTarget();

        UpdateOptions();

        EvaluateState();

        if (player.fighter.CustomAI(this))
        {

        }
        else if (state == ComputerState.Recovering)
        {
            leftStick = (new Vector3(0, 10, 0) - transform.position).normalized;

            if (player.velocity.y < 0 && player.hitstun <= 0)
            {
                if (player.hasWallJump && player.free ? player.TestForWallJump(true) : false)
                {
                    
                }
                else if (player.doubleJumps > 0 && player.free)
                {
                    player.Jump();
                }
                else if (options.Count > 0 && !player.pratfall)
                {
                    string moveToUse = (forceMove != "" && forceMove != null ? forceMove : options[Random.Range(0, options.Count)]);

                    if (moveToUse.Contains("Special") && GetLeftStickX() != 0)
                    {
                        if (GetLeftStickX() > 0.1f)
                        {
                            player.direction = 1;
                        }
                        else if (GetLeftStickX() < -0.1f)
                        {
                            player.direction = -1;
                        }
                    }
                    if (player.free)
                    {
                        player.fighter.SetAttack(moveToUse, false);
                    }
                    else if (player.model.allowFollowup)
                    {
                        player.fighter.SetAttack(moveToUse, true);
                    }
                }
            }
        }
        else
        {
            leftStick = (target.transform.position - transform.position).normalized;

            if (player.grounded)
            {
                if (leftStick.x > 0)
                {
                    leftStick.x = 1;
                }
                if (leftStick.x < 0)
                {
                    leftStick.x = -1;
                }
                leftStick.y = 0;
            }

            if (options.Count > 0 && !player.pratfall && player.hitstun <= 0)
            {
                if ((player.fighter.currentMove != null ? player.fighter.currentMove.name.Contains("Special") : false) && player.model.allowFollowup)
                {
                    options.Remove("Up Special");
                }

                string moveToUse = (forceMove != "" && forceMove != null ? forceMove : options[Random.Range(0, options.Count)]);

                if (moveToUse == "Forward Air" && !player.InFront(target.transform.position))
                {
                    moveToUse = "Back Air";
                }

                if (moveToUse.Contains("Special") && GetLeftStickX() != 0)
                {
                    if (GetLeftStickX() > 0.1f)
                    {
                        player.direction = 1;
                    }
                    else if (GetLeftStickX() < -0.1f)
                    {
                        player.direction = -1;
                    }
                }
                if (player.fighter.currentMove == null && player.free)
                {
                    if (player.doubleJumps > 0 && moveToUse.Contains("Air") && (player.grounded || (target.transform.position.y > transform.position.y + 1)))
                    {
                        player.Jump();
                    }

                    print(moveToUse);
                    player.fighter.SetAttack(moveToUse, false);
                }
                else if (player.model.allowFollowup)
                {
                    if (player.doubleJumps > 0 && moveToUse.Contains("Air") && (player.grounded || (target.transform.position.y > transform.position.y + 1)))
                    {
                        player.Jump();
                    }

                    player.fighter.SetAttack(moveToUse, true);
                }
            }
        }
    }

    public void UpdateTarget()
    {
        PlayerScript closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (PlayerScript entity in player.multiplayer.players)
        {
            if (entity.isActiveAndEnabled && entity != player && !entity.unconscious)
            {
                Vector3 diff = entity.transform.position - position;
                //float curDistance = diff.sqrMagnitude;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = entity;
                    distance = curDistance;
                }
            }
        }
        target = closest;
    }

    public override Vector2 GetLeftStick()
    {
        return (isActiveAndEnabled ? leftStick : Vector2.zero);
    }

    public override float GetLeftStickX()
    {
        return (isActiveAndEnabled ? leftStick.x : 0);
    }

    public override float GetLeftStickY()
    {
        return (isActiveAndEnabled ? leftStick.y : 0);
    }

    public override bool GetStickFlick()
    {
        return !player.sprinting && player.fighter.currentMove == null && Vector3.Distance(transform.position, target.transform.position) > 2;
    }

    public override bool GetAttack()
    {
        return overrideAttack;
    }

    public override bool GetSpecial()
    {
        return overrideSpecial;
    }

    public override bool GetJump()
    {
        return overrideJump;
    }

    public override bool GetShield()
    {
        return overrideShield;
    }

    public void UpdateOptions()
    {
        options.Clear();
        if (target == null)
        {
            return;
        }

        bool targetGrounded = true;
        if (target.GetComponent<PlayerScript>() ? !target.GetComponent<PlayerScript>().grounded : false)
        {
            targetGrounded = false;
        }

        if (player.fighter.currentMove != null ? player.fighter.currentMove.nextMoves.Length > 0 : false)
        {
            options = new List<string>();
            foreach (string currentOption in player.fighter.currentMove.nextMoves)
            {
                if (player.fighter.moveset.followupOptions.Contains(currentOption))
                {
                    options.Add(currentOption);
                }
            }
            if (player.fighter.currentMove.nextMoves.Contains("Tilts") && (Vector3.Distance(transform.position, target.transform.position) < 3.25f))
            {
                foreach (string currentOption in player.fighter.moveset.followupOptions)
                {
                    if (currentOption.Contains("Tilt"))
                    {
                        options.Add(currentOption);
                    }
                }
            }
            if (player.fighter.currentMove.nextMoves.Contains("Aerials") && (Vector3.Distance(transform.position, target.transform.position) < 3.25f))
            {
                foreach(string currentOption in player.fighter.moveset.followupOptions)
                {
                    if (currentOption.Contains("Air"))
                    {
                        options.Add(currentOption);
                    }
                }
            }
        }
        else if (state == ComputerState.Recovering)
        {
            if (transform.position.y > -2)
            {
                options.AddRange(player.fighter.moveset.highRecoveryOptions);
            }
            else
            {
                options.AddRange(player.fighter.moveset.lowRecoveryOptions);
            }
        }
        else
        {
            if (player.landing || player.gettingUp)
            {
                
            }
            else if (player.grounded)
            {
                if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
                {
                    if (targetGrounded)
                    {
                        options.AddRange(player.fighter.moveset.closeGroundOptions);
                    }
                    else
                    {
                        options.AddRange(player.fighter.moveset.antiAirOptions);
                    }
                }
                else if (Vector3.Distance(transform.position, target.transform.position) < 3.25f)
                {
                    if (targetGrounded)
                    {
                        options.AddRange(player.fighter.moveset.closeGroundOptions);
                        options.AddRange(player.fighter.moveset.mediumGroundOptions);
                    }
                    else
                    {
                        options.AddRange(player.fighter.moveset.antiAirOptions);
                    }
                }
                else if (Vector3.Distance(transform.position, target.transform.position) < 7.5f)
                {
                    if (targetGrounded)
                    {
                        options.AddRange(player.fighter.moveset.longGroundOptions);
                    }
                }
                else
                {
                    options.AddRange(player.fighter.moveset.farGroundOptions);
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
                {
                    options.AddRange(player.fighter.moveset.closeAirOptions);
                    options.AddRange(player.fighter.moveset.mediumAirOptions);
                }
                else if (Vector3.Distance(transform.position, target.transform.position) < 3.25f)
                {
                    if (!targetGrounded)
                    {
                        options.AddRange(player.fighter.moveset.mediumAirOptions);
                    }
                    else
                    {
                        options.AddRange(player.fighter.moveset.antiGroundOptions);
                    }
                }
                else if (Vector3.Distance(transform.position, target.transform.position) < 7.5f)
                {
                    if (!targetGrounded)
                    {
                        options.AddRange(player.fighter.moveset.longAirOptions);
                    }
                }
                else
                {
                    options.AddRange(player.fighter.moveset.farAirOptions);
                }
            }
        }

        if (player.hitstun > 0 || player.unactionable > 0)
        {
            options.Clear();
        }
    }

    public void EvaluateState()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            state = ComputerState.Recovering;
        }
        else
        {
            state = ComputerState.Attacking;
        }
    }
}
