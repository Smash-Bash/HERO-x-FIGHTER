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
    public GameObject readyPrompt;
    public Image startFade;
    public TMP_Text stageName;
    public MenuStageSelect stageSelect;

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
        foreach (CSSPlayer player in cssPlayers)
        {
            if (player.selectedPuck != null)
            {
                ready = false;
            }
        }

        stageName.text = stageSelect.pucks[0].stageName.text;
        readyPrompt.gameObject.SetActive(ready);
        startFade.color = new Color(startFade.color.r, startFade.color.g, startFade.color.b, Mathf.Lerp(startFade.color.a, (ready ? 0.25f : 0f), Time.deltaTime * 10));

        foreach (CSSPlayer player in cssPlayers)
        {
            if (ready && player.input.GetStartDown())
            {
                StartMatch();
            }
        }
    }

    public void StartMatch()
    {
        GameSystems.fighters.Clear();
        GameSystems.inputIDs.Clear();
        foreach (CSSPlayer player in cssPlayers)
        {
            GameSystems.fighters.Add(player.puck.currentCharacterButton.fighter);
            if (player.input is KeyboardInput)
            {
                GameSystems.inputIDs.Add(0);
            }
            else if (player.input is GamepadInput)
            {
                GameSystems.inputIDs.Add(player.input.GetComponent<GamepadInput>().gamepadID);
            }
        }
        GameSystems.LoadScene(stageSelect.pucks[0].currentStageButton.stage);
    }
}
