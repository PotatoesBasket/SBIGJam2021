using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour, IContextuallyActionable
{
    [SerializeField] float jumpVelocity = 0;

    public void DoThing()
    {
        Debug.Log("Jump");

        ThirdPersonController cont = GameManager.current.playerController;

        if (cont.IsGrounded())
        {
            cont.isAirborne = true;
            cont.velocity.y = jumpVelocity;
            AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.jump);
        }
    }

    public void DoThingLonger()
    {
    }

    [SerializeField] int priority = 5;
    public int GetPriority()
    {
        return priority;
    }
}