using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SSSPuck : MonoBehaviour
{
    public MenuStageSelect stageSelect;
    public StageButton currentStageButton;
    public TMP_Text stageName;
    public Image stagePreview;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StageButton oldButton = currentStageButton;
        currentStageButton = FindClosestCharacter();

        if (oldButton != currentStageButton)
        {
            RefreshCharacter();
        }
    }

    public void RefreshCharacter()
    {
        if (currentStageButton.stage != null)
        {
            stageName.text = currentStageButton.stage.stageName.ToUpper() + "\n<size=150%>" + currentStageButton.stage.stageVariant.ToUpper();
            stagePreview.sprite = currentStageButton.stageImage.sprite;
        }
        else
        {
            stageName.text = "Coming Soon...";
        }
        
    }

    public StageButton FindClosestCharacter()
    {
        StageButton closest = null;
        float distance = 15000;
        Vector3 position = transform.position;
        foreach (StageButton stage in stageSelect.stages)
        {
            if (Vector3.Distance(position, stage.transform.position) <= 15000 && stage.isActiveAndEnabled)
            {
                Vector3 diff = stage.transform.position - position;
                //float curDistance = diff.sqrMagnitude;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = stage;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}
