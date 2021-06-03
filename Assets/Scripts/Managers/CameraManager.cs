using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager current = null;

    public List<CinemachineVirtualCamera> cameraList = new List<CinemachineVirtualCamera>();

    public CinemachineVirtualCamera playerCam = null; // cam attached to back of player, always active and is default
    public CinemachineVirtualCamera guideCam = null;

    CinemachineBrain camBrain;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        camBrain = Camera.main.GetComponent<CinemachineBrain>();
        LoadCameraList();
    }

    public bool CheckCamBlendState()
    {
        return camBrain.IsBlending;
    }

    void LoadCameraList()
    {
        // camera attribute order: camera index, state (0 off, 1 on)
        // (eg: disable guide's cam = 10)

        cameraList.Add(playerCam);  //0
        cameraList.Add(guideCam);   //1
    }
}