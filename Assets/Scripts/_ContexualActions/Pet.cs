using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour, IContextuallyActionable
{
    [SerializeField] WaypointController waypointController = null;
    [SerializeField] GameObject nextEventTrigger = null;
    [SerializeField] GameObject[] unlockActions = null;
    public bool triggersEvent = true;

    bool hasBeenPet = false;

    public void DoThing()
    {
        if (hasBeenPet == false)
        {
            hasBeenPet = true;
            ++GameManager.current.boyCounter;
        }

        GameManager.current.playerAnimController.SetTrigger("Pet");
        GameManager.current.playerController.SetPhysicsPauseState(true);
        GameManager.current.playerController.ResetVelocity();
        
        if (waypointController != null)
            waypointController.isPaused = true;

        StartCoroutine(Sequence());
    }

    public void DoThingLonger()
    {
    }

    IEnumerator Sequence()
    {
        GameManager.current.playerController.SetPlayerInputPauseState(true);
        GameManager.current.playerController.ClearInput();

        yield return new WaitForSeconds(2.83f);

        AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.meow);
        GameManager.current.playerController.SetPlayerInputPauseState(false);
        GameManager.current.playerController.SetPhysicsPauseState(false);

        if (waypointController != null)
            waypointController.isPaused = false;

        if (triggersEvent)
        {
            triggersEvent = false;
            DialogManager.current.LoadNextDialogBlock(3);

            nextEventTrigger.transform.position = GameManager.current.player.transform.position;

            foreach (GameObject o in unlockActions)
                o.SetActive(true);
        }
    }

    [SerializeField] int priority = 100;
    public int GetPriority()
    {
        return priority;
    }
}