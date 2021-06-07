using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformProgressStarted : MonoBehaviour, ITriggerableEvent
{
    public void DoThing()
    {
        GameManager.current.initiatedPlatformClimb = true;
    }
}