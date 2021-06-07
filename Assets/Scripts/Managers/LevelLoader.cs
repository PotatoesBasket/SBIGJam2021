using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hhhhhhhhhhhhhhhhhhhhhh

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject player = null;
    [SerializeField] ThirdPersonController playerController = null;
    [SerializeField] GameObject fireEffect = null;

    [SerializeField] Animator playerAnimController = null;
    [SerializeField] Animator guideAnimController = null;

    private void Awake()
    {
        GameManager.current.player = player;
        GameManager.current.playerController = playerController;
        GameManager.current.fireEffect = fireEffect;

        GameManager.current.playerAnimController = playerAnimController;
        GameManager.current.guideAnimController = guideAnimController;

        GameManager.current.animatorControllerList = new List<Animator>();
        GameManager.current.animatorPropertyList = new List<string>();
        GameManager.current.LoadAnimationLists();

        DialogManager.current.currentAnimator = null;
        DialogManager.current.currentCam = null;

        DialogManager.current.allowProgression = true;
        DialogManager.current.EndDialogBlock();

        AudioManager.current.BGMDefaultSource.clip = AudioManager.current.fire;
        AudioManager.current.BGMDefaultSource.loop = true;
        AudioManager.current.BGMDefaultSource.Play();

        DialogManager.current.LoadNextDialogBlock(20);
        StartCoroutine(eeeee());
    }

    IEnumerator eeeee()
    {
        yield return new WaitUntil(DialogManager.current.IsClosed);
        yield return new WaitForSeconds(2);

        AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.win);

        GameManager.current.resultsPanel.SetActive(true);
    }
}