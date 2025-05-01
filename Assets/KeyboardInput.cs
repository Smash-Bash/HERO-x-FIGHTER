using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : PlayerInput
{
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
        return isActiveAndEnabled && (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude >= 0.9f && Input.GetKey(KeyCode.LeftShift));
    }

    public override Vector2 GetLeftStick()
    {
        return (isActiveAndEnabled ? new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized : Vector2.zero);
    }

    public override float GetLeftStickX()
    {
        return (isActiveAndEnabled ? Input.GetAxisRaw("Horizontal") : 0);
    }

    public override float GetLeftStickY()
    {
        return (isActiveAndEnabled ? Input.GetAxisRaw("Vertical") : 0);
    }

    public override Vector2 GetRightStick()
    {
        return (isActiveAndEnabled ? Vector2.zero : Vector2.zero);
    }

    public override float GetRightStickX()
    {
        return (isActiveAndEnabled ? 0 : 0);
    }

    public override float GetRightStickY()
    {
        return (isActiveAndEnabled ? 0 : 0);
    }

    public override bool GetAttack()
    {
        return isActiveAndEnabled && (Input.GetMouseButton(0));
    }

    public override bool GetAttackDown()
    {
        bool buffered = false;
        if (attackBuffer > 0)
        {
            buffered = true;
        }
        return isActiveAndEnabled && (Input.GetMouseButtonDown(0) || buffered);
    }

    public override bool GetAttackUp()
    {
        return isActiveAndEnabled && (Input.GetMouseButtonUp(0));
    }

    public override bool GetSpecial()
    {
        return isActiveAndEnabled && (Input.GetMouseButton(1));
    }

    public override bool GetSpecialDown()
    {
        bool buffered = false;
        if (specialBuffer > 0)
        {
            buffered = true;
        }
        return isActiveAndEnabled && (Input.GetMouseButtonDown(1) || buffered);
    }

    public override bool GetSpecialUp()
    {
        return isActiveAndEnabled && (Input.GetMouseButtonUp(1));
    }

    public override bool GetJump()
    {
        return isActiveAndEnabled && (Input.GetKey(KeyCode.Space));
    }

    public override bool GetJumpDown()
    {
        bool buffered = false;
        if (jumpBuffer > 0)
        {
            buffered = true;
        }
        return isActiveAndEnabled && (Input.GetKeyDown(KeyCode.Space) || buffered);
    }

    public override bool GetJumpUp()
    {
        return isActiveAndEnabled && (Input.GetKeyUp(KeyCode.Space));
    }

    public override bool GetShield()
    {
        return isActiveAndEnabled && (Input.GetKey(KeyCode.LeftControl));
    }

    public override bool GetStartDown()
    {
        return isActiveAndEnabled && (Input.GetKeyDown(KeyCode.Return));
    }
}
