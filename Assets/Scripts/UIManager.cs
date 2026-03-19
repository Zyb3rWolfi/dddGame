using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI puzzlesCompleteText;
    private int puzzlesCompleted = 0;

    private void OnEnable()
    {
        
        DenaryPuzzle.OnPuzzleComplete += OnPuzzleComplete;
    }

    private void OnDisable()
    {
        DenaryPuzzle.OnPuzzleComplete -= OnPuzzleComplete;
    }

    private void OnPuzzleComplete()
    {
        puzzlesCompleted++;
        
        puzzlesCompleteText.text = $"Puzzles Completed: {puzzlesCompleted}/5";
    }
}
