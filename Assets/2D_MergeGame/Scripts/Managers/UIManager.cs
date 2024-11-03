using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject levelMapPanel;
    [SerializeField] private GameObject dropLine;

    [Header("Actions")]
    public static Action onMapOpened;

    private void Awake()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
        LevelMapManager.onLevelButtonClicked += LevelButtonCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
        LevelMapManager.onLevelButtonClicked -= LevelButtonCallback;
    }

    private void Start()
    {
        //SetMenu();
    }

    private void GameStateChangedCallback(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.Menu:
                SetMenu(); 
                break;
            
            case GameState.Game: 
                SetGame();
                break;

            case GameState.Gameover:
                SetGameOver();
                break;
        }
    }

    private void SetMenu()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mapPanel.SetActive(false);
    }

    private void SetGame()
    {
        gamePanel.SetActive(true);
        menuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        mapPanel.SetActive(false);
    }

    private void SetGameOver()
    {
        gameOverPanel.SetActive(true);
        menuPanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void LevelButtonCallback()
    {
        GameManager.instance.SetGameState();
        SetGame();
    }

    public void NextButtonCallback()
    {
        SceneManager.LoadScene(0);
    }

    public void SettingsPanelCallback()
    {
        if (GameManager.instance.IsGameState())
        {
            PauseGame();
        }
        settingsPanel.SetActive(true);
        // Disable the LineRenderer when settings panel is opened
        if (dropLine != null)
        {
            dropLine.SetActive(false);
        }
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        ResumeGame();
        // Re-enable the LineRenderer when settings panel is closed
        if (dropLine != null)
        {
            dropLine.SetActive(true);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0; // Pauses the game
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resumes the game
    }

    public bool IsSettingsPanelActive()
    {
        return settingsPanel.activeSelf; // Check if settings panel is active
    }

    public void ShopButtonCallback()
    {
        shopPanel.SetActive(true);
    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }

    public void OpenMap()
    {
        mapPanel.SetActive(true);

        onMapOpened?.Invoke();
    }

    public void CloseMap()
    {
        mapPanel.SetActive(false);
    }
}
