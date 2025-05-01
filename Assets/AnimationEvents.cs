using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    public UnityEvent[] events;

    public void Invoke(int id)
    {
        events[id].Invoke();
    }
}
