using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "Scriptable Objects/Level Data", order = 1)]
public class LevelDataSO : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private int requiredHighScore;

    [Header("Time Settings")]
    [SerializeField] private bool isTimed; // Determines whether or not to play against the clock.
    [SerializeField] private float timeLimit; // If it is against time, it indicates duration.
    [SerializeField] private int targetScore; // Target score against time
    [SerializeField] private int rewardPrice;

    public int LevelIndex;

    public GameObject GetLevel()
    {
        return levelPrefab;
    }

    public int GetRequiredHighScore()
    {
        return requiredHighScore;
    }

    public bool IsTimed()
    {
        return isTimed;
    }

    public float GetTimeLimit()
    {
        return timeLimit;
    }

    public int GetTargetScore()
    {
        return targetScore;
    }

    public int GetRewardPrice()
    {
        return rewardPrice;
    }
}