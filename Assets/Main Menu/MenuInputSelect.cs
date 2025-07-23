using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuInputSelect : MenuScreen
{
    public MenuInputSlot[] inputSlots;
    public List<int> takenGamepads;
    public List<int> untakenGamepads;
    public bool keyboardTaken;
    private bool computerEdit;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        computerEdit = false;

        UpdateSlots();

        MenuInputSlot finalComputerSlot = null;

        foreach (MenuInputSlot slot in inputSlots)
        {
            if (slot.player.computerPlayer)
            {
                finalComputerSlot = slot;
            }
            else if (slot.player.input != null)
            {
                if (slot.player.input is KeyboardInput ? Input.GetMouseButtonDown(1) : false)
                {
                    Destroy(slot.player.input);
                }
                else if (slot.player.input is GamepadInput ? Input.GetKeyDown("joystick " + slot.player.GetComponent<GamepadInput>().gamepadID + " button " + 1) : false)
                {
                    Destroy(slot.player.input);
                }
            }
            else if (slot.player.input == null)
            {
                AssignInput(slot.player);
            }
        }

        if (finalComputerSlot != null)
        {
            foreach (int gamepad in new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 })
            {
                if (!computerEdit && Input.GetKeyDown("joystick " + gamepad + " button " + 4))
                {
                    computerEdit = true;
                    finalComputerSlot.player.computerPlayer = false;
                    UpdateSlots();
                }
            }
            if (!computerEdit && Input.GetKeyDown("q"))
            {
                computerEdit = true;
                finalComputerSlot.player.computerPlayer = false;
                UpdateSlots();
            }
        }

        foreach (MenuInputSlot slot in inputSlots)
        {
            if (slot.player.input != null)
            {
                if (slot.player.input is GamepadInput ? Input.GetKeyDown("joystick " + slot.player.input.GetComponent<GamepadInput>().gamepadID + " button " + slot.player.input.GetComponent<GamepadInput>().startButton) : false)
                {
                    mainMenu.ChangeMenu("Stage Select Screen");
                }
                else if (slot.player.input is KeyboardInput ? Input.GetKeyDown(KeyCode.Return) : false)
                {
                    mainMenu.ChangeMenu("Stage Select Screen");
                }
            }
        }
    }

    public void AssignInput(CSSPlayer player)
    {
        foreach (int gamepad in untakenGamepads)
        {
            if (player.input == null && Input.GetKeyDown("joystick " + gamepad + " button " + 0))
            {
                player.input = player.AddComponent<GamepadInput>();
                player.input.GetComponent<GamepadInput>().gamepadID = gamepad;
                UpdateSlots();
            }
        }
        if (!keyboardTaken)
        {
            if (player.input == null && Input.GetMouseButtonDown(0))
            {
                player.input = player.AddComponent<KeyboardInput>();
                player.input.GetComponent<KeyboardInput>();
                UpdateSlots();
            }
        }
        foreach (int gamepad in new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 })
        {
            if (player.input == null && !computerEdit)
            {
                if (Input.GetKeyDown("joystick " + gamepad + " button " + 5))
                {
                    computerEdit = true;
                    player.computerPlayer = true;
                    UpdateSlots();
                }
            }
        }
        if (player.input == null && !computerEdit)
        {
            if (Input.GetKeyDown("e"))
            {
                computerEdit = true;
                player.computerPlayer = true;
                UpdateSlots();
            }
        }
    }

    public void UpdateSlots()
    {
        takenGamepads.Clear();
        untakenGamepads = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
        keyboardTaken = false;

        foreach (MenuInputSlot slot in inputSlots)
        {
            if (slot.player.computerPlayer)
            {
                slot.player.gameObject.SetActive(false);
                slot.player.puck.gameObject.SetActive(true);
            }
            else if (slot.player.input == null)
            {
                slot.player.gameObject.SetActive(false);
                slot.player.puck.gameObject.SetActive(false);
            }
            else if (slot.player.input is GamepadInput)
            {
                takenGamepads.Add(slot.player.input.GetComponent<GamepadInput>().gamepadID);
                untakenGamepads.Remove(slot.player.input.GetComponent<GamepadInput>().gamepadID);
                slot.player.gameObject.SetActive(true);
                slot.player.puck.gameObject.SetActive(true);
            }
            else if (slot.player.input is KeyboardInput)
            {
                keyboardTaken = true;
                slot.player.gameObject.SetActive(true);
                slot.player.puck.gameObject.SetActive(true);
            }
        }
    }
}
