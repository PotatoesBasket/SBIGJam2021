using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Vector3 velocity;

    // components
    CharacterController charController;
    Animator animController;
    RotateObjectWithMouse camController;

    [SerializeField] SphereCollider groundCheck = null;
    [SerializeField] LayerMask groundLayerMask = 0;
    [SerializeField] SphereCollider interactCheck = null;
    [SerializeField] LayerMask interactLayerMask = 0;

    // player movement
    [SerializeField] float acceleration = 0;
    [SerializeField] float maxSpeed = 0;
    [SerializeField] float maxVerticalSpeed = 0;
    [SerializeField] float drag = 0;

    // camera movement
    [SerializeField] float turnSpeed = 3;
    [SerializeField] AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    // pausing
    public bool physicsPaused = false;
    public bool playerInputPaused = false;
    public bool cameraInputPaused = false;
    public bool isAirborne = false;

    private void Start()
    {
        charController = GetComponent<CharacterController>();
        animController = GetComponent<Animator>();
        camController = GetComponentInChildren<RotateObjectWithMouse>();
    }

    private void Update()
    {
        UpdateDebugControls();

        if (!GameManager.current.GamePaused)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

            if (!playerInputPaused)
                UpdatePlayerInput();

            if (!cameraInputPaused)
                UpdateCameraInput();

            if (contextInput)
                DoThing();
        }
    }

    private void FixedUpdate()
    {
        if (!GameManager.current.GamePaused)
        {
            if (!physicsPaused)
            {
                Gravity();
                Drag();
                //Looking();
                Moving();

                charController.Move(transform.TransformVector(velocity) * Time.fixedDeltaTime);
            }
        }
    }

    //==============================================================
    #region PRIVATE METHODS
    //==============================================================

    //--------------------------------------------------------------
    #region Input
    //--------------------------------------------------------------

    Vector2 joystick;
    Vector2 mouse;
    bool contextInput;
    bool longContextInput;

    void UpdateDebugControls()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerInputPaused = !playerInputPaused;
            Debug.Log("Player input " + (playerInputPaused ? "paused" : "unpaused"));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cameraInputPaused = !cameraInputPaused;
            camController.IsPaused = !camController.IsPaused;
            Debug.Log("Player-rotated camera input " + (cameraInputPaused ? "paused" : "unpaused"));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            physicsPaused = !physicsPaused;
            Debug.Log("Physics " + (physicsPaused ? "paused" : "unpaused"));
        }
    }

    void UpdatePlayerInput()
    {
        joystick.x = Input.GetAxisRaw("Horizontal");
        joystick.y = Input.GetAxisRaw("Vertical");

        contextInput = Input.GetButtonDown("Button");
        longContextInput = Input.GetButton("Button");

        animController.SetFloat("Move X", joystick.x);
        animController.SetFloat("Move Y", joystick.y);
    }

    void UpdateCameraInput()
    {
        mouse.x = Input.GetAxis("Mouse X");
        mouse.y = Input.GetAxis("Mouse Y");
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Physics
    //--------------------------------------------------------------

    void Gravity()
    {
        if (!IsGrounded()) // apply gravity
        {
            Vector3 gravity = Vector3.up * Physics.gravity.y;

            velocity += gravity * Time.fixedDeltaTime;

            if (!isAirborne)
                isAirborne = true;
        }
        else // reset y velocity on landing (prevents gravity accumulating)
        {
            if (isAirborne)
            {
                isAirborne = false;

                if (velocity.y < -3.0f) // arbitrary number to help with slopes
                {
                    velocity.y = 0;
                }
            }
        }

        animController.SetBool("isAirborne", isAirborne);
        animController.SetFloat("VelY", velocity.y);
    }

    void Drag()
    {
        // apply drag
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        velocity -= horizontalVelocity * drag * Time.fixedDeltaTime;

        // cancel out movements that are too small
        if (velocity.magnitude <= 0.005f)
            velocity = Vector3.zero;

        // cap horizontal speed
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 newVel = horizontalVelocity.normalized * maxSpeed;
            velocity = new Vector3(newVel.x, velocity.y, newVel.z);
        }

        // cap vertical speed
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
    }

    void Looking()
    {
        float mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouse.magnitude);
        transform.rotation *= Quaternion.AngleAxis(mouse.x * mouseSensitivityFactor * turnSpeed, Vector3.up);
    }

    void Moving()
    {
        velocity.x += joystick.x * acceleration;
        velocity.z += joystick.y * acceleration;
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Misc
    //--------------------------------------------------------------

    void DoThing()
    {
        if (IsInteracting())
        {
            Collider[] triggers = Physics.OverlapSphere(transform.TransformPoint(interactCheck.center), interactCheck.radius, interactLayerMask, QueryTriggerInteraction.Collide);

            foreach (Collider trigger in triggers)
            {
                if (trigger.TryGetComponent(out IContextuallyActionable context))
                    context.DoThing();
            }
        }
    }
    #endregion
    //--------------------------------------------------------------

    #endregion
    //==============================================================

    //==============================================================
    #region PUBLIC METHODS
    //==============================================================

    //--------------------------------------------------------------
    #region Environment Checks
    //--------------------------------------------------------------

    public bool IsGrounded()
    {
        if (groundCheck != null)
        {
            if (Physics.CheckSphere(transform.TransformPoint(groundCheck.center), groundCheck.radius, groundLayerMask, QueryTriggerInteraction.Ignore))
                return true;
        }

        return false;
    }

    public bool IsInteracting()
    {
        if (interactCheck != null)
        {
            if (Physics.CheckSphere(transform.TransformPoint(interactCheck.center), interactCheck.radius, interactLayerMask, QueryTriggerInteraction.Collide))
                return true;
        }

        return false;
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Pausing
    //--------------------------------------------------------------

    public void SetPhysicsPauseState(bool state)
    {
        physicsPaused = state;
    }

    public void SetPlayerInputPauseState(bool state)
    {
        playerInputPaused = state;
    }

    public void SetCameraInputPauseState(bool state)
    {
        cameraInputPaused = state;
        camController.IsPaused = state;
    }

    public void SetAnimatorPauseState(bool state)
    {
        animController.speed = (state ? 0.0f : 1.0f);
    }

    public void SetAllPauseState(bool state)
    {
        SetPhysicsPauseState(state);
        SetPlayerInputPauseState(state);
        SetCameraInputPauseState(state);
        SetAnimatorPauseState(state);
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Misc
    //--------------------------------------------------------------

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    #endregion
    //--------------------------------------------------------------

    #endregion
    //==============================================================

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITriggerableEvent e))
        {
            e.DoThing();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded() ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.8f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheck.center), groundCheck.radius);

        Gizmos.color = IsInteracting() ? new Color(0, 1, 1, 0.5f) : new Color(1, 0, 1, 0.8f);
        Gizmos.DrawSphere(transform.TransformPoint(interactCheck.center), interactCheck.radius);
    }
}