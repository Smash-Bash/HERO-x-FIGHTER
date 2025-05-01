using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuScreen : MonoBehaviour
{
    [HideInInspector]
    public MenuMultiplayerManager multiplayer;
    [HideInInspector]
    public MainMenu mainMenu;
    public MenuButton defaultButton;
    public TMP_Text description;
    public bool skipInEventMode;
    public MenuScreen nextMenu;
    public MenuScreen previousMenu;
    public RawImage background;
    public Scrollbar scrollbar;
    [TextArea(2, 2)]
    public string menuPrompt;

    // Start is called before the first frame update
    public virtual void Start()
    {
        multiplayer = FindFirstObjectByType<MenuMultiplayerManager>();
        mainMenu = FindFirstObjectByType<MainMenu>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (scrollbar != null)
        {
            foreach (MenuPlayer player in multiplayer.players)
            {
                scrollbar.value += Input.GetAxisRaw(player.GetPlayerID() + "Arrow Vertical") * Time.deltaTime * 2.5f;
            }
        }
    }
}
