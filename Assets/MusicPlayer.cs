using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [HideInInspector]
    public MusicClip music;
    [HideInInspector]
    public AudioSource speaker;
    [HideInInspector]
    public MusicClip currentMusic;
    public MusicClip chosenMusic;
    [Header("Reactive")]
    public MusicOverride musicOverride;
    [Header("Guitar")]
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        speaker = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (time % 10 <= 0)
        {
            time += (music ? music.bpm : 1);
        }

        if (musicOverride == null)
        {
            musicOverride = GameObject.FindObjectOfType<MusicOverride>();
        }

        if (musicOverride != null)
        {
            if (musicOverride.isActiveAndEnabled)
            {
                music = musicOverride.musicOverride;
            }
            else
            {
                musicOverride = null;
            }
        }
        else if (chosenMusic != null)
        {
            music = chosenMusic;
        }

        if (music != null && music != currentMusic)
        {
            if (music.start != null)
            {
                speaker.Stop();
                speaker.PlayOneShot(music.start);
                speaker.clip = music.loop;
                speaker.PlayDelayed(music.start.length);
                time = music.bpm / 10;
            }
            else
            {
                speaker.Stop();
                speaker.clip = music.loop;
                speaker.Play();
                time = music.bpm / 10;
            }
            currentMusic = music;
        }
        else if (music == null)
        {
            if (currentMusic != null)
            {
                time = 0;
            }
            speaker.Stop();
            currentMusic = null;
        }
    }
}
