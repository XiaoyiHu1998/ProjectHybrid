using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LivelyEvent : MonoBehaviour
{
    public UnityEvent livelyEvent;

    public void Invoke()
    {
        livelyEvent.Invoke();
    }
}
