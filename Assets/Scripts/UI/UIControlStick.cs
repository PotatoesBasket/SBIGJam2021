using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControlStick : MonoBehaviour
{
    [SerializeField] float upDownMaxRot = 0;
    [SerializeField] float leftRightMaxRot = 0;

    private void Update()
    {
        float xRemap = Mathf.InverseLerp(-1, 1, Input.GetAxis("Vertical"));
        float xAngle = Mathf.Lerp(upDownMaxRot, -upDownMaxRot, xRemap);

        float zRemap = Mathf.InverseLerp(-1, 1, Input.GetAxis("Horizontal"));
        float zAngle = Mathf.Lerp(leftRightMaxRot, -leftRightMaxRot, zRemap);

        transform.rotation = Quaternion.Euler(new Vector3(xAngle, 0, zAngle));
    }
}