using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerScript player;
    public float attackBuffer;
    public float specialBuffer;
    public float jumpBuffer;
    public bool ignoreBuffers;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (player ? true : true)
        {
            attackBuffer -= Time.deltaTime;
            specialBuffer -= Time.deltaTime;
            jumpBuffer -= Time.deltaTime;
        }
    }

    public virtual bool GetStickFlick()
    {
        return false;
    }

    public virtual Vector2 GetLeftStick()
    {
        return Vector2.zero;
    }

    public virtual float GetLeftStickX()
    {
        return 0;
    }

    public virtual float GetLeftStickY()
    {
        return 0;
    }

    public virtual Vector2 GetRightStick()
    {
        return Vector2.zero;
    }

    public virtual float GetRightStickX()
    {
        return 0;
    }

    public virtual float GetRightStickY()
    {
        return 0;
    }

    public virtual bool GetAttack()
    {
        return false;
    }

    public virtual bool GetAttackDown()
    {
        return false;
    }

    public virtual bool GetAttackUp()
    {
        return false;
    }

    public virtual bool GetSpecial()
    {
        return false;
    }

    public virtual bool GetSpecialDown()
    {
        return false;
    }

    public virtual bool GetSpecialUp()
    {
        return false;
    }

    public virtual bool GetJump()
    {
        return false;
    }

    public virtual bool GetJumpDown()
    {
        return false;
    }

    public virtual bool GetJumpUp()
    {
        return false;
    }

    public virtual bool GetShield()
    {
        return false;
    }

    public virtual bool GetStartDown()
    {
        return false;
    }
}
