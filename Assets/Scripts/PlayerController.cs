using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform; // Drag your Virtual Camera here
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float acceleration = 8f;
    
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upLimit = -90f;
    [SerializeField] private float downLimit = 90f;
    [SerializeField] public bool isPlayerCaught = false;
    
    [SerializeField] private GameObject[] respawnPoints;
    [SerializeField] private SphereCollider audioTrigger; // For proximity-based audio cues
    [SerializeField] private float walkingRadius = 5f; // Radius for walking audio cues
    [SerializeField] private float sprintingRadius = 10f; // Radius for sprinting audio cues
    
    // Internal State
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float currentSpeed;
    private float verticalRotation = 0f;
    private bool isSprinting;
    
    public static Action PlayWalkingSfx;
    public static Action PlaySprintingSfx;
    public static Action PlayJumpingSfx;

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
    
    private void PlayerCaught()
    {
        isPlayerCaught = true;
    }
    
    private void ResetPlayerPos()
    {
        // Choose a random respawn point
        GameObject respawnPoint = respawnPoints[UnityEngine.Random.Range(0, respawnPoints.Length)];
        transform.position = respawnPoint.transform.position;
        transform.rotation = respawnPoint.transform.rotation;
        isPlayerCaught = false;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Setup Rigidbody for First Person
        rb.freezeRotation = true; // Stop physics from tilting the player
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) isSprinting = true;
        if (context.canceled) isSprinting = false;
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && Mathf.Abs(rb.velocity.y) < 0.01f) // Simple check to prevent double jumps
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
            PlayJumpingSfx?.Invoke();
        }
    }


    void Update()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, mouseX, 0));  
        // 2. Vertical Rotation: Rotate ONLY the camera up/down
        float mouseY = lookInput.y * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, upLimit, downLimit);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        
        HandleSpeed();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }
    

    private void HandleSpeed()
    {
        // Gradually increase/decrease speed
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // Create direction relative to where the player is facing
        Vector3 moveDir = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        
        // Apply velocity but keep the existing vertical (Y) velocity for gravity
        Vector3 newVelocity = moveDir * currentSpeed;
        newVelocity.y = rb.velocity.y;

        rb.velocity = newVelocity;
        
        // Play walking SFX if moving and on the ground
        if (moveInput.magnitude > 0.1f && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            audioTrigger.enabled = true; // Enable the audio trigger when walking
            if (isSprinting)
            {
                PlaySprintingSfx?.Invoke();
                audioTrigger.radius = sprintingRadius; // Set larger radius for sprinting
            }
            else
            {
                PlayWalkingSfx?.Invoke();   
                audioTrigger.radius = walkingRadius; // Set smaller radius for walking
            }
        } else {
            audioTrigger.enabled = false; // Disable the audio trigger when not walking
        }
    }
}