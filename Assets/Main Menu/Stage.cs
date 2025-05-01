using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Stage", order = 1)]
public class Stage : ScriptableObject
{
    public string sceneName;
    public string stageName;
    public string stageVariant;
    [TextArea(0,5)]
    public string stageDescription;
    public Sprite thumbnail;
}
