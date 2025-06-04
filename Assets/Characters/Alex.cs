using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alex : Fighter
{
    public bool hasBounce;

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

        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shield Surf"))
        {
            player.fastFall = false;
            if (player.velocity.y < 0)
            {
                hitboxes[0].currentlyHitThings.Clear();
            }
            if (player.grounded && hasBounce)
            {
                player.velocity.y = 10;
                hasBounce = false;
            }
            else if (Mathf.Abs(player.velocity.x) < 1 && player.velocity.y < 0 && player.grounded)
            {
                model.animator.Play("Grounded");
            }
        }
        if (currentMove != null ? currentMove.name == "Down Air" || currentMove.name == "Forward Special" : false)
        {
            currentMove.hitboxes[0].angle = player.GetMovementAngle() * player.direction;
        }

        print(player.playerID + player.GetMovementAngle());
    }

    public override bool CanSetAttack(string attackName)
    {
        bool canSetAttack = true;

        if (attackName == "A2A" || attackName == "6B")
        {
            if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Down Air") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Side Special") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shield Surf"))
            {
                canSetAttack = false;
            }
        }

        return canSetAttack;
    }

    public override void OnHit(string moveName)
    {
        base.OnHit(moveName);

        if (moveName == "Down Air" || moveName == "Forward Special")
        {
            player.velocity.y = 10;
            hasBounce = false;
        }
    }

    public override void OnSetAttack()
    {
        base.OnSetAttack();

        if (currentMove.name == "Down Air" || currentMove.name == "Forward Special")
        {
            hasBounce = true;
        }
    }
}
