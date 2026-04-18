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
    public float realTimeLimit = 300f;
    private float elapsedRealTime = 0f;

    private float startSeconds = 23f * 3600f;
    
    private int puzzlesCompleted = 0;
    
    public static Action ResetPosition;

    private void Start()
    {
        playerController = FindObjectOfType<FirstPersonController>();
    }

    private void OnEnable()
    {
        
        DenaryPuzzle.OnPuzzleComplete += OnPuzzleComplete;
        AIController.OnPlayerCaught += PlayerCaughtScreen;
    }

    private void OnDisable()
    {
        DenaryPuzzle.OnPuzzleComplete -= OnPuzzleComplete;
        AIController.OnPlayerCaught -= PlayerCaughtScreen;
    }

    private void OnPuzzleComplete()
    {
        puzzlesCompleted++;
        if (puzzlesCompleted >= 2)
        {
            puzzlesCompleteText.text = "All puzzles completed! Exit via the elevator...";
            return;
        }
        puzzlesCompleteText.text = $"Find & Complete Puzzles to exit [{puzzlesCompleted}/2]";
    }

    private void Update()
    {
        if (elapsedRealTime < realTimeLimit)
        {
            elapsedRealTime += Time.deltaTime;

            float currentGameSeconds = (elapsedRealTime * 12f) + startSeconds;
            
            timerText.text = FormatTime(currentGameSeconds);
            
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
}

