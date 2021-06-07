using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LoadLevel : MonoBehaviour, ITriggerableEvent
{
    [SerializeField] int levelIdx = 0;

    public void DoThing()
    {
        StartCoroutine(WaitForDialogBox());
    }

    IEnumerator WaitForDialogBox()
    {
        yield return new WaitUntil(DialogManager.current.IsLastBox);

        CameraManager.current.cameraList[1].Follow = null;
        WaypointController wpc = GameManager.current.guideAnimController.transform.parent.parent.GetComponent<WaypointController>();
        wpc.currentPauseIdx = -1;
        wpc.speed = 30;
        wpc.isPaused = false;

        DialogManager.current.allowProgression = false;

        yield return new WaitForSeconds(4);

        DialogManager.current.dialogBox.SetActive(false);

        SceneManager.LoadScene(levelIdx);
    }
}