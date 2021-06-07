using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoronAlert : MonoBehaviour, ITriggerableEvent
{
    public void DoThing()
    {
        Debug.LogError("WEEWOOWEEWOO WE GOT A DUMMY");
        AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.wrong);
    }
}