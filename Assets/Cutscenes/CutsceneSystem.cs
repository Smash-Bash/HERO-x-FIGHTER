using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSystem : MonoBehaviour
{
    public Animator animator;
    public MultiplayerManager multiplayer;

    public List<GameObject> actorObjects;
    public bool introPlayed;
    public bool playDialogue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!introPlayed)
        {
            PlayCutscene(multiplayer.allPlayers[0].model);
            introPlayed = true;
        }
    }

    public void PlayCutscene(PlayerModel model)
    {
        ClearActors();
        actorObjects.Add(Instantiate(model.gameObject, transform));
        actorObjects[0].gameObject.name = "Ruby (Ninja)";
        actorObjects[0].gameObject.SetActive(true);
        Destroy(actorObjects[0].GetComponent<PlayerModel>());
        actorObjects[0].GetComponent<Animator>().writeDefaultValuesOnDisable = true;
        actorObjects[0].GetComponent<Animator>().StopPlayback();
        actorObjects[0].GetComponent<Animator>().enabled = false;
        Destroy(actorObjects[0].GetComponent<Animator>());
        animator.Rebind();
    }

    public void ClearActors()
    {
        foreach (GameObject currentObject in actorObjects)
        {
            Destroy(currentObject.gameObject);
        }
        actorObjects.Clear();
    }
}
