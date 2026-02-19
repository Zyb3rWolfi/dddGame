using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    Vector2 moveInput;
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 direction = new  Vector3(moveInput.x, 0,moveInput.y);
        
        transform.Translate(direction * speed * Time.deltaTime);
        
    }

    public void WASDMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void JumpMovement(InputAction.CallbackContext context)
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
    }
}
