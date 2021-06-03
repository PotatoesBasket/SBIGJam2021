using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour, IContextuallyActionable
{
    public void DoThing()
    {
        Debug.Log("Climb");

        Animator anim = GameManager.current.PlayerAnimController;
        anim.SetTrigger("Climb");
    }

    public void DoThingLonger() { }
}