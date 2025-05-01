using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;

public class MenuMultiplayerManager : MonoBehaviour
{
    public MainMenu mainMenu;
    public MenuPlayer[] players;
    public MenuPlayer[] allPlayers;
    public bool[] freeGamepads = new bool[] { true, true, true, true };
    public bool freeKeyboard = true;
    public Image[] playerControlDisplays;
    public Sprite unusedImage;
    public Sprite keyboardImage;
    public Sprite xboxImage;
    public Sprite playstationImage;
    public Sprite switchImage;
    public TMP_Text time;
    public TMP_Text joinPrompt;
    public TMP_Text menuPrompt;

    // Credit: https://www.tutorialsteacher.com/linq/linq-sorting-operators-orderby-orderbydescending
    public void OrderLoadedByPlayerID()
    {
        IList<MenuPlayer> playerList = players;

        var orderByResult = from p in playerList
                            orderby p.playerID
                            select p;

        var orderByAscendingResult = from p in playerList
                                      orderby p.playerID ascending
                                      select p;

        players = orderByAscendingResult.ToArray<MenuPlayer>();
    }
    public void OrderAllByPlayerID()
    {
        IList<MenuPlayer> playerList = allPlayers;

        var orderByResult = from p in playerList
                            orderby p.playerID
                            select p;

        var orderByAscendingResult = from p in playerList
                                      orderby p.playerID ascending
                                      select p;

        allPlayers = orderByAscendingResult.ToArray<MenuPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindObjectsOfType<MenuPlayer>();
        allPlayers = GameObject.FindObjectsOfType<MenuPlayer>(true);
        OrderLoadedByPlayerID();
        OrderAllByPlayerID();

        time.text = System.DateTime.Now.ToString("t");

        GetFreeGamepads();

        if (players.Length < 4)
        {
            if (freeKeyboard)
            {
                joinPrompt.text = "(SPACE / LB + RB)\nto join!";
            }
            else
            {
                joinPrompt.text = "(LB + RB)\nto join!";
            }
        }
        else
        {
            joinPrompt.text = "";
        }
    }

    public void GetFreeGamepads()
    {
        freeGamepads = new bool[] { true, true, true, true };
        freeKeyboard = true;
        foreach (MenuPlayer player in players)
        {
            if (player.gamepadID != 0)
            {
                freeGamepads[player.gamepadID - 1] = false;
            }
            if (player.gamepadID == 0 && player.keyboardControls)
            {
                freeKeyboard = false;
            }
        }
    }
}
