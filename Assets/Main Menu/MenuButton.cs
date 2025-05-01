using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour
{
    [HideInInspector]
    public MainMenu mainMenu;
    public MenuPlayer lastInteractor;
    public Button button;
    public Toggle toggle;
    public bool allowButtonOrToggleActivation = true;

    public string description;

    public MenuButton northButton;
    public MenuButton southButton;
    public MenuButton westButton;
    public MenuButton eastButton;

    // NOTE TO FUTURE ME: THIS IS SO YOU CAN ASSIGN FOR INSTANCE: 'Parkour' or 'Grapple' to allow for shoulder button menu switching.
    public string shortcutInput;

    // This is only used in certain menus like the Character Select Screen.
    public int ID;

    // Start is called before the first frame update
    public virtual void Start()
    {
        mainMenu = FindFirstObjectByType<MainMenu>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (shortcutInput != "")
        {
            if (mainMenu != null)
            {
                foreach (MenuPlayer player in mainMenu.multiplayer.players)
                {
                    if (Input.GetButtonDown(player.GetPlayerID() + shortcutInput))
                    {
                        if (button != null)
                        {
                            ExecuteEvents.Execute(button.gameObject, new BaseEventData(FindObjectOfType<EventSystem>()), ExecuteEvents.submitHandler);
                        }

                        if (toggle != null)
                        {
                            ExecuteEvents.Execute(toggle.gameObject, new BaseEventData(FindObjectOfType<EventSystem>()), ExecuteEvents.submitHandler);
                        }
                    }
                }
            }
        }
    }

    public virtual void ButtonUpdate(MenuPlayer player)
    {

    }
}
