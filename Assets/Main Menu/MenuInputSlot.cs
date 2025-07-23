using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuInputSlot : MonoBehaviour
{
    public CSSPlayer player;
    public Image inputTypeDisplay;
    public TMP_Text prompt;
    public TMP_Text cpu;
    public TMP_Text upperDisplay;
    public TMP_Text lowerDisplay;
    public Image cpuBackground;
    public Image noneBackground;
    public Sprite keyboard;
    public Sprite xboxController;
    public Sprite playstationController;
    public Sprite switchController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.computerPlayer)
        {
            prompt.enabled = false;
            inputTypeDisplay.enabled = false;
            cpu.enabled = true;
            upperDisplay.text = "COMPUTER";
            lowerDisplay.text = "Q / LB: Remove";
        }
        else if (player.input == null)
        {
            prompt.enabled = true;
            inputTypeDisplay.enabled = false;
            cpu.enabled = false;
            upperDisplay.text = "NONE";
            lowerDisplay.text = "M1 / A: Join";
        }
        else if (player.input is GamepadInput)
        {
            prompt.enabled = false;
            inputTypeDisplay.sprite = xboxController;
            inputTypeDisplay.enabled = true;
            cpu.enabled = false;
            upperDisplay.text = "GAMEPAD " + player.input.GetComponent<GamepadInput>().gamepadID;
            lowerDisplay.text = "B: Drop Out";
        }
        else if (player.input is KeyboardInput)
        {
            prompt.enabled = false;
            inputTypeDisplay.sprite = keyboard;
            inputTypeDisplay.enabled = true;
            cpu.enabled = false;
            upperDisplay.text = "KEYBOARD + MOUSE";
            lowerDisplay.text = "M2: Drop Out";
        }
        cpuBackground.enabled = cpu.enabled;
        noneBackground.enabled = player.input == null && !player.computerPlayer;
    }
}
