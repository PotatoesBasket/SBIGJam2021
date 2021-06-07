using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current = null;

    public GameObject resultsPanel = null;

    // need reset on new level
    public Animator controllerIcon = null;

    public GameObject player = null;
    public ThirdPersonController playerController = null;
    public GameObject fireEffect = null;

    public Animator playerAnimController = null;  // 0
    public Animator guideAnimController = null;   // 1

    public List<Animator> animatorControllerList = new List<Animator>();
    public List<string> animatorPropertyList = new List<string>();

    // some data
    public int boyCounter = 0;
    public bool initiatedPlatformClimb = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (current == null)
            current = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponentInChildren<ThirdPersonController>();
        playerAnimController = player.GetComponentInChildren<Animator>();
        guideAnimController = GameObject.FindGameObjectWithTag("Guide").GetComponentInChildren<Animator>();

        LoadAnimationLists();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void SlideInHUD()
    {
        controllerIcon.SetTrigger("trigger");
    }

    public void LoadAnimationLists()
    {
        // anim attribute order: controller ID, property ID, state ID (0 off, 1 on)
        // (eg: set guide's isWalking true = 121)

        animatorControllerList.Add(playerAnimController);   // 0
        animatorControllerList.Add(guideAnimController);    // 1

        animatorPropertyList.Add("isTalking");  // 0
        animatorPropertyList.Add("isExcited");  // 1
        animatorPropertyList.Add("isWalking");  // 2
    }
}