using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour, IContextuallyActionable
{
    [SerializeField] float launchVelocity = 0;

    bool initiated = false;
    bool isActive = false;
    bool wasContinued = false;

    bool effectsActive = false;

    public void DoThing()
    {
        AudioManager.current.SFXDefaultSource.clip = AudioManager.current.launch;
        AudioManager.current.SFXDefaultSource.loop = true;
        AudioManager.current.SFXDefaultSource.Play();
        GameManager.current.playerAnimController.SetBool("isLaunching", true);
        GameManager.current.fireEffect.SetActive(true);

        initiated = true;
        isActive = true;

        effectsActive = true;
    }

    public void DoThingLonger()
    {
        GameManager.current.playerController.isAirborne = true;
        GameManager.current.playerController.velocity.y += launchVelocity * Time.deltaTime;

        if (effectsActive == false)
        {
            GameManager.current.playerAnimController.SetBool("isLaunching", true);
            GameManager.current.fireEffect.SetActive(true);
            AudioManager.current.SFXDefaultSource.Play();
            effectsActive = true;
        }

        wasContinued = true;
    }

    private void Update()
    {
        if (effectsActive && (GameManager.current.playerController.velocity.y < -15.0f || GameManager.current.playerController.IsGrounded()))
        {
            AudioManager.current.SFXDefaultSource.Stop();
            GameManager.current.playerAnimController.SetBool("isLaunching", false);
            GameManager.current.fireEffect.SetActive(false);
            effectsActive = false;
        }

        if (!initiated)
            return;

        else if (isActive)
        {
            if (wasContinued)
                wasContinued = false;
            else
                isActive = false;
        }

        else
            EndThing();
    }

    void EndThing()
    {
        initiated = false;
    }

    [SerializeField] int priority = 10;
    public int GetPriority()
    {
        return priority;
    }
}