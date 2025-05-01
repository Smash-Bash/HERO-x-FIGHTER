using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSSPlayer : MonoBehaviour
{
    public MenuStageSelect stageSelect;
    public PlayerInput input;
    public GameObject defaultCursor;
    public GameObject highlightedCursor;
    public GameObject grabbingCursor;
    public SSSPuck puck;
    public SSSPuck selectedPuck;
    public Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector2.Lerp(velocity, new Vector3(input.GetLeftStickX(), input.GetLeftStickY()), Time.deltaTime * 5);
        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime * 500;



        if (selectedPuck != null)
        {
            selectedPuck.transform.position = transform.position;

            defaultCursor.SetActive(false);
            highlightedCursor.SetActive(false);
            grabbingCursor.SetActive(true);

            if (input.GetAttackDown())
            {
                selectedPuck = null;
            }
        }
        else
        {
            SSSPuck closestPuck = FindClosestPuck();

            if (closestPuck != null)
            {
                defaultCursor.SetActive(false);
                highlightedCursor.SetActive(true);
                grabbingCursor.SetActive(false);

                if (input.GetAttackDown() && closestPuck == puck)
                {
                    selectedPuck = closestPuck;
                }
            }
            else
            {
                defaultCursor.SetActive(true);
                highlightedCursor.SetActive(false);
                grabbingCursor.SetActive(false);

                if (input.GetSpecialDown())
                {
                    selectedPuck = puck;
                }
            }
        }
    }

    public SSSPuck FindClosestPuck()
    {
        SSSPuck closest = null;
        float distance = 2500;
        Vector3 position = transform.position;
        foreach (SSSPuck puck in stageSelect.pucks)
        {
            if (Vector3.Distance(position, puck.transform.position) <= 2500 && puck.isActiveAndEnabled)
            {
                Vector3 diff = puck.transform.position - position;
                //float curDistance = diff.sqrMagnitude;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = puck;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}
