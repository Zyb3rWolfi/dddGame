using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactText;
    
    private float distanceToInteract = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, distanceToInteract))
        {
            DenaryPuzzle puzzle = hit.collider.GetComponent<DenaryPuzzle>();

            if (hit.collider.CompareTag("Interactable") && !puzzle.IsPuzzleCompleted)
            {
                interactText.text = "Press E to interact";
            }
            else
            {
                interactText.text = "";
            }
        }
        else
        {
            interactText.text = "";
        }
    }
    
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
        
            if (Physics.Raycast(ray, out RaycastHit hit, distanceToInteract))
            {
                DenaryPuzzle puzzle = hit.collider.GetComponent<DenaryPuzzle>();

                if (hit.collider.CompareTag("Interactable") && !puzzle.IsPuzzleCompleted)
                {
                    // Implement interaction logic here
                    puzzle.ShowUI();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }
}
