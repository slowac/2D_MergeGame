using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("Settings")]
    private GameState gameState;

    [Header("Actions")]
    public static Action<GameState> onGameStateChanged;

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

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        onGameStateChanged += SetGameoverState;
        onGameStateChanged += SetLevelCompletedState;
        SetMenu();
    }

    private void OnDestroy()
    {
        onGameStateChanged -= SetGameoverState;

    }

    private void SetMenu()
    {
        SetGameState(GameState.Menu);
    }

    private void SetGame()
    {
        SetGameState(GameState.Game);
    }

    private void SetGameOver()
    {
        StartCoroutine(HandleGameOver());
    }

    private void SetLevelCompleted()
    {
        StartCoroutine(HandleLevelCompleted());
    }

    private IEnumerator HandleGameOver()
    {
        yield return StartCoroutine(FruitManager.instance.ExplodeFruits(GameState.Gameover)); // explode fruits
        SetGameState(GameState.Gameover);
    }

    private IEnumerator HandleLevelCompleted()
    {
        yield return StartCoroutine(FruitManager.instance.ExplodeFruits(GameState.LevelCompleted)); // explode fruits
        SetGameState(GameState.LevelCompleted);
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
        onGameStateChanged?.Invoke(gameState);
    }

    public void SetGameoverState(GameState gameState)
    {
        if (gameState == GameState.Gameover)
        {
            AdsManager.instance.ShowInterstitialAd();
        }
    }

    public void SetLevelCompletedState(GameState gameState)
    {
        if (gameState == GameState.LevelCompleted)
        {
            AdsManager.instance.ShowInterstitialAd();
        }
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetGameState()
    {
        SetGame();
    }

    public bool IsGameState()
    {
        return gameState == GameState.Game;
    }

    public void SetGameOverState()
    {
        SetGameOver();
    }

    public void SetLevelCompletedState()
    {
        SetLevelCompleted();
    }
}