using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager current = null;

    public List<CinemachineVirtualCamera> cameraList = new List<CinemachineVirtualCamera>();

    [SerializeField] CinemachineVirtualCamera introCam = null;
    [SerializeField] CinemachineVirtualCamera playCam01 = null;       // 0
    [SerializeField] CinemachineVirtualCamera guideCam = null;        // 1
    [SerializeField] CinemachineVirtualCamera AICam = null;           // 2
    [SerializeField] CinemachineVirtualCamera catCam = null;          // 3
    [SerializeField] CinemachineVirtualCamera towerCam = null;        // 4

    CinemachineBrain camBrain;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
        {
            Destroy(current.gameObject);
            current = this;
        }
    }

    private void Start()
    {
        camBrain = Camera.main.GetComponent<CinemachineBrain>();
        LoadCameraList();

        introCam.enabled = false;
    }

    public bool CamIsBlending()
    {
        return camBrain.IsBlending;
    }

    void LoadCameraList()
    {
        // camera attribute order: camera index, state (0 off, 1 on)
        // (eg: disable guide's cam = 10)

        cameraList.Add(playCam01);      // 0
        cameraList.Add(guideCam);       // 1
        cameraList.Add(AICam);          // 2
        cameraList.Add(catCam);         // 3
        cameraList.Add(towerCam);       // 4
    }
}