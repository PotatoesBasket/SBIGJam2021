using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboZoomies : MonoBehaviour, ITriggerableEvent
{
    [SerializeField] WaypointController floatyboi = null;

    public void DoThing()
    {
        GetComponent<BoxCollider>().enabled = false; // disable ability to retrigger

        StartCoroutine(Go()); // start waypoint controller after dialog close
    }

    IEnumerator Go()
    {
        yield return new WaitUntil(DialogManager.current.IsClosed);

        floatyboi.Resume();
    }
}