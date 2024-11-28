using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public ParticleManager particleManager;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI gameScoreText;
    [SerializeField] private TextMeshProUGUI menuBestScoreText;

    [Header("Settings")]
    [SerializeField] private float scoreMultiplier;
    private int score;
    private int bestScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        MergeManager.onMergeProcessed += MergeProcessedCallback;

        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;

        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void Start()
    {
        LoadData();

        UpdateScoreText();
        UpdateBestScoreText();
    }

    private void Update()
    {

    }

    public int GetBestScore()
    {
        return bestScore;
    }

    private void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Gameover:
                CalculateBestScore();
                break;
        }
    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 unused_vector2)
    {
        int scoreToAdd = (int)fruitType;
        //AddScore((int)(scoreToAdd * scoreMultiplier));

        if (fruitType == FruitType.NoodlePlate + 1)
        {
            Debug.LogError("Plates are merged!");
            AddScore(scoreToAdd * 10 * (int)scoreMultiplier);
        }
        else
        {
            AddScore(scoreToAdd * (int)scoreMultiplier);
        }
    }


    // new addscore fanc.
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }


    private void UpdateScoreText()
    {
        gameScoreText.text = score.ToString();
    }

    private void UpdateBestScoreText()
    {
        menuBestScoreText.text = bestScore.ToString();
    }

    private void CalculateBestScore()
    {
        if (score > bestScore)
        {
            bestScore = score;
            SaveData();
        }
    }

    public int GetCurrentScore()
    {
        return score;
    }


    private void LoadData()
    {
        bestScore = SaveSystem.Instance.BestScore;
    }

    private void SaveData()
    {
        SaveSystem.Instance.BestScore = bestScore;
    }
}
