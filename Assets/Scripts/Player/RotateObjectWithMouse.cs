using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour
{
    [SerializeField] float speed = 3;
    [SerializeField] float lookUpLimit = -40;
    [SerializeField] float lookDownLimit = 40;
    [SerializeField] AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    public bool IsPaused { get; set; }

    Vector2 mouse;

    private void Update()
    {
        if (IsPaused == false)
            UpdateInput();
    }

    private void FixedUpdate()
    {
        if (IsPaused == false)
        {
            float mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouse.magnitude);
            transform.rotation *= Quaternion.AngleAxis(-mouse.y * mouseSensitivityFactor * speed, Vector3.right);

            Vector3 angles = transform.localEulerAngles;
            angles.z = 0;

            float angle = transform.localEulerAngles.x;

            if (angle > 180 && angle < 360 + lookUpLimit)
                angles.x = 360 + lookUpLimit;
            else if (angle < 180 && angle > lookDownLimit)
                angles.x = lookDownLimit;

            transform.localEulerAngles = angles;
        }
    }

    void UpdateInput()
    {
        mouse.x = Input.GetAxis("Mouse X");
        mouse.y = Input.GetAxis("Mouse Y");
    }
}