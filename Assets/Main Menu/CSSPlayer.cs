using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSPlayer : MonoBehaviour
{
    public MenuCharacterSelect characterSelect;
    public PlayerInput input;
    public int playerID;
    public bool computerPlayer;
    public GameObject defaultCursor;
    public GameObject highlightedCursor;
    public GameObject grabbingCursor;
    public CSSPuck puck;
    public CSSPuck selectedPuck;
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


        if (selectedPuck == null && input.GetSpecialDown())
        {
            selectedPuck = puck;
        }


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
            CSSPuck closestPuck = FindClosestPuck();

            if (closestPuck != null)
            {
                defaultCursor.SetActive(false);
                highlightedCursor.SetActive(true);
                grabbingCursor.SetActive(false);

                if (input.GetAttackDown() && (closestPuck == puck || closestPuck.player.computerPlayer))
                {
                    selectedPuck = closestPuck;
                }
            }
            else
            {
                defaultCursor.SetActive(true);
                highlightedCursor.SetActive(false);
                grabbingCursor.SetActive(false);
            }
        }
    }

    public CSSPuck FindClosestPuck()
    {
        CSSPuck closest = null;
        float distance = 2500;
        Vector3 position = transform.position;
        foreach (CSSPuck puck in characterSelect.pucks)
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
