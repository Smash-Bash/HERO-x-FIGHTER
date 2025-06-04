using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : MenuScreen
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        animator.keepAnimatorStateOnDisable = true;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Logo") && (mainMenu.input.GetAttackDown() || mainMenu.input.GetSpecialDown() || mainMenu.input.GetJumpDown()))
        {
            mainMenu.ChangeMenu("Stage Select Screen");
        }
    }
}
