using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Puzzle Question", menuName = "Puzzle Question")]
public class PuzzleQuestions : ScriptableObject
{
    public string question;
    [TextArea]
    public string[] answers;
    public int correctAnswerIndex;

}
