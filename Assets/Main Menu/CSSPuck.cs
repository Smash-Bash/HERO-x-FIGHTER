using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CSSPuck : MonoBehaviour
{
    public MenuCharacterSelect characterSelect;
    public CharacterButton currentCharacterButton;
    public GameObject currentCharacter;
    public Transform characterBase;
    public TMP_Text fighterName;
    public CSSPlayer player;
    public Image puckImage;
    public TMP_Text puckText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CharacterButton oldButton = currentCharacterButton;
        currentCharacterButton = FindClosestCharacter();

        if (currentCharacter != null)
        {
            if (currentCharacter.name == "Random(Clone)")
            {
                currentCharacter.transform.position = Vector3.Lerp(currentCharacter.transform.position, characterBase.position + (Vector3.up * 0.5f) + (Vector3.up * 0.1f * Mathf.Sin(Time.time * 2)), Time.deltaTime * 15);
                if (currentCharacter.transform.parent.localPosition.x < 0)
                {
                    currentCharacter.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else
            {
                currentCharacter.transform.position = Vector3.Lerp(currentCharacter.transform.position, characterBase.position, Time.deltaTime * 15);
            }
        }

        if (oldButton != currentCharacterButton)
        {
            RefreshCharacter();
        }
    }

    public void RefreshCharacter()
    {
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        if (currentCharacterButton ? currentCharacterButton.fighter != null : false)
        {
            fighterName.text = currentCharacterButton.fighter.fullName;

            currentCharacter = GameObject.Instantiate(currentCharacterButton.fighter.gameObject, characterBase);

            if (characterBase.gameObject.name.Contains("1"))
            {
                currentCharacter.transform.position += new Vector3(-1, 0.25f, 0);
            }
            else if (characterBase.gameObject.name.Contains("2"))
            {
                currentCharacter.transform.position += new Vector3(1, 0.25f, 0);
            }

            Destroy(currentCharacter.GetComponent<PlayerScript>().model);
            Destroy(currentCharacter.GetComponent<PlayerScript>());
            Destroy(currentCharacter.GetComponent<Fighter>());
        }
        else
        {
            fighterName.text = "Coming Soon...";
        }
        
    }

    public CharacterButton FindClosestCharacter()
    {
        CharacterButton closest = null;
        float distance = 5000;
        Vector3 position = transform.position;
        foreach (CharacterButton character in characterSelect.characters)
        {
            if (Vector3.Distance(position, character.transform.position) <= 5000 && character.isActiveAndEnabled)
            {
                Vector3 diff = character.transform.position - position;
                //float curDistance = diff.sqrMagnitude;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = character;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}
