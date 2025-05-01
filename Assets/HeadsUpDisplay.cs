using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeadsUpDisplay : MonoBehaviour
{
    public PlayerScript player;
    public TMP_Text damageText;
    public Gradient damageColor;
    public Image fighterIcon;
    public Image healthBar;
    public Image hitstunBar;
    public Image[] stockIcons;
    public TMP_Text stockDisplay;
    public Image[] coloredImages;
    public TMP_Text comboDisplay;
    public TMP_Text traditionalName;
    public TMP_Text platformName;

    [Header("Kusaka")]
    public GameObject energyMeterParent;
    public Image energyMeter;
    public Image[] energyCharges;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (damageText != null)
        {
            damageText.text = (player.damage).ToString("0.0") + "%";
            damageText.text = damageText.text.Insert(damageText.text.IndexOf("."), "<size=50%>");
            damageText.color = damageColor.Evaluate(player.damage / 999);
        }
        if (healthBar != null)
        {
            healthBar.fillAmount = (player.health - player.damage) / player.health;
        }
        if (hitstunBar != null)
        {
            hitstunBar.fillAmount = (player.hitstun);
        }
        if (comboDisplay != null)
        {
            if (player.combo > 1)
            {
                comboDisplay.text = player.combo.ToString();
            }
            else
            {
                comboDisplay.text = "";
            }
        }

        if (player.fighter is Kusaka)
        {
            energyMeterParent.SetActive(true);

            energyMeter.fillAmount = player.fighter.GetComponent<Kusaka>().energy;

            int index = 0;

            foreach (Image charge in energyCharges)
            {
                if (energyMeter.fillAmount >= (index + 1) / 4f)
                {
                    charge.color = Color.Lerp(new Color(1, 1, 0), new Color(1, 1, 1), Mathf.Sin((Time.time * 15) + (index / 4f * 45)));
                }
                else
                {
                    charge.color = Color.clear;
                }
                index++;
            }
        }
        else if (player.fighter is FutureRuby)
        {
            energyMeterParent.SetActive(true);

            energyMeter.fillAmount = player.fighter.GetComponent<FutureRuby>().energy;

            int index = 0;

            foreach (Image charge in energyCharges)
            {
                if (energyMeter.fillAmount >= (index + 1) / 4f)
                {
                    charge.color = Color.Lerp(new Color(1, 1, 0), new Color(1, 1, 1), Mathf.Sin((Time.time * 15) + (index / 4f * 45)));
                }
                else
                {
                    charge.color = Color.clear;
                }
                index++;
            }
        }
        else
        {
            energyMeterParent.SetActive(false);
        }

        foreach (Image currentImage in coloredImages)
        {
            currentImage.color = new Color(player.multiplayer.colors[Mathf.Clamp(player.playerID - 1, 0, 3)].r, player.multiplayer.colors[Mathf.Clamp(player.playerID - 1, 0, 3)].g, player.multiplayer.colors[Mathf.Clamp(player.playerID - 1, 0, 3)].b, currentImage.color.a);
        }

        if (stockDisplay != null)
        {
            if (player.stocks > stockIcons.Length)
            {
                for (int i = 0; i < stockIcons.Length; i++)
                {
                    stockIcons[i].sprite = player.fighter.stockIcon;
                    stockIcons[i].enabled = false;
                }
                if (stockIcons.Length >= 2)
                {
                    stockIcons[1].enabled = true;
                }
                stockDisplay.text = "x" + player.stocks;
            }
            else
            {
                for (int i = 0; i < stockIcons.Length; i++)
                {
                    stockIcons[i].sprite = player.fighter.stockIcon;
                    stockIcons[i].enabled = (player.stocks > i);
                }
                stockDisplay.text = "";
            }
        }

        if (fighterIcon != null)
        {
            fighterIcon.sprite = player.fighter.displayIcon;
        }
        if (traditionalName != null)
        {
            traditionalName.text = player.fighter.fullName;
        }
        if (platformName != null)
        {
            platformName.text = player.fighter.characterName.ToUpper();
        }
    }
}
