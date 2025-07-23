using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alex : Fighter
{
    public bool hasBounce;
    public bool hasUpAir;
    public bool hasSideSpecial;
    public bool hasUpSpecial;
    public bool magicCharge;
    public GameObject auraObject;
    public GameObject regularTrail;
    public GameObject auraTrail;
    public GameObject explosionPrefab;

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

        auraObject.SetActive(magicCharge);
        regularTrail.SetActive(!magicCharge);
        auraTrail.SetActive(magicCharge);

        if (player.grounded)
        {
            hasUpAir = true;
            hasSideSpecial = true;
            hasUpSpecial = true;
        }

        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special"))
        {
            hasSideSpecial = false;
            hasUpSpecial = false;
        }
        if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shield Surf"))
        {
            player.fastFall = false;
            if (player.velocity.y < 0)
            {
                hitboxes[0].currentlyHitThings.Clear();
            }
            if (player.grounded && hasBounce)
            {
                if (!player.sprinting)
                {
                    player.velocity.y = 10;
                }
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
    }

    public override bool CanSetAttack(string attackName)
    {
        bool canSetAttack = true;

        if (attackName == "5B" || attackName == "Neutral Special")
        {
            if (!magicCharge)
            {
                canSetAttack = false;
                model.animator.Play("Neutral Special Charge");
            }
            else
            {
                magicCharge = false;
            }
        }
        if (attackName == "A8A" || attackName == "Up Air")
        {
            if (hasUpAir)
            {
                hasUpAir = false;
            }
            else
            {
                canSetAttack = false;
            }
        }
        if (attackName == "4B" || attackName == "6B" || attackName == "Forward Special")
        {
            if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Down Air") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Forward Special") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shield Surf"))
            {
                canSetAttack = false;
            }
            if (hasSideSpecial)
            {
                hasSideSpecial = false;
            }
            else
            {
                canSetAttack = false;
            }
        }
        if (attackName == "8B" || attackName == "Up Special")
        {
            if (hasUpSpecial)
            {
                hasUpSpecial = false;
                hasSideSpecial = false;
            }
            else
            {
                canSetAttack = false;
            }
        }
        if (attackName == "A2A" || attackName == "Down Air")
        {
            if (model.animator.GetCurrentAnimatorStateInfo(0).IsName("Down Air") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Forward Special") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special") || model.animator.GetCurrentAnimatorStateInfo(0).IsName("Shield Surf"))
            {
                canSetAttack = false;
            }
        }

        return canSetAttack;
    }

    public override void OnHit(string moveName)
    {
        base.OnHit(moveName);

        if (!model.animator.GetCurrentAnimatorStateInfo(0).IsName("Up Special"))
        {
            if (moveName == "Down Air" || moveName == "Forward Special" || moveName == "Up Special")
            {
                player.velocity.y = 10;
                hasBounce = false;
            }
        }
    }

    public override void OnSetAttack()
    {
        base.OnSetAttack();

        if (currentMove.name == "Up Special")
        {
            player.velocity.y = Mathf.Max(player.velocity.y, 10f);
        }
        if (currentMove.name == "Down Air" || currentMove.name == "Forward Special" || currentMove.name == "Up Special")
        {
            hasBounce = true;
        }
    }

    public override void OnWallJump()
    {
        hasUpAir = true;
        hasSideSpecial = true;
        hasUpSpecial = true;

        base.OnWallJump();
    }

    public void MagicCharge()
    {
        magicCharge = true;
    }

    public void ExplosionJump()
    {
        GameObject.Instantiate(explosionPrefab, transform.position - Vector3.up, transform.rotation).transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        player.Damage(10);
        player.hitstop = 0.1f;
        player.velocity.y = 25;
        player.velocity.x = player.direction * 17.5f;
    }

    public override void ResetFighter()
    {
        hasUpAir = true;
        hasSideSpecial = true;
        hasUpSpecial = true;
        magicCharge = false;
    }
}
