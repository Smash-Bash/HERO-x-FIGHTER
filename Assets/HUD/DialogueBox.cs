using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public TMP_Text dialogueDisplay;

    public string dialogue;
    public GameObject dialogueBox;
    public float dialogueTime;

    public List<Dialogue> lines;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //dialogueBox.SetActive(dialogueTime > 0);
        if (dialogue.Length > 0)
        {
            dialogueDisplay.text = dialogueDisplay.text + dialogue[0];
            dialogue = dialogue.Substring(1);
        }
        else
        {
            if (dialogueTime < 0)
            {
                if (lines.Count > 0)
                {
                    dialogue = lines[0].dialogueString;
                    lines.RemoveAt(0);
                    dialogueDisplay.text = "";
                    dialogueTime = 2;
                }
                else
                {
                    //dialogue = "";
                    //dialogueDisplay.text = "";
                    dialogueTime = 0;
                }
            }
            else
            {
                dialogueTime -= Time.deltaTime;
            }
        }
    }

    public void SetDialogue(string newDialogue)
    {
        dialogue = newDialogue;
        //gameObject.SetActive(true);
    }
}
