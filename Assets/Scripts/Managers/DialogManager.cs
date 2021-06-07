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

    string dialogFilePath = Application.streamingAssetsPath + "/Dialog.txt";
    List<string> currentPendingDialog = new List<string>();
    public Animator currentAnimator = null;
    public CinemachineVirtualCamera currentCam = null;

    public bool allowProgression = true;

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
                if (Input.GetButtonDown("Button") && !CameraManager.current.CamIsBlending() && allowProgression)
                {
                    currentDialogState = DialogState.Continuing;
                    AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.dialogContinueBoop);
                }
                break;

            case DialogState.WaitForClose:
                if (Input.GetButtonDown("Button") && !CameraManager.current.CamIsBlending() && allowProgression)
                {
                    EndDialogBlock();
                    currentDialogState = DialogState.Closed;
                    AudioManager.current.SFXDefaultSource.PlayOneShot(AudioManager.current.dialogContinueBoop);
                    AudioManager.current.SFXDefaultSource.Stop();
                }
                break;

            case DialogState.Closed:
                break;
        }
    }

    public void LoadNextDialogBlock(uint dialogID = 0)
    {
        GameManager.current.playerController.SetAllPauseState(true);

        // find dialog block in dictionary by ID
        if (dialogDictionary.TryGetValue(dialogID, out List<string> lines))
            currentPendingDialog = lines;

        currentDialogState = DialogState.Starting;
    }

    public bool IsLastBox()
    {
        return currentDialogState == DialogState.WaitForClose;
    }

    public bool IsClosed()
    {
        return currentDialogState == DialogState.Closed;
    }

    void LoadNextDialogLine(bool firstLine = false)
    {
        string line = currentPendingDialog[0];
        int trimNo = 0;

        while (line[trimNo] == '[') // get dialog line attributes
        {
            char attributeCode = line[trimNo + 1];

            Debug.Log("Processing line: " + line);
            Debug.Log("Current trimNo: " + trimNo);
            Debug.Log("Attr code: " + attributeCode);

            switch (attributeCode)
            {
                case 'N': // get current speaker name
                    {
                        Debug.Log("Processing nametag");

                        int startIdx = trimNo + 2;
                        int endIdx = line.IndexOf('}');

                        char[] name = line.ToCharArray(startIdx, endIdx - startIdx);

                        nameTag.text = new string(name);

                        trimNo += name.Length + 4;
                    }
                    break;

                case 'A': // get next animation property
                    {
                        Debug.Log("Processing animation");

                        int.TryParse(line[trimNo + 2].ToString(), out int idx);

                        currentAnimator = GameManager.current.animatorControllerList[idx];

                        int.TryParse(line[trimNo + 3].ToString(), out idx);
                        int.TryParse(line[trimNo + 4].ToString(), out int idx2);

                        if (currentAnimator != null)
                            currentAnimator.SetBool(GameManager.current.animatorPropertyList[idx], idx2 == 1);

                        trimNo += 6;
                    }
                    break;

                case 'C': // get next camera
                    {
                        Debug.Log("Processing camera");

                        int.TryParse(line[trimNo + 2].ToString(), out int idx);

                        currentCam = CameraManager.current.cameraList[idx];

                        int.TryParse(line[trimNo + 3].ToString(), out idx);

                        if (currentCam != null)
                            currentCam.enabled = idx == 1;

                        Debug.Log(currentCam.name + " set to " + (idx == 1));

                        trimNo += 5;
                    }
                    break;

                case 'E': // extra
                    {
                        Debug.Log("Processing eeeeee");

                        GameManager.current.SlideInHUD();

                        trimNo += 3;
                    }
                    break;

                    throw new System.Exception("Something went wrong trying to read dialog line attribute " + attributeCode + " from line [" + currentPendingDialog + "]");
            }
        }

        dialogText.text = line.Remove(0, trimNo); // write dialog line into UI text
        currentPendingDialog.RemoveAt(0); // remove used dialog from list

        Debug.Log("Pushed processed line into dialog display: " + line.Remove(0, trimNo));

        if (firstLine)
        {
            if (currentCam == null)
                BeginDialogBlock();
            else
                StartCoroutine(WaitForCamThenOpenDialog());
        }
        else
        {
            StartTalkingSFX();
        }
    }

    IEnumerator WaitForCamThenOpenDialog()
    {
        yield return new WaitUntil(CameraManager.current.CamIsBlending); // need to wait like a frame or something for blend to actually start

        while (CameraManager.current.CamIsBlending()) // run until cam is finished blending
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

        StartTalkingSFX();
    }

    public void EndDialogBlock()
    {
        if (currentCam != null)
            currentCam.enabled = false;

        if (currentAnimator != null)
            currentAnimator.SetBool("isTalking", false);

        currentCam = null;
        currentAnimator = null;

        dialogBox.SetActive(false);
        GameManager.current.playerController.SetAllPauseState(false);
    }

    void LoadDialogFromFile()
    {
        StreamReader file = new StreamReader(dialogFilePath);

        uint blockID = 0;
        List<string> block = new List<string>();

        file.ReadLine(); // skip note line

        Debug.Log("begin dialog file read");

        while (!file.EndOfStream)
        {
            string line = file.ReadLine(); // read in next line

            // check if line is start of new block
            if (line[0] == '=') // yes
            {
                // process completed block
                List<string> blockCopy = new List<string>();

                foreach (string l in block)
                    blockCopy.Add((string)l.Clone());

                dialogDictionary.Add(blockID, blockCopy);
                block.Clear();

                Debug.Log("Block " + blockID + " added to block dictionary");

                // get new block ID
                string ID = new string(line.ToCharArray(), 3, 2);
                uint.TryParse(ID, out blockID);
            }
            else // no
            {
                // add line to current block
                block.Add(line);

                Debug.Log("Line (" + line + ") added to block " + blockID);
            }
        }

        // process final block
        List<string> finalBlockCopy = new List<string>();

        foreach (string l in block)
            finalBlockCopy.Add((string)l.Clone());

        dialogDictionary.Add(blockID, finalBlockCopy);
        Debug.Log("Block " + blockID + " added to block dictionary");

        Debug.Log("end dialog file read");

        file.Close();
    }

    void StartTalkingSFX()
    {
        AudioManager.current.SFXDefaultSource.clip = AudioManager.current.gibberish.GetRandomizedClip();
        AudioManager.current.SFXDefaultSource.loop = false;
        AudioManager.current.SFXDefaultSource.Play();
    }
}