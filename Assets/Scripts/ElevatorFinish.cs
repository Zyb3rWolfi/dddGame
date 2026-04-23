using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFinish : MonoBehaviour
{
    public static Action levelFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Playerrrr");
            levelFinished?.Invoke();
        }
    }
}
