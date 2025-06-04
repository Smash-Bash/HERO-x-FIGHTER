using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadInput : PlayerInput
{
    [Range(1, 4)]
    public int gamepadID;

    public int attackButton = 0;
    public int specialButton = 1;
    public int jumpButton = 2;
    public int altJumpButton = 3;
    public int shieldButton = 4;
    public int altShieldButton = 5;
    public int startButton = 7;
    public bool leftStickFlick;
    private float leftStickFlickTime;
    public Vector2 leftStickLerp;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (Vector2.Distance(new Vector2(Input.GetAxisRaw(gamepadID + " Horizontal"), Input.GetAxisRaw(gamepadID + " Vertical")), leftStickLerp) > 0.75f)
        {
            leftStickFlickTime = 0.1f;
        }
        else
        {
            leftStickFlickTime -= Time.deltaTime;
        }
        leftStickLerp = Vector2.Lerp(leftStickLerp, new Vector2(Input.GetAxisRaw(gamepadID + " Horizontal"), Input.GetAxisRaw(gamepadID + " Vertical")), Time.deltaTime * 10);
    }

    public override bool GetStickFlick()
    {
        return isActiveAndEnabled && (new Vector2(Input.GetAxisRaw(gamepadID + " Horizontal"), Input.GetAxisRaw(gamepadID + " Vertical")).magnitude >= 0.9f && leftStickFlickTime > 0);
    }

    public override Vector2 GetLeftStick()
    {
        return (isActiveAndEnabled ? new Vector2(Input.GetAxisRaw(gamepadID + " Horizontal"), Input.GetAxisRaw(gamepadID + " Vertical")) : Vector2.zero);
    }

    public override float GetLeftStickX()
    {
        return (isActiveAndEnabled ? Input.GetAxisRaw(gamepadID + " Horizontal") : 0);
    }

    public override float GetLeftStickY()
    {
        return (isActiveAndEnabled ? Input.GetAxisRaw(gamepadID + " Vertical") : 0);
    }

    public override Vector2 GetRightStick()
    {
        return (isActiveAndEnabled ? new Vector2(Input.GetAxisRaw(gamepadID + " RS Horizontal"), Input.GetAxisRaw(gamepadID + " RS Vertical")) : Vector2.zero);
    }

    public override float GetRightStickX()
    {
        return (isActiveAndEnabled ? Input.GetAxisRaw(gamepadID + " RS Horizontal") : 0);
    }

    public override float GetRightStickY()
    {
        return (isActiveAndEnabled ? Input.GetAxisRaw(gamepadID + " RS Vertical") : 0);
    }

    public override bool GetAttack()
    {
        return isActiveAndEnabled && Input.GetKey("joystick " + gamepadID + " button " + attackButton);
    }

    public override bool GetAttackDown()
    {
        bool buffered = false;
        if (attackBuffer > 0 && !ignoreBuffers)
        {
            buffered = true;
        }
        return isActiveAndEnabled && Input.GetKeyDown("joystick " + gamepadID + " button " + attackButton) || buffered;
    }

    public override bool GetAttackUp()
    {
        return isActiveAndEnabled && Input.GetKeyUp("joystick " + gamepadID + " button " + attackButton);
    }

    public override bool GetSpecial()
    {
        return isActiveAndEnabled && Input.GetKey("joystick " + gamepadID + " button " + specialButton);
    }

    public override bool GetSpecialDown()
    {
        bool buffered = false;
        if (specialBuffer > 0 && !ignoreBuffers)
        {
            buffered = true;
        }
        return isActiveAndEnabled && Input.GetKeyDown("joystick " + gamepadID + " button " + specialButton) || buffered;
    }

    public override bool GetSpecialUp()
    {
        return isActiveAndEnabled && Input.GetKeyUp("joystick " + gamepadID + " button " + specialButton);
    }

    public override bool GetJump()
    {
        return isActiveAndEnabled && Input.GetKey("joystick " + gamepadID + " button " + jumpButton) || Input.GetKey("joystick " + gamepadID + " button " + altJumpButton);
    }

    public override bool GetJumpDown()
    {
        bool buffered = false;
        if (jumpBuffer > 0 && !ignoreBuffers)
        {
            buffered = true;
        }
        return isActiveAndEnabled && Input.GetKeyDown("joystick " + gamepadID + " button " + jumpButton) || Input.GetKeyDown("joystick " + gamepadID + " button " + altJumpButton) || buffered;
    }

    public override bool GetJumpUp()
    {
        return isActiveAndEnabled && Input.GetKeyUp("joystick " + gamepadID + " button " + jumpButton) || Input.GetKeyUp("joystick " + gamepadID + " button " + altJumpButton);
    }

    public override bool GetShield()
    {
        return isActiveAndEnabled && Input.GetKey("joystick " + gamepadID + " button " + shieldButton) || Input.GetKey("joystick " + gamepadID + " button " + altShieldButton);
    }

    public override bool GetStartDown()
    {
        return isActiveAndEnabled && (Input.GetKeyDown("joystick " + gamepadID + " button " + startButton));
    }
}
