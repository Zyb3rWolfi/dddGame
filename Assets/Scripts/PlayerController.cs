using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class FirstPersonController : MonoBehaviour
{
    public enum PlayerState { Idle, Crouching, Walking, Sprinting, Jumping }
    public PlayerState currentState = PlayerState.Idle;

    [Header("Heights")]
    public float standardHeight = 2f;
    public float crouchHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    [SerializeField] private float cameraStandOffset = 0.8f; // Height of eyes from pivot
    [SerializeField] private float cameraCrouchOffset = 0.2f; // Height of eyes when crouched

    [Header("References")]
    [SerializeField] private Transform cameraTransform; 
    private Rigidbody rb;
    private CapsuleCollider capsule;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float acceleration = 8f;
    
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upLimit = -90f;
    [SerializeField] private float downLimit = 90f;
    
    [Header("Stealth Settings")]
    [SerializeField] private SphereCollider audioTrigger; 
    [SerializeField] private float walkingRadius = 5f; 
    [SerializeField] private float sprintingRadius = 10f;

    // Internal State
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float currentSpeed;
    private float verticalRotation = 0f;
    private bool isSprinting;
    private bool isCrouching;
    private float currentHeight;
    public bool isPlayerCaught = false;
    [SerializeField] private GameObject[] respawnPoints;
    
    public static Action PlayWalkingSfx;
    public static Action PlaySprintingSfx;
    public static Action PlayJumpingSfx;
    public static Action levelFinished;

    private void OnEnable()
    {
        AIController.OnPlayerCaught += PlayerCaught;
        UIManager.ResetPosition += ResetPlayerPos;
    }

    private void OnDisable()
    {
        AIController.OnPlayerCaught -= PlayerCaught;
        UIManager.ResetPosition -= ResetPlayerPos;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHeight = standardHeight;
    }

    // --- INPUT SYSTEM CALLBACKS ---
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
    public void OnSprint(InputAction.CallbackContext context) => isSprinting = context.ReadValueAsButton();
    
    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.started) isCrouching = true;
        if (context.canceled)
        {
            
            if (!Physics.Raycast(transform.position, Vector3.up, 1.2f))
            {
                audioTrigger.enabled = false;
                isCrouching = false;
            }
            else
            {
                isCrouching = true; 
                audioTrigger.enabled = true; 
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && Mathf.Abs(rb.velocity.y) < 0.01f) 
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
            PlayJumpingSfx?.Invoke();
        }
    }

    void Update()
    {
        HandleLook();
        HandleSpeed();
        HandleCrouchHeight(); 
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX, 0));  
        
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, upLimit, downLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleSpeed()
    {
        float targetSpeed = walkSpeed;
        if (isCrouching) targetSpeed = crouchSpeed;
        else if (isSprinting) targetSpeed = sprintSpeed;

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    private void HandleCrouchHeight()
    {
        float targetHeight = isCrouching ? crouchHeight : standardHeight;
        float lastHeight = currentHeight;
        
        currentHeight = Mathf.MoveTowards(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        capsule.height = currentHeight;
        
        float centerShift = (standardHeight - currentHeight) / 2f;
        capsule.center = new Vector3(0, -centerShift, 0);

        float targetCamY = isCrouching ? cameraCrouchOffset : cameraStandOffset;
        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = camPos;

        if (!isCrouching && currentHeight > lastHeight)
        {
            rb.position += Vector3.up * 0.02f; 
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDir = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        Vector3 newVelocity = moveDir * currentSpeed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
        
        if (moveInput.magnitude > 0.1f && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            // Silent when crouching
            audioTrigger.enabled = !isCrouching; 
            
            if (!isCrouching)
            {
                if (isSprinting)
                {
                    PlaySprintingSfx?.Invoke();
                    audioTrigger.radius = sprintingRadius;
                }
                else
                {
                    PlayWalkingSfx?.Invoke();   
                    audioTrigger.radius = walkingRadius;
                }
            }
        } 
        else 
        {
            audioTrigger.enabled = false;
        }
    }

    private void PlayerCaught() => isPlayerCaught = true;

    private void ResetPlayerPos()
    {
        GameObject respawnPoint = respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)];
        transform.position = respawnPoint.transform.position;
        transform.rotation = respawnPoint.transform.rotation;
        isPlayerCaught = false;
    }
    
}