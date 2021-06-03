using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current = null;
    public bool GamePaused { get; private set; } = false;
    
    public GameObject Player { get; private set; } = null;
    public ThirdPersonController PlayerController { get; private set; } = null;

    public List<Animator> animatorControllerList = new List<Animator>();
    public List<string> animatorPropertyList = new List<string>();

    public Animator PlayerAnimController { get; private set; } = null;
    public Animator GuideAnimController { get; private set; } = null;

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
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerController = Player.GetComponentInChildren<ThirdPersonController>();
        PlayerAnimController = Player.GetComponentInChildren<Animator>();
        GuideAnimController = GameObject.FindGameObjectWithTag("Guide").GetComponentInChildren<Animator>();

        LoadAnimationLists();
    }

    void LoadAnimationLists()
    {
        // anim attribute order: controller ID, property ID, state ID (0 off, 1 on)
        // (eg: set guide's isWalking true = 121)

        animatorControllerList.Add(PlayerAnimController);   //0
        animatorControllerList.Add(GuideAnimController);    //1

        animatorPropertyList.Add("isTalking");  //0
        animatorPropertyList.Add("isExcited");  //1
        animatorPropertyList.Add("isWalking");  //2
    }
}