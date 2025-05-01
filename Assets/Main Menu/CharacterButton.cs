using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Fighter fighter;
    public Image characterImage;

    // Start is called before the first frame update
    void Start()
    {
        if (fighter == null)
        {
            characterImage.color = Color.gray;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
