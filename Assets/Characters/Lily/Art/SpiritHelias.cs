using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritHelias : MonoBehaviour
{
    public Transform model;
    public Animator baseAnimator;
    public Animator currentAnimator;
    public Vector3 position;
    public float lerpSpeed = 1;
    public bool sinSize = true;

    // Start is called before the first frame update
    void Start()
    {
        position = model.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (sinSize)
        {
            transform.localScale = Vector3.one + (Vector3.one * ((Mathf.Sin(Time.time * 5) + 1) / 2) * 0.05f);
        }
        foreach (AnimatorControllerParameter parameter in currentAnimator.parameters)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    currentAnimator.SetFloat(parameter.name, baseAnimator.GetFloat(parameter.name));
                    break;
                case AnimatorControllerParameterType.Int:
                    currentAnimator.SetInteger(parameter.name, baseAnimator.GetInteger(parameter.name));
                    break;
                case AnimatorControllerParameterType.Bool:
                    currentAnimator.SetBool(parameter.name, baseAnimator.GetBool(parameter.name));
                    break;
            }
        }
        position = Vector3.Lerp(position, model.position, Time.deltaTime * lerpSpeed);
        transform.position = position;
    }

    void LateUpdate()
    {
        transform.position = position;
    }
}
