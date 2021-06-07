using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjectActiveState : MonoBehaviour, ITriggerableEvent
{
    [SerializeField] GameObject[] objects = null;
    bool state = true;

    public void DoThing()
    {
        foreach (GameObject o in objects)
            o.SetActive(state);
    }
}