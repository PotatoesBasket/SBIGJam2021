using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOneOffDialog : MonoBehaviour, ITriggerableEvent
{
    [SerializeField] uint dialogID = 0;

    [SerializeField] GameObject thingToMoveToPlayer = null; // for moving a trigger to the player for activation immediately (?)
    [SerializeField] GameObject[] thingsToActivate = null;

    public void DoThing()
    {
        DialogManager.current.LoadNextDialogBlock(dialogID);

        if (thingToMoveToPlayer != null)
            thingToMoveToPlayer.transform.position = GameManager.current.player.transform.position;

        foreach (GameObject o in thingsToActivate)
            o.SetActive(true);

        Destroy(gameObject);
    }
}