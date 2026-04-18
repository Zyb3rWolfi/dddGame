using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DenaryPuzzle : MonoBehaviour
{
    public bool IsPuzzleCompleted
    {
        get => isPuzzleCompleted;
        set => isPuzzleCompleted = value;
    }

    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject answerButtons;
    [SerializeField] private TextMeshProUGUI question;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private bool isPuzzleCompleted = false;
    [SerializeField] private GameObject material;
    [SerializeField] public bool inProgress;
    
    // Random questions and answers
    [SerializeField] private PuzzleQuestions[] questions;
    public static Action OnPuzzleComplete;
    public static Action StopKeyboardSfx;
    private PuzzleQuestions chosenQuestion;
    
    private void Start() => GenerateProblem();
    public void GenerateProblem()
    {
        if (isPuzzleCompleted) return;
        chosenQuestion = questions[Random.Range(0, questions.Length)];
    }

    private void OnEnable()
    {
        Interaction.onPuzzleSubmit += CheckAnswer;
    }
    
    private void OnDisable()
    {
        Interaction.onPuzzleSubmit -= CheckAnswer;
    }

    public void ShowUI()
    {
        uiPanel.SetActive(true);
        question.text = chosenQuestion.question;
        StartCoroutine(StartTimer(10));
        int i = 0;
        foreach (var button in answerButtons.GetComponentsInChildren<Button>())
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = chosenQuestion.answers[i];
            i++;
        }
    }

    public void CheckAnswer(int buttonIndex)
    {
        if (this.inProgress == false) return;
        if (buttonIndex == chosenQuestion.correctAnswerIndex)
        {
            uiPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnPuzzleComplete?.Invoke();
            material.GetComponent<MeshRenderer>().material.color = Color.green; // Change material color to green on correct answer
            isPuzzleCompleted = true;
            StopKeyboardSfx?.Invoke();
        }
    }
    
    public IEnumerator StartTimer(float duration)
    {
        float timeRemaining = duration;
        while (timeRemaining > 0)
        {
            timer.text = $"{Mathf.Ceil(timeRemaining)}s";
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }
        timer.text = "Time's up!";
        // Implement logic for when time runs out (e.g., reset puzzle, penalize player, etc.)
    }

}
