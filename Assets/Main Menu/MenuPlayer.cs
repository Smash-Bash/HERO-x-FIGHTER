using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPlayer : MonoBehaviour
{
    // NOTE: To get the selection system working the way I wanted, I had to remove the joystick bindings from 'Submit' and 'Cancel'.

    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public Image image;
    [Range(1, 4)]
    public int playerID;
    [Range(0, 4)]
    public int gamepadID;
    public bool keyboardControls;
    public bool fireHeld;
    public bool altFireHeld;

    public MenuScreen screen;

    public MenuButton selectedButton;

    public MenuTransition transition;

    public float moveDelay;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        transition = FindFirstObjectByType<MenuTransition>();
        if (selectedButton == null)
        {
            selectedButton = FindFirstObjectByType<MenuScreen>().defaultButton;
            if (selectedButton != null)
            {
                rectTransform.position = selectedButton.transform.position;
                rectTransform.sizeDelta = selectedButton.GetComponent<RectTransform>().sizeDelta;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (screen == null || !screen.isActiveAndEnabled)
        {
            screen = FindFirstObjectByType<MenuScreen>();
        }
        fireHeld = ((keyboardControls && Input.GetMouseButton(0)) || Input.GetAxis(GetPlayerID() + "Fire") >= 0.5f);
        altFireHeld = ((keyboardControls && Input.GetMouseButton(1)) || Input.GetAxis(GetPlayerID() + "Alt-Fire") >= 0.5f);

        image.enabled = transition.from == null && transition.to == null;

        if (selectedButton == null)
        {
            selectedButton = FindFirstObjectByType<MenuScreen>().defaultButton;
            if (selectedButton != null)
            {
                rectTransform.position = selectedButton.transform.position;
                rectTransform.sizeDelta = selectedButton.GetComponent<RectTransform>().sizeDelta;
            }
        }
        else if (!selectedButton.isActiveAndEnabled && (selectedButton.northButton != null || selectedButton.southButton != null || selectedButton.westButton != null || selectedButton.eastButton != null))
        {
            if (selectedButton.southButton != null)
            {
                selectedButton = selectedButton.southButton;
            }
            else if (selectedButton.northButton != null)
            {
                selectedButton = selectedButton.northButton;
            }
            else if (selectedButton.westButton != null)
            {
                selectedButton = selectedButton.westButton;
            }
            else if (selectedButton.eastButton != null)
            {
                selectedButton = selectedButton.eastButton;
            }
            if (screen.description != null)
            {
                screen.description.text = selectedButton.description;
            }
        }
        else
        {
            moveDelay -= Time.deltaTime;

            if (moveDelay <= 0 && (selectedButton.eastButton ? selectedButton.eastButton.isActiveAndEnabled : false) && ((Input.GetAxisRaw(GetPlayerID() + "Horizontal") + Input.GetAxisRaw(GetPlayerID() + "D-Pad Horizontal")) > 0.5f || (keyboardControls && Input.GetKey("d"))))
            {
                moveDelay = 0.25f;
                selectedButton = selectedButton.eastButton;
                if (screen.description != null)
                {
                    screen.description.text = selectedButton.description;
                }
            }
            else if (moveDelay <= 0 && (selectedButton.westButton ? selectedButton.westButton.isActiveAndEnabled : false) && ((Input.GetAxisRaw(GetPlayerID() + "Horizontal") + Input.GetAxisRaw(GetPlayerID() + "D-Pad Horizontal")) < -0.5f || (keyboardControls && Input.GetKey("a"))))
            {
                moveDelay = 0.25f;
                selectedButton = selectedButton.westButton;
                if (screen.description != null)
                {
                    screen.description.text = selectedButton.description;
                }
            }
            else if (moveDelay <= 0 && (selectedButton.northButton ? selectedButton.northButton.isActiveAndEnabled : false) && ((Input.GetAxisRaw(GetPlayerID() + "Vertical") + Input.GetAxisRaw(GetPlayerID() + "D-Pad Vertical")) > 0.5f || (keyboardControls && Input.GetKey("w"))))
            {
                moveDelay = 0.25f;
                selectedButton = selectedButton.northButton;
                if (screen.description != null)
                {
                    screen.description.text = selectedButton.description;
                }
            }
            else if (moveDelay <= 0 && (selectedButton.southButton ? selectedButton.southButton.isActiveAndEnabled : false) && ((Input.GetAxisRaw(GetPlayerID() + "Vertical") + Input.GetAxisRaw(GetPlayerID() + "D-Pad Vertical")) < -0.5f || (keyboardControls && Input.GetKey("s"))))
            {
                moveDelay = 0.25f;
                selectedButton = selectedButton.southButton;
                if (screen.description != null)
                {
                    screen.description.text = selectedButton.description;
                }
            }
            else if ((!keyboardControls && Mathf.Abs((Input.GetAxisRaw(GetPlayerID() + "Horizontal") + Input.GetAxisRaw(GetPlayerID() + "D-Pad Horizontal"))) < 0.5f && Mathf.Abs((Input.GetAxisRaw(GetPlayerID() + "Vertical") + Input.GetAxisRaw(GetPlayerID() + "D-Pad Vertical"))) < 0.5f) || (keyboardControls && !Input.GetKey("w") && !Input.GetKey("s") && !Input.GetKey("a") && !Input.GetKey("d")))
            {
                moveDelay = 0f;
            }

            rectTransform.position = selectedButton.transform.position;
            rectTransform.sizeDelta = selectedButton.GetComponent<RectTransform>().sizeDelta;

            selectedButton.ButtonUpdate(this);

            if (selectedButton.button != null && (Input.GetButtonDown(GetPlayerID() + "Jump")) && selectedButton.allowButtonOrToggleActivation)
            {
                selectedButton.lastInteractor = this;
                ExecuteEvents.Execute(selectedButton.button.gameObject, new BaseEventData(FindObjectOfType<EventSystem>()), ExecuteEvents.submitHandler);
            }

            if (selectedButton.toggle != null && (Input.GetButtonDown(GetPlayerID() + "Jump") || (Input.GetButton(GetPlayerID() + "Jump") && moveDelay == 0.25f)) && selectedButton.allowButtonOrToggleActivation)
            {
                selectedButton.lastInteractor = this;
                ExecuteEvents.Execute(selectedButton.toggle.gameObject, new BaseEventData(FindObjectOfType<EventSystem>()), ExecuteEvents.submitHandler);
            }
        }

        image.enabled = selectedButton != null;
    }

    public string GetPlayerID()
    {
        if (keyboardControls)
        {
            return "";
        }
        else
        {
            return gamepadID + " ";
        }
    }
}
