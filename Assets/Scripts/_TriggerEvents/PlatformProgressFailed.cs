using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformProgressFailed : MonoBehaviour, ITriggerableEvent
{
    public void DoThing()
    {
        if (GameManager.current.initiatedPlatformClimb)
        {
            GameManager.current.initiatedPlatformClimb = false;
            AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.fart);
        }
    }
}