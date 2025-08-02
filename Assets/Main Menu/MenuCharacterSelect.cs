using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCharacterSelect : MenuScreen
{
    public CharacterButton[] characters;
    public CSSPuck[] pucks;
    public CSSPlayer[] cssPlayers;
    public CSSPlayer globalPlayer;
    public Color[] colors;
    public GameObject readyPrompt;
    public Image startFade;
    public TMP_Text stageName;
    public MenuStageSelect stageSelect;
    public CharacterButton randomButton;

    void OnEnable()
    {
        foreach (CSSPlayer player in cssPlayers)
        {
            if (player.computerPlayer)
            {
                player.selectedPuck = null;
            }
        }
    }

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
        foreach (CSSPuck puck in pucks)
        {
            if (puck.isActiveAndEnabled)
            {
                if (puck.currentCharacter == null)
                {
                    ready = false;
                }
            }
        }
        bool playerAvailable = false;
        foreach (CSSPlayer player in cssPlayers)
        {
            if (player.isActiveAndEnabled)
            {
                playerAvailable = true;
            }
            if (player.isActiveAndEnabled && player.selectedPuck != null)
            {
                ready = false;
            }
        }
        if (globalPlayer.isActiveAndEnabled && globalPlayer.selectedPuck != null)
        {
            ready = false;
        }

        globalPlayer.gameObject.SetActive(!playerAvailable);

        stageName.text = stageSelect.pucks[0].stageName.text;
        readyPrompt.gameObject.SetActive(ready);
        startFade.color = new Color(startFade.color.r, startFade.color.g, startFade.color.b, Mathf.Lerp(startFade.color.a, (ready ? 0.25f : 0f), Time.deltaTime * 10));

        bool computerOnly = true;
        foreach (CSSPlayer player in cssPlayers)
        {
            if (player.input != null)
            {
                computerOnly = false;
            }

            if (player.input ? player.input.GetJumpDown() : false)
            {
                foreach (CSSPlayer playerTwo in cssPlayers)
                {
                    playerTwo.selectedPuck.transform.position = randomButton.transform.position;
                    playerTwo.selectedPuck = null;
                }
            }

            if (ready && (player.input ? player.input.GetStartDown() : false))
            {
                StartMatch();
            }
        }

        if (ready && computerOnly && mainMenu.input.GetStartDown())
        {
            StartMatch();
        }
    }

    public void StartMatch()
    {
        GameSystems.fighters.Clear();
        GameSystems.inputIDs.Clear();
        foreach (CSSPlayer player in cssPlayers)
        {
            if (player.puck.isActiveAndEnabled)
            {
                GameSystems.fighters.Add(player.puck.currentCharacterButton.fighter);
                if (player.computerPlayer)
                {
                    GameSystems.inputIDs.Add(-1);
                }
                else if (player.input == null)
                {
                    GameSystems.inputIDs.Add(-1);
                }
                else if (player.input is KeyboardInput)
                {
                    GameSystems.inputIDs.Add(0);
                }
                else if (player.input is GamepadInput)
                {
                    GameSystems.inputIDs.Add(player.input.GetComponent<GamepadInput>().gamepadID);
                }
            }
        }
        GameSystems.LoadScene(stageSelect.pucks[0].currentStageButton.stage);
    }
}
