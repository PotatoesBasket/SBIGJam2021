using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdiotAlarm : MonoBehaviour, IContextuallyActionable
{
    public void DoThing()
    {
        Debug.LogError("YOU DID IT. YOU DID THE DUMB THING AGAIN. YOU DUMB.");
        AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.wrong);
    }

    public void DoThingLonger()
    {
    }

    [SerializeField] int priority = 0;
    public int GetPriority()
    {
        return priority;
    }
}