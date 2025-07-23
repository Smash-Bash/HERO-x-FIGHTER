using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public PlayerScript player;
    public PlayerModel model;

    public string characterName;
    public string fullName;

    public Moveset moveset;
    public float groundedAcceleration = 5f;
    public float aerialAcceleration = 2f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 10f;
    public float gravity = 25f;
    public float maxGravity = 10f;
    public int maxDoubleJumps = 1;
    public int maxAirDodges = 1;

    [Header("Runtime")]
    public bool hitAttack;
    public bool grabbedOpponent;

    [Header("References")]
    public Sprite displayIcon;
    public Sprite stockIcon;
    public Move currentMove;
    public Hitbox[] hitboxes;
    public Grabbox[] grabboxes;
    public ProjectileSpawner[] projectileSpawners;
    public GameObject hitEffect;
    public bool hitPlayer;
    public int hitHitbox;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GetComponent<PlayerScript>();
        model = player.model;
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void FighterUpdate(PlayerScript player)
    {
        model = player.model;

        if ((player.free || player.tumbling) && !player.pratfall && player.unactionable <= 0 && !player.landing && !player.gettingUp)
        {
            currentMove = null;
            GetAttackInput(player, false);
            hitPlayer = false;
        }
        else if (model.allowFollowup && !player.pratfall && player.unactionable <= 0 && !player.landing && !player.gettingUp)
        {
            GetAttackInput(player, true);

            if (currentMove != null ? currentMove.nextMoves == null : false)
            {
                player.input.attackBuffer = 0;
                player.input.specialBuffer = 0;
            }
        }

        grabbedOpponent = false;
        if (!player.free)
        {
            TestForHits();
        }
        else
        {
            hitAttack = false;
        }

        GetAttack();
    }

    public virtual bool CustomAI(ComputerInput AI)
    {
        return false;
    }

    public void TestForHits()
    {
        if (currentMove == null)
        {
            return;
        }

        foreach (Grabbox grabbox in grabboxes)
        {
            if (grabbox.isActiveAndEnabled && grabbox.target == null)
            {
                grabbox.Attack(player);
            }
            if (grabbox.target != null)
            {
                grabbedOpponent = true;
            }
        }

        List<Entity> hitEntities = new List<Entity>();
        foreach (Hitbox hitbox in hitboxes)
        {
            if (hitbox.isActiveAndEnabled)
            {
                List<Entity> currentHitEntities = hitbox.Attack(player);
                foreach (Entity entity in currentHitEntities)
                {
                    if (!hitEntities.Contains(entity))
                    {
                        hitHitbox = hitbox.hitboxNum;
                        print(entity.gameObject);
                        hitEntities.Add(entity);
                        entity.HitboxDamage(currentMove.hitboxes[hitbox.hitboxNum], player, hitbox.transform.position, player.direction, hitbox.transform.eulerAngles.x);
                        if (!entity.invincible)
                        {
                            player.hitstop = Mathf.Max(player.hitstop, currentMove.hitboxes[hitbox.hitboxNum].hitstop * currentMove.hitboxes[hitbox.hitboxNum].attackerHitstopMultiplier);
                            OnHit(currentMove.name);
                            hitAttack = true;
                        }
                        hitPlayer = true;
                    }
                }
            }
        }
    }

    public virtual void SetAttack(string attackName, bool followup = false)
    {
        player.sprinting = false;
        if (followup && attackName != "" && attackName != null)
        {
            foreach (Move move in moveset.moves)
            {
                if (currentMove.nextMoves.Contains(move.name) || (currentMove.nextMoves.Contains("Aerials") && move.name.Contains("Air")))
                {
                    if (attackName == move.input || attackName == move.name)
                    {
                        if (CanSetAttack(attackName))
                        {
                            foreach (Hitbox hitbox in hitboxes)
                            {
                                hitbox.currentlyHitThings.Clear();
                                hitbox.gameObject.SetActive(false);
                            }
                            player.input.attackBuffer = 0;
                            player.input.specialBuffer = 0;
                            player.launched = false;
                            hitAttack = false;
                            model.allowFollowup = false;
                            if (currentMove.name == move.name)
                            {
                                model.animator.SetTrigger("DoItAgain");
                            }
                            else
                            {
                                model.animator.Play(move.animationName, 0);
                            }
                            SetMoveData(move);
                            if (move.name.Contains("Special") && player.input.GetLeftStickX() != 0)
                            {
                                if (player.input.GetLeftStickX() > 0.1f)
                                {
                                    player.direction = 1;
                                }
                                else if (player.input.GetLeftStickX() < -0.1f)
                                {
                                    player.direction = -1;
                                }
                            }
                            OnSetAttack();
                            break;
                        }
                    }
                }
            }
        }
        else if (attackName != "" && attackName != null)
        {
            foreach (Move move in moveset.moves)
            {
                if (move.input == attackName || attackName == move.name)
                {
                    if (CanSetAttack(attackName))
                    {
                        foreach (Hitbox hitbox in hitboxes)
                        {
                            hitbox.currentlyHitThings.Clear();
                        }
                        player.input.attackBuffer = 0;
                        player.input.specialBuffer = 0;
                        player.launched = false;
                        hitAttack = false;
                        SetMoveData(move);
                        model.animator.Play(move.animationName, 0);
                        OnSetAttack();
                        break;
                    }
                }
            }
        }
    }

    public virtual void GetAttack()
    {
        foreach (Move move in moveset.moves)
        {
            if (model.animator.GetCurrentAnimatorStateInfo(0).IsName(move.animationName) && (currentMove != null ? currentMove.name != move.name : true))
            {
                SetMoveData(move);
            }
        }
    }

    public virtual bool CanSetAttack(string attackName)
    {
        return true;
    }

    void GetAttackInput(PlayerScript player, bool followup = false)
    {
        Vector2 input = Vector2.zero;
        Vector2 leftStick = player.input.GetLeftStick();
        Vector2 rightStick = player.input.GetRightStick();
        int direction = 0;
        bool rightStickInput = false;
        if (rightStick.magnitude > 0.25f)
        {
            input = rightStick;
            rightStickInput = true;
        }
        else
        {
            input = leftStick;
        }
        direction = GetCardinalDirection(input, player);
        if (followup && player.grounded)
        {
            if (!rightStickInput)
            {
                if (player.input.GetStickFlick() && Mathf.Abs(input.x) > 0.25f)
                {
                    player.model.animator.Play("Grounded");
                    if (player.input.GetLeftStickX() * Mathf.Infinity != player.direction * Mathf.NegativeInfinity)
                    {
                        player.velocity.x = player.fighter.runSpeed * input.x;
                    }
                    player.sprinting = true;
                }
            }

            if (player.input.GetJumpDown())
            {
                player.input.jumpBuffer = 0;
                model.animator.SetBool("Grounded", false);
                model.animator.Play("Airborne");
                model.overrideXVelocity = false;
                model.overrideYVelocity = false;
                model.velocity = Vector2.zero;
                player.Jump();
                player.grounded = false;
                player.free = true;
                player.launched = false;
            }

            if (input.x > 0.1f)
            {
                player.direction = 1;
            }
            else if (input.x < -0.1f)
            {
                player.direction = -1;
            }
        }
        if (player.input.GetAttackDown() || rightStickInput)
        {
            if (player.grounded)
            {
                if (direction == 4)
                {
                    player.direction *= -1;
                }
                SetAttack("G" + direction + "A", followup);
            }
            else
            {
                SetAttack("A" + direction + "A", followup);
            }
        }
        else if (player.input.GetSpecialDown())
        {
            print(direction);
            if (player.multiplayer.gamemode == MultiplayerManager.gamemodeType.Traditional && direction == 4)
            {
                direction = 8;
            }
            else if (input.x > 0.25f)
            {
                player.direction = 1;
                direction = GetDirection(input, player);
            }
            else if (input.x < -0.25f)
            {
                player.direction = -1;
                direction = GetDirection(input, player);
            }
            SetAttack(direction + "B", followup);
        }
    }

    int GetDirection(Vector2 input, PlayerScript player)
    {
        int direction = 5;
        input.x = Mathf.MoveTowards(input.x * player.direction, 0, 0.1f);
        input.y = Mathf.MoveTowards(input.y, 0, 0.1f);
        if (Mathf.Abs(input.x) < 0.25f && Mathf.Abs(input.y) < 0.25f)
        {
            direction = 5;
        }
        else if (input.x < -0.25f && input.y < -0.25f)
        {
            direction = 2;
        }
        else if (Mathf.Abs(input.x) < 0.25f && input.y < -0.25f)
        {
            direction = 2;
        }
        else if (input.x > 0.25f && input.y < -0.25f)
        {
            direction = 2;
        }
        else if (input.x < -0.25f && Mathf.Abs(input.y) < 0.25f)
        {
            direction = 4;
        }
        else if (input.x > 0.25f && Mathf.Abs(input.y) < 0.25f)
        {
            direction = 6;
        }
        else if (input.x < -0.25f && input.y > 0.25f)
        {
            direction = 8;
        }
        else if (Mathf.Abs(input.x) < 0.25f && input.y > 0.25f)
        {
            direction = 8;
        }
        else if (input.x > 0.25f && input.y > 0.25f)
        {
            direction = 8;
        }
        return direction;
    }

    int GetCardinalDirection(Vector2 input, PlayerScript player)
    {
        int direction = 5;
        input.x = Mathf.MoveTowards(input.x * player.direction, 0, 0.1f);
        input.y = Mathf.MoveTowards(input.y, 0, 0.1f);
        if (Mathf.Abs(input.x) < 0.1f && Mathf.Abs(input.y) < 0.1)
        {
            direction = 5;
        }
        else if (input.y <= Mathf.Abs(input.x) * -1)
        {
            direction = 2;
        }
        else if (input.y >= Mathf.Abs(input.x))
        {
            direction = 8;
        }
        else if (input.x <= Mathf.Abs(input.y) * -1)
        {
            direction = 4;
        }
        else if (input.x >= Mathf.Abs(input.y))
        {
            direction = 6;
        }
        return direction;
    }

    public virtual void OnHit(string moveName)
    {

    }

    public virtual bool OnDamage(Entity other)
    {
        bool cancel = false;

        return cancel;
    }

    public virtual void OnDamageRecieved(float damage)
    {

    }

    public virtual void OnDamageDealt(float damage)
    {

    }

    public virtual void OnSetAttack()
    {

    }

    public void SetMoveData(Move move)
    {
        if (currentMove == null)
        {
            currentMove = new Move();
        }
        currentMove.name = move.name;
        currentMove.animationName = move.name;
        currentMove.input = move.input;
        //currentMove.grounded = move.grounded;
        if (move.grounded)
        {
            currentMove.grounded = true;
        }
        else
        {
            currentMove.grounded = false;
        }
        //currentMove.airborne = move.grounded;
        if (move.airborne)
        {
            currentMove.airborne = true;
        }
        else
        {
            currentMove.airborne = false;
        }
        print(move.superArmour);
        currentMove.superArmour = move.superArmour;
        List<string> nextMoves = new List<string>();
        foreach (string newMove in move.nextMoves)
        {
            nextMoves.Add(newMove);
        }
        currentMove.nextMoves = nextMoves.ToArray();
        List<HitboxInfo> nextHitboxes = new List<HitboxInfo>();
        foreach (HitboxInfo hitbox in move.hitboxes)
        {
            HitboxInfo newHitbox = new HitboxInfo();
            newHitbox.damage = hitbox.damage;
            newHitbox.hitstun = hitbox.hitstun;
            newHitbox.hitstop = hitbox.hitstop;
            newHitbox.attackerHitstopMultiplier = hitbox.attackerHitstopMultiplier;
            newHitbox.attraction = hitbox.attraction;
            newHitbox.scaledKnockback = hitbox.scaledKnockback;
            newHitbox.unscaledKnockback = hitbox.unscaledKnockback;
            newHitbox.angle = hitbox.angle;
            newHitbox.guaranteeLaunch = hitbox.guaranteeLaunch;
            newHitbox.forwardDependentAngle = hitbox.forwardDependentAngle;
            newHitbox.directionIndependentAngle = hitbox.directionIndependentAngle;
            newHitbox.type = hitbox.type;
            newHitbox.hitEffect = hitbox.hitEffect;
            nextHitboxes.Add(newHitbox);
        }
        currentMove.hitboxes = nextHitboxes.ToArray();
    }

    public virtual void ResetFighter()
    {

    }

    public virtual void OnProjectileSpawn(int ID)
    {

    }

    public virtual void InputReverse()
    {
        if (player.input.GetLeftStickX() > 0)
        {
            player.direction = 1;
        }
        else if (player.input.GetLeftStickX() < 0)
        {
            player.direction = -1;
        }
    }

    public virtual void OnWallJump()
    {

    }
}

[System.Serializable]
public class DialogueInteraction
{
    public string otherFighter;
    public Dialogue[] dialogue;
}