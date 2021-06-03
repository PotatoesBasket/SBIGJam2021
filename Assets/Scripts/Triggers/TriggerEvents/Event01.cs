using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event01 : MonoBehaviour, ITriggerableEvent
{
    public void DoThing()
    {
        DialogManager.current.LoadNextDialogBlock(1);
        Destroy(gameObject);
    }
}