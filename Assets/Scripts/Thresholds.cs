using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Threshold", menuName = "Thresholds")]

public class Thresholds : ScriptableObject
{
    
    [Header("Time Thresholds (in seconds)")]
    public float firstClass = 45f;
    public float upperSecond = 85f;
    public float lowerSecond = 125f;
    public float thirdClass = 160f;

    public string CalculateGrade(float time)
    {
        if (time <= firstClass)
        {
            return "1st Class";
        } 
        else if (time <= upperSecond)
        {
            return "2:1 (Upper Second)";
        } 
        else if (time <= lowerSecond)
        {
            return "2:2 (Lower Second)";
        } 
        else if (time <= thirdClass)
        {
            return "3rd Class";
        }
        
        return "nill";
    }

}
