using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager current = null;

    public GameObject dialogBox = null;
    public Text nameTag = null;
    public Text dialogText = null;

    Dictionary<uint, List<string>> dialogDictionary = new Dictionary<uint, List<string>>();

    string dialogFilePath = "Dialog.txt";
    List<string> currentPendingDialog = new List<string>();
    public Animator currentAnimator = null;
    public CinemachineVirtualCamera currentCam = null;

    enum DialogState
    {
        Starting,
        Continuing,
        DecidingNextState,
        WaitForNext,
        WaitForClose,
        Closed
    }

    DialogState currentDialogState = DialogState.Closed;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadDialogFromFile();
    }

    private void Update()
    {
        switch (currentDialogState)
        {
            case DialogState.Starting:
                {
                    LoadNextDialogLine(true);
                    currentDialogState = DialogState.DecidingNextState;
                }
                break;

            case DialogState.Continuing:
                {
                    LoadNextDialogLine();
                    currentDialogState = DialogState.DecidingNextState;
                }
                break;

            case DialogState.DecidingNextState:
                {
                    if (currentPendingDialog.Count > 0)
                        currentDialogState = DialogState.WaitForNext;
                    else
                        currentDialogState = DialogState.WaitForClose;
                }
                break;

            case DialogState.WaitForNext:
                if (Input.GetButtonDown("Button"))
                {
                    currentDialogState = DialogState.Continuing;
                }
                break;

            case DialogState.WaitForClose:
                if (Input.GetButtonDown("Button"))
                {
                    EndDialogBlock();
                    currentDialogState = DialogState.Closed;
                }
                break;

            case DialogState.Closed:
                break;
        }
    }

    public void LoadNextDialogBlock(uint dialogID = 0)
    {
        GameManager.current.PlayerController.SetAllPauseState(true);

        // find dialog block in dictionary by ID
        if (dialogDictionary.TryGetValue(dialogID, out List<string> lines))
            currentPendingDialog = lines;

        currentDialogState = DialogState.Starting;
    }

    void LoadNextDialogLine(bool firstLine = false)
    {
        string line = currentPendingDialog[0];
        int trimNo = 0;

        while (line[trimNo] == '[') // get dialog line attributes
        {
            char attributeCode = line[trimNo + 1];

            switch (attributeCode)
            {
                case 'N': // get current speaker name
                    {
                        //nameTag.text = attributeName

                        trimNo += 12;
                    }
                    break;

                case 'A': // get next animation property
                    {
                        int.TryParse(line[2].ToString(), out int idx);

                        currentAnimator = GameManager.current.animatorControllerList[idx];

                        int.TryParse(line[3].ToString(), out idx);
                        int.TryParse(line[4].ToString(), out int idx2);

                        if (currentAnimator != null)
                            currentAnimator.SetBool(GameManager.current.animatorPropertyList[idx], idx2 == 1);

                        trimNo += 6;
                    }
                    break;

                case 'C': // get next camera
                    {
                        int.TryParse(line[2].ToString(), out int idx);

                        currentCam = CameraManager.current.cameraList[idx];

                        int.TryParse(line[3].ToString(), out idx);

                        if (currentCam != null)
                            currentCam.enabled = idx == 1;

                        trimNo += 5;
                    }
                    break;

                case 'E': // extra
                    {
                        //do HUD slide in thing

                        trimNo += 3;
                    }
                    break;

                    throw new System.Exception("Something went wrong trying to read dialog line attribute " + attributeCode + " from line [" + currentPendingDialog + "]");
            }
        }

        dialogText.text = line.Remove(0, trimNo); // write dialog line into UI text
        currentPendingDialog.RemoveAt(0); // remove used dialog from list

        if (firstLine)
            BeginDialogBlock(); //StartCoroutine(WaitForCamThenOpenDialog());
    }

    IEnumerator WaitForCamThenOpenDialog()
    {
        yield return new WaitUntil(CameraManager.current.CheckCamBlendState); // need to wait like a frame or something for blend to actually start

        while (CameraManager.current.CheckCamBlendState()) // run until cam is finished blending
        {
            yield return null;
        }

        BeginDialogBlock(); // then open the prepared dialog
    }

    void BeginDialogBlock()
    {
        if (currentAnimator != null)
            currentAnimator.SetBool("isTalking", true);

        dialogBox.SetActive(true);
    }

    void EndDialogBlock()
    {
        if (currentCam != null)
            currentCam.enabled = false;

        if (currentAnimator != null)
            currentAnimator.SetBool("isTalking", false);

        currentCam = null;
        currentAnimator = null;

        dialogBox.SetActive(false);
        GameManager.current.PlayerController.SetAllPauseState(false);
    }

    void LoadDialogFromFile()
    {
        StreamReader file = new StreamReader(dialogFilePath);

        uint blockID = 0;
        List<string> block = new List<string>();

        file.ReadLine(); // skip note line

        while (!file.EndOfStream)
        {
            string line = file.ReadLine();

            if (line[0] != '=') // not new block, add to current block list
            {
                block.Add(line);
            }
            else // new block, copy current block list to dictionary and clear
            {
                List<string> blockCopy = new List<string>();

                foreach (string l in block)
                    blockCopy.Add((string)l.Clone());

                dialogDictionary.Add(blockID, blockCopy);
                ++blockID;
                block.Clear();
            }
        }

        file.Close();
    }
}