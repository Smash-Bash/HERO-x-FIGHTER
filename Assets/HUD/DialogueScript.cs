using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueScript", menuName = "DialogueScript", order = 1)]
[System.Serializable]
public class DialogueScript : ScriptableObject
{
    public Dialogue[] script;
}

[System.Serializable]
public class Dialogue
{
    [TextArea(1, 3)]
    public string dialogueString;
}
