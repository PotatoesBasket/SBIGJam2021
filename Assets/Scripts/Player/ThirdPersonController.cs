using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Vector3 velocity;

    // components
    CharacterController charController;
    Animator animController;

    [SerializeField] SphereCollider groundCheck = null;
    [SerializeField] LayerMask groundLayerMask = 0;
    [SerializeField] SphereCollider interactCheck = null;
    [SerializeField] LayerMask interactLayerMask = 0;

    // player movement
    [SerializeField] float acceleration = 0;
    [SerializeField] float maxSpeed = 0;
    [SerializeField] float maxVerticalSpeed = 0;
    [SerializeField] float drag = 0;
    [SerializeField] float gravity = 0;

    //// camera movement
    //[SerializeField] float turnSpeed = 3;
    //[SerializeField] AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

    // pausing
    public bool physicsPaused = false;
    public bool playerInputPaused = false;
    public bool isAirborne = false;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        animController = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerInputPaused == false)
            UpdatePlayerInput();

        if (contextInput)
            DoThing();

        if (longContextInput)
            DoThingLonger();
    }

    private void FixedUpdate()
    {
        if (physicsPaused == false)
        {
            Gravity();
            Drag();
            Moving();

            charController.Move(transform.TransformVector(velocity) * Time.fixedDeltaTime);
        }
    }

    //==============================================================
    #region PRIVATE METHODS
    //==============================================================

    //--------------------------------------------------------------
    #region Input
    //--------------------------------------------------------------

    Vector2 joystick;
    bool contextInput;
    bool longContextInput;
    bool contextInputEnded;

    void UpdatePlayerInput()
    {
        joystick.x = Input.GetAxisRaw("Horizontal");
        joystick.y = Input.GetAxisRaw("Vertical");

        contextInput = Input.GetButtonDown("Button");
        longContextInput = Input.GetButton("Button");
        contextInputEnded = Input.GetButtonUp("Button");

        animController.SetFloat("Move X", joystick.x);
        animController.SetFloat("Move Y", joystick.y);
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Physics
    //--------------------------------------------------------------

    void Gravity()
    {
        if (IsGrounded() == false) // apply gravity
        {
            velocity.y -= gravity * Time.fixedDeltaTime;

            if (isAirborne == false)
                isAirborne = true;
        }
        else // reset y velocity on landing (prevents gravity accumulating)
        {
            if (isAirborne)
            {
                isAirborne = false;

                if (velocity.y < 3.0f) // arbitrary number to help with slopes
                {
                    velocity.y = 0;
                }
            }
        }
    }

    void Drag()
    {
        // apply drag
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        //velocity -= horizontalVelocity * drag * Time.fixedDeltaTime;

        velocity -= velocity * drag * Time.fixedDeltaTime;

        // cancel out movements that are too small
        if (velocity.x > -0.005f && velocity.x < 0.005f)
            velocity.x = 0;
        if (velocity.y > -0.005f && velocity.y < 0.005f)
            velocity.y = 0;
        if (velocity.z > -0.005f && velocity.z < 0.005f)
            velocity.z = 0;

        // cap horizontal speed
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 newVel = horizontalVelocity.normalized * maxSpeed;
            velocity = new Vector3(newVel.x, velocity.y, newVel.z);
        }

        // cap vertical speed
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
    }

    //void Looking()
    //{
    //    float mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouse.magnitude);
    //    transform.rotation *= Quaternion.AngleAxis(mouse.x * mouseSensitivityFactor * turnSpeed, Vector3.up);
    //}

    void Moving()
    {
        velocity.x += joystick.x * acceleration;
        velocity.z += joystick.y * acceleration;
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Context Triggers
    //--------------------------------------------------------------

    void DoThing()
    {
        if (IsInteracting())
        {
            Collider[] triggers = Physics.OverlapSphere(transform.TransformPoint(interactCheck.center), interactCheck.radius, interactLayerMask, QueryTriggerInteraction.Collide);

            List<IContextuallyActionable> actions = new List<IContextuallyActionable>();
            IContextuallyActionable action;

            foreach (Collider t in triggers)
            {
                if (t.TryGetComponent(out action))
                    actions.Add(action);
            }

            IContextuallyActionable highestPriority = actions[0];

            foreach (IContextuallyActionable a in actions)
            {
                if (a.GetPriority() > highestPriority.GetPriority())
                    highestPriority = a;
            }

            highestPriority.DoThing();
        }
    }

    void DoThingLonger()
    {
        if (IsInteracting())
        {
            Collider[] triggers = Physics.OverlapSphere(transform.TransformPoint(interactCheck.center), interactCheck.radius, interactLayerMask, QueryTriggerInteraction.Collide);

            List<IContextuallyActionable> actions = new List<IContextuallyActionable>();
            IContextuallyActionable action;

            foreach (Collider t in triggers)
            {
                if (t.TryGetComponent(out action))
                    actions.Add(action);
            }

            IContextuallyActionable highestPriority = actions[0];

            foreach (IContextuallyActionable a in actions)
            {
                if (a.GetPriority() > highestPriority.GetPriority())
                    highestPriority = a;
            }

            highestPriority.DoThingLonger();
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

    public void SetAnimatorPauseState(bool state)
    {
        animController.speed = (state ? 0.0f : 1.0f);
    }

    public void SetAllPauseState(bool state)
    {
        SetPhysicsPauseState(state);
        SetPlayerInputPauseState(state);
        SetAnimatorPauseState(state);
    }

    #endregion
    //--------------------------------------------------------------

    //--------------------------------------------------------------
    #region Misc
    //--------------------------------------------------------------

    public void ClearInput()
    {
        joystick = Vector2.zero;
        contextInput = false;
        longContextInput = false;
    }

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
        // trigger events
        if (other.TryGetComponent(out ITriggerableEvent e))
            e.DoThing();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded() ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.8f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheck.center), groundCheck.radius);

        Gizmos.color = IsInteracting() ? new Color(0, 1, 1, 0.5f) : new Color(1, 0, 1, 0.8f);
        Gizmos.DrawSphere(transform.TransformPoint(interactCheck.center), interactCheck.radius);
    }
}