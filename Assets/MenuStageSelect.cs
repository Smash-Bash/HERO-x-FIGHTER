using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuStageSelect : MenuScreen
{
    public StageButton[] stages;
    public SSSPuck[] pucks;
    public SSSPlayer[] players;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        bool ready = true;
        foreach (SSSPuck puck in pucks)
        {
            if (puck.isActiveAndEnabled)
            {
                if (puck.currentStageButton == null)
                {
                    ready = false;
                }
            }
        }
        foreach (SSSPlayer player in players)
        {
            if (player.selectedPuck != null)
            {
                ready = false;
            }
        }

        if (ready)
        {
            mainMenu.ChangeMenu("Character Select Screen");
        }
    }
}
