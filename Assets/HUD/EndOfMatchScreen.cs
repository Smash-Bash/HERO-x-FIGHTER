using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndOfMatchScreen : MonoBehaviour
{
    public MultiplayerManager multiplayer;
    public DialogueBox dialogue;

    public Image rematchBar;
    public Image mainMenuBar;

    public float rematchProgress;
    public float mainMenuProgress;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool rematchMoved = false;
        bool mainMenuMoved = false;
        rematchProgress = Mathf.Clamp(rematchProgress, 0, 1);
        mainMenuProgress = Mathf.Clamp(mainMenuProgress, 0, 1);
        foreach (PlayerScript player in multiplayer.players)
        {
            player.input.enabled = true;
            if (player.input.GetAttack())
            {
                rematchMoved = true;
                if (rematchProgress <= 0)
                {
                    rematchProgress = 0.01f;
                }
                rematchProgress = Mathf.MoveTowards(rematchProgress, 1, Time.deltaTime);
            }
            if (player.input.GetSpecial())
            {
                mainMenuMoved = true;
                if (mainMenuProgress <= 0)
                {
                    mainMenuProgress = 0.01f;
                }
                mainMenuProgress = Mathf.MoveTowards(mainMenuProgress, 1, Time.deltaTime / 2);
            }
            player.input.enabled = false;
        }
        if (!rematchMoved)
        {
            rematchProgress = Mathf.MoveTowards(rematchProgress, 0, Time.deltaTime);
        }
        if (!mainMenuMoved)
        {
            mainMenuProgress = Mathf.MoveTowards(mainMenuProgress, 0, Time.deltaTime);
        }
        if (rematchProgress >= 1)
        {
            Rematch();
        }
        else if (mainMenuProgress >= 1)
        {
            MainMenu();
        }
        rematchBar.fillAmount = rematchProgress;
        mainMenuBar.fillAmount = mainMenuProgress;
    }

    private void OnEnable()
    {
        
    }

    public void Rematch()
    {
        GameSystems.LoadScene(GameSystems.lastLoadedStage);
    }

    public void MainMenu()
    {
        GameSystems.LoadScene(null);
    }
}
