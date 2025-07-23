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
    public bool allowBacktracking = true;
    public Image backtrackWheel;
    public TMP_Text description;
    public bool skipInEventMode;
    public MenuScreen nextMenu;
    public MenuScreen previousMenu;
    public Scrollbar scrollbar;
    [TextArea(2, 2)]
    public string menuPrompt;
    public GameObject background;

    // Start is called before the first frame update
    public virtual void Start()
    {
        multiplayer = FindFirstObjectByType<MenuMultiplayerManager>();
        mainMenu = FindFirstObjectByType<MainMenu>();

        if (backtrackWheel != null)
        {
            backtrackWheel.fillAmount = 0;
        }
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

        if (backtrackWheel != null)
        {
            if (mainMenu.input.GetSpecial())
            {
                backtrackWheel.fillAmount = Mathf.MoveTowards(backtrackWheel.fillAmount, 1, Time.deltaTime * 0.5f);
            }
            else
            {
                backtrackWheel.fillAmount = Mathf.MoveTowards(backtrackWheel.fillAmount, 0, Time.deltaTime * 0.5f);
            }
        }

        if (allowBacktracking && previousMenu != null && (backtrackWheel ? backtrackWheel.fillAmount >= 1 : true))
        {
            if (mainMenu.input.GetSpecialDown() || backtrackWheel.fillAmount >= 1)
            {
                mainMenu.ChangeMenu(previousMenu.gameObject.name);
                if (backtrackWheel != null)
                {
                    backtrackWheel.fillAmount = 0;
                }
            }
        }
    }
}
