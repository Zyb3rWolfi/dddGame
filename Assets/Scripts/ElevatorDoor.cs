using System;
using UnityEngine;
using System.Collections;

public class ElevatorDoor : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The scale the door reaches when fully open (e.g., 0.1 on the X axis)")]
    [SerializeField] private Vector3 targetOpenScale = new Vector3(0.01f, 2f, 1f);
    [SerializeField] private float openSpeed = 2f;

    private Vector3 closedScale;
    private bool isOpen = false;

    private void OnEnable()
    {
        UIManager.openElevator += OpenDoor;
    }
    
    private void OnDisable()
    {
        UIManager.openElevator -= OpenDoor;
    }
    void Start()
    {
        closedScale = transform.localScale;
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            gameObject.SetActive(false); 
        }
    }

    private IEnumerator ScaleDoor(Vector3 target)
    {
        float progress = 0;
        Vector3 initialScale = transform.localScale;

        while (progress < 1f)
        {
            progress += Time.deltaTime * openSpeed;
            
            transform.localScale = Vector3.Lerp(initialScale, target, progress);
            
            yield return null; 
        }

        transform.localScale = target;
    }
}