using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour, IContextuallyActionable
{
    [SerializeField] float jumpVelocity = 0;

    public void DoThing()
    {
        Debug.Log("Jump");

        ThirdPersonController cont = GameManager.current.PlayerController;

        if (cont.IsGrounded())
        {
            cont.isAirborne = true;
            cont.velocity.y = jumpVelocity;

            Animator anim = GameManager.current.PlayerAnimController;
            anim.SetTrigger("Jump");
        }
    }

    public void DoThingLonger() { }
}