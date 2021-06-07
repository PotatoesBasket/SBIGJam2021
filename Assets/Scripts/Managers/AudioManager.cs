using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current = null;

    public AudioSource BGMDefaultSource = null;
    public AudioSource SFXDefaultSource = null;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(gameObject);
    }

    public AudioClip meow;
    public AudioClip roboStartup;
    public AudioClip dialogContinueBoop;
    public AudioClip jump;
    public AudioClip wrong;
    public AudioClip launch;
    public RandomizedSound gibberish;
    public AudioClip fart;
    public AudioClip fire;
    public AudioClip win;
}