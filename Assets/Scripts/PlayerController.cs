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

    // Internal State
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float currentSpeed;
    private float verticalRotation = 0f;
    private bool isSprinting;

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
        }
    }


    void Update()
    {
        HandleRotation();
        HandleSpeed();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleRotation()
    {
        // 1. Horizontal Rotation: Rotate the whole Player body left/right
        float mouseX = lookInput.x * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // 2. Vertical Rotation: Rotate ONLY the camera up/down
        float mouseY = lookInput.y * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, upLimit, downLimit);
        
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
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
    }
}