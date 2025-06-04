using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class MainMenu : MonoBehaviour
{
    public MenuMultiplayerManager multiplayer;
    public MenuTransition transition;
    public MenuScreen currentScreen;
    public string discordServerURL;
    public VideoPlayer videoPlayer;
    public Image videoFade;
    public float demoVideoTimer;
    public GameObject normalDisplay;
    public GameObject demoDisplay;
    public UniversalInput input;

    [Header("Screens")]
    public MenuScreen[] allScreens;
    public GameObject[] allBackgrounds;
    public MenuScreen titleScreen;
    public MenuScreen mainMenu;
    public EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 1;

        if (currentScreen ? !currentScreen.isActiveAndEnabled : true)
        {
            currentScreen = FindFirstObjectByType<MenuScreen>();
        }

        eventSystem.SetSelectedGameObject(null);
    }

    public void ChangeMenu(string menuName)
    {
        MenuScreen screen = null;
        foreach (MenuScreen currentScreen in allScreens)
        {
            if (currentScreen.gameObject.name == menuName)
            {
                screen = currentScreen;
            }
        }
        if (screen != null)
        {
            transition.Transition(FindFirstObjectByType<MenuScreen>(), screen);
        }
    }

    public void Back()
    {
        if (currentScreen != null)
        {
            if (currentScreen.previousMenu != null)
            {
                transition.Transition(currentScreen, currentScreen.previousMenu);
            }
        }
    }
}
