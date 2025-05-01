using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicClip", menuName = "Music Clip", order = 1)]
public class MusicClip : ScriptableObject
{
    public string source;
    public string trackName;
    public AudioClip start;
    public AudioClip loop;
    public float bpm = 1;
}
