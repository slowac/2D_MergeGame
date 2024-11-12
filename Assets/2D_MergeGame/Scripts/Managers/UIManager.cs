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
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject levelMapPanel;
    [SerializeField] private GameObject dropLine;

    [Header("Settings")]
    private Vector2 settingsPanelOpenedPosition;
    private Vector2 settingsPanelClosedPosition;
    private Vector2 shopPanelOpenedPosition;
    private Vector2 shopPanelClosedPosition;

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
        SettingsPanelInitialize();
        ShopPanelInitialize();
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

    private void SettingsPanelInitialize()
    {
        settingsPanel.gameObject.SetActive(false);

        settingsPanelOpenedPosition = Vector2.zero;
        settingsPanelClosedPosition = new Vector2(-settingsPanel.rect.width, 0);

        settingsPanel.anchoredPosition = settingsPanelClosedPosition;
    }

    public void SettingsPanelCallback()
    {
        settingsPanel.gameObject.SetActive(true);

        LeanTween.cancel(settingsPanel);
        LeanTween.move(settingsPanel, settingsPanelOpenedPosition, 0.3f).setEase(LeanTweenType.easeInOutSine);

        if (GameManager.instance.IsGameState())
        {
            LeanTween.delayedCall(0.3f, PauseGame);
        }

        if (dropLine != null)
        {
            dropLine.SetActive(false);
        }
    }

    public void CloseSettingsPanel()
    {
        ResumeGame();

        LeanTween.cancel(settingsPanel);
        LeanTween.move(settingsPanel, settingsPanelClosedPosition, 0.3f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.delayedCall(0.3f, () => settingsPanel.gameObject.SetActive(false));

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
        return settingsPanel.gameObject.activeSelf; // Check if settings panel is active
    }

    private void ShopPanelInitialize()
    {
        shopPanel.gameObject.SetActive(false);

        shopPanelOpenedPosition = Vector2.zero;
        shopPanelClosedPosition = new Vector2(shopPanel.rect.width, 0);

        shopPanel.anchoredPosition = shopPanelClosedPosition;
    }

    public void ShopButtonCallback()
    {
        shopPanel.gameObject.SetActive(true);

        LeanTween.cancel(shopPanel);
        LeanTween.move(shopPanel, shopPanelOpenedPosition, 0.3f).setEase(LeanTweenType.easeInOutSine);
    }

    public void CloseShopPanel()
    {
        LeanTween.cancel(shopPanel);
        LeanTween.move(shopPanel, shopPanelClosedPosition, 0.3f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.delayedCall(0.3f, () => shopPanel.gameObject.SetActive(false));
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
