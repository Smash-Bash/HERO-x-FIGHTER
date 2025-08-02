using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTutorialScreen : MenuScreen
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();

        if (mainMenu.input.GetAttackDown())
        {
            mainMenu.ChangeMenu(nextMenu.gameObject.name);
        }
        else if (mainMenu.input.GetSpecialDown())
        {
            mainMenu.ChangeMenu(previousMenu.gameObject.name);
        }
    }
}
