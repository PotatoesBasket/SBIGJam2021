using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNoise : MonoBehaviour, ITriggerableEvent
{
    [SerializeField] AudioClip audioClip = null;
    [SerializeField] bool oneOff = false;

    public void DoThing()
    {
        AudioManager.current.SFXDefaultSource.PlayOneShot(audioClip);

        if (oneOff)
            Destroy(this);
    }
}