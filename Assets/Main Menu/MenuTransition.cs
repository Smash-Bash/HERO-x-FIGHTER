using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTransition : MonoBehaviour
{
    public MainMenu menu;
    public Image transitionImage;
    public MenuScreen from;
    public MenuScreen to;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (from != null && to != null)
        {
            transitionImage.color = Vector4.MoveTowards(transitionImage.color, new Vector4(0, 0, 0, 255), Time.deltaTime * 4);
            transitionImage.raycastTarget = true;
            if (transitionImage.color.a >= 1)
            {
                foreach (GameObject background in menu.allBackgrounds)
                {
                    background.SetActive(false);
                }
                if (to.background != null)
                {
                    to.background.SetActive(true);
                }
                from.gameObject.SetActive(false);
                to.gameObject.SetActive(true);
                //foreach (MenuPlayer player in menu.multiplayer.allPlayers)
                //{
                //    player.selectedButton = to.defaultButton;
                //}
                //if (to.description != null && to.defaultButton != null)
                //{
                //    to.description.text = to.defaultButton.description;
                //}
                from = null;
                to = null;
            }
        }
        else
        {
            transitionImage.color = Vector4.MoveTowards(transitionImage.color, new Vector4(0, 0, 0, 0), Time.deltaTime * 4);
            transitionImage.raycastTarget = false;
        }
    }

    public void Transition(MenuScreen fromObject, MenuScreen toObject)
    {
        from = fromObject;
        to = toObject;

        if (GameSystems.eventMode)
        {
            if (to == from.previousMenu)
            {
                while (to.skipInEventMode && to.previousMenu != null)
                {
                    to = to.previousMenu;
                }
            }
            else
            {
                while (to.skipInEventMode && to.nextMenu != null)
                {
                    to = to.nextMenu;
                }
            }
        }

        //bool gamepadTaken = false;
        //foreach (MenuPlayer player in menu.multiplayer.players)
        //{
        //    if (player.gamepadID != 0)
        //    {
        //        gamepadTaken = true;
        //    }
        //}

        //menu.multiplayer.menuPrompt.text = toObject.menuPrompt;
    }
}
