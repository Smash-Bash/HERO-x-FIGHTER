using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalInput : PlayerInput
{
    public PlayerInput[] inputs;

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

    public override bool GetStickFlick()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetStickFlick())
            {
                value = true;
            }
        }
        return isActiveAndEnabled && value;
    }

    public override Vector2 GetLeftStick()
    {
        Vector2 value = Vector2.zero;
        foreach (PlayerInput input in inputs)
        {
            value += input.GetLeftStick();
        }
        return value;
    }

    public override float GetLeftStickX()
    {
        float value = 0;
        foreach (PlayerInput input in inputs)
        {
            value += input.GetLeftStickX();
        }
        return value;
    }

    public override float GetLeftStickY()
    {
        float value = 0;
        foreach (PlayerInput input in inputs)
        {
            value += input.GetLeftStickY();
        }
        return value;
    }

    public override Vector2 GetRightStick()
    {
        Vector2 value = Vector2.zero;
        foreach (PlayerInput input in inputs)
        {
            value += input.GetRightStick();
        }
        return value;
    }

    public override float GetRightStickX()
    {
        float value = 0;
        foreach (PlayerInput input in inputs)
        {
            value += input.GetRightStickX();
        }
        return value;
    }

    public override float GetRightStickY()
    {
        float value = 0;
        foreach (PlayerInput input in inputs)
        {
            value += input.GetRightStickY();
        }
        return value;
    }

    public override bool GetAttack()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetAttack())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetAttackDown()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetAttackDown())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetAttackUp()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetAttackUp())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetSpecial()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetSpecial())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetSpecialDown()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetSpecialDown())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetSpecialUp()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetSpecialUp())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetJump()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetJump())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetJumpDown()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetJumpDown())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetJumpUp()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetJumpUp())
            {
                value = true;
            }
        }
        return value;
    }

    public override bool GetStartDown()
    {
        bool value = false;
        foreach (PlayerInput input in inputs)
        {
            if (input.GetStartDown())
            {
                value = true;
            }
        }
        return value;
    }
}
