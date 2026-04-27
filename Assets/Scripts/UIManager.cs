using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    FirstPersonController playerController;
    [SerializeField] private TextMeshProUGUI puzzlesCompleteText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject DarknessOverlay;
    [SerializeField] private GameObject finishLine;
    [SerializeField] private GameObject finishScreen;
    [SerializeField] Thresholds thresholds;
    [SerializeField] private Slider staminaBar; // Assign your UI Slider here
    [SerializeField] private TextMeshProUGUI gradeText;
    
    public float realTimeLimit = 300f;
    [SerializeField]private float elapsedRealTime = 0f;
    private bool isLevelFinished = false;

    private float startSeconds = 23f * 3600f;
    
    private int puzzlesCompleted = 0;
    
    public static Action ResetPosition;
    public static Action openElevator;

    private void Start()
    {
        playerController = FindObjectOfType<FirstPersonController>();
        
        if (staminaBar != null) staminaBar.value = 1f;
    }

    private void OnEnable()
    {
        
        DenaryPuzzle.OnPuzzleComplete += OnPuzzleComplete;
        AIController.OnPlayerCaught += PlayerCaughtScreen;
        ElevatorFinish.levelFinished += ShowFinishScreen;
        
        FirstPersonController.OnStaminaChanged += UpdateStaminaUI;
    }

    private void OnDisable()
    {
        DenaryPuzzle.OnPuzzleComplete -= OnPuzzleComplete;
        AIController.OnPlayerCaught -= PlayerCaughtScreen;
        ElevatorFinish.levelFinished -= ShowFinishScreen;
        
        FirstPersonController.OnStaminaChanged -= UpdateStaminaUI;
    }
    
    private void UpdateStaminaUI(float staminaPercentage)
    {
        if (staminaBar != null)
        {
            staminaBar.value = staminaPercentage;
        }
    }
    
    private void OnPuzzleComplete()
    {
        puzzlesCompleted++;
        if (puzzlesCompleted >= 2)
        {
            puzzlesCompleteText.text = "All puzzles completed! Exit via the elevator...";
            openElevator?.Invoke();
            return;
        }
        puzzlesCompleteText.text = $"Find & Complete Puzzles to exit [{puzzlesCompleted}/2]";
    }

    private void Update()
    {
        if (!isLevelFinished)
        {
            if (elapsedRealTime < realTimeLimit)
            {
                elapsedRealTime += Time.deltaTime;

                // Multiplier of 30 means 2 minutes of real time = 1 hour of game time
                // 23:00 (Start) -> 00:00 (Midnight/Deadline)
                float gameTimeMultiplier = 20f; 
                float currentGameSeconds = startSeconds + (elapsedRealTime * gameTimeMultiplier);
            
                timerText.text = FormatTime(currentGameSeconds);
            }
            else
            {
                // DEADLINE REACHED
                elapsedRealTime = realTimeLimit;
                timerText.text = "00:00"; // Force show Midnight
            }
        }
    }

    string FormatTime(float totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f) % 24;
        int minutes = Mathf.FloorToInt(totalSeconds / 60f) % 60;
        
        return string.Format("{0:00}:{1:00}", hours, minutes);    
    }
    
    private void PlayerCaughtScreen()
    {
        if (playerController.isPlayerCaught) return;
        elapsedRealTime += 30f;
        StartCoroutine(blankScreen(5f));

    }
    
    private IEnumerator blankScreen(float duration)
    {
        DarknessOverlay.SetActive(true);
        Mathf.Lerp(DarknessOverlay.GetComponent<Image>().color.a , 1f, Time.deltaTime * 5f);
        yield return new WaitForSeconds(duration);
        ResetPosition?.Invoke();
        DarknessOverlay.SetActive(false);
    }
    
    private void ShowFinishScreen()
    {
        isLevelFinished = true;
        finishScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gradeText.text =  "Grade Achieved: " + thresholds.CalculateGrade(elapsedRealTime);
        
    }
}

