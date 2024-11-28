using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Localization.Settings;

public class LevelMapManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private RectTransform mapContent;
    [SerializeField] private RectTransform[] levelButtonParents;
    [SerializeField] private LevelButton levelButtonPrefab;

    [Header("Data")]
    [SerializeField] private LevelDataSO[] levelDatas;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText; // Kalan s�reyi g�sterecek TextMeshPro referans�
    [SerializeField] private TextMeshProUGUI targetScoreText; // Hedef puan� g�sterecek TextMeshPro referans�
    [SerializeField] private TextMeshProUGUI rewardCoinText;

    [Header("Fruit Manager")]
    [SerializeField] private FruitManager fruitManager;

    [Header("Actions")]
    public static Action onLevelButtonClicked;

    private void Awake()
    {
        UIManager.onMapOpened += UpdateLevelButtonsInteractability;
    }

    private void OnDestroy()
    {
        UIManager.onMapOpened -= UpdateLevelButtonsInteractability;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Setup the scroll view - scroll view - map
        mapContent.anchoredPosition = Vector2.up * 1920 * (mapContent.childCount - 1);

        CreateLevelButtons();
        UpdateLevelButtonsInteractability();
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < levelDatas.Length; i++)
        {
            CreateLevelButton(i, levelButtonParents[i]);
        }
    }

    private void CreateLevelButton(int buttonIndex, Transform levelButtonParent)
    {
        LevelButton levelButton = Instantiate(levelButtonPrefab, levelButtonParent);
        levelButton.Configure(buttonIndex + 1);

        levelButton.GetButton().onClick.AddListener(() => LevelButtonClicked(buttonIndex));
    }

    private void LevelButtonClicked(int buttonIndex)
    {
        // Temizlik yap
        while (transform.childCount > 0)
        {
            Transform t = transform.GetChild(0);
            t.SetParent(null);
            Destroy(t.gameObject);
        }

        // Seviye prefab�n� y�kle
        Instantiate(levelDatas[buttonIndex].GetLevel(), transform);

        SaveSystem.Instance.LastPlayedLevelIndex = levelDatas[buttonIndex].LevelIndex;

        // Zamanl� seviye kontrol�
        if (levelDatas[buttonIndex].IsTimed())
        {
            StartTimedLevel(levelDatas[buttonIndex]);
        }
        else
        {
            // Zamanl� de�ilse, timer text'i gizle
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false);
            }

            // Hedef puan text'i gizlensin
            if (targetScoreText != null)
            {
                targetScoreText.gameObject.SetActive(false);
            }
        }

        // Oyun ba�latma aksiyonu
        onLevelButtonClicked?.Invoke();
    }

    private void StartTimedLevel(LevelDataSO levelData)
    {
        float timeLimit = levelData.GetTimeLimit();
        int targetScore = levelData.GetTargetScore();

        Debug.Log($"Zamana Kar�� Seviye Ba�lad�! S�re: {timeLimit}, Hedef Puan: {targetScore}");

        // Zamanl� seviyelerde hedef puan�n� g�stermek
        if (targetScoreText != null)
        {
            targetScoreText.gameObject.SetActive(true); // Text'i aktif et

            // Check the current language and update target score text accordingly
            string language = LocalizationSettings.SelectedLocale.Identifier.Code;
            if (language == "tr-TR")
            {
                targetScoreText.text = $"Hedef Skor: {targetScore}";  // Turkish
            }
            else
            {
                targetScoreText.text = $"Target Score: {targetScore}";  // English
            }
        }

        // Timer ba�lat
        StartCoroutine(LevelTimer(timeLimit, targetScore, levelData));
    }

    private IEnumerator LevelTimer(float timeLimit, int targetScore, LevelDataSO levelData)
    {
        float remainingTime = timeLimit;

        // UI Timer aktif ediliyor
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        while (remainingTime > 0)
        {
            // Kalan s�reyi ekranda g�ncelle
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(remainingTime).ToString();
            }

            yield return new WaitForSeconds(1f);
            remainingTime--;

            // Hedefe ula��ld� m� kontrol et
            if (ScoreManager.instance.GetCurrentScore() >= targetScore)
            {
                Debug.Log("Hedefe ula��ld�! �d�l veriliyor.");
                StartCoroutine(fruitManager.ExplodeFruits(GameState.LevelCompleted));
                CoinManager.instance.AddCoins(levelData.GetRewardPrice()); // �d�l ekle

                // Kazan�lan coini g�ster
                if (rewardCoinText != null)
                {
                    rewardCoinText.gameObject.SetActive(true); // Metin kutusunu aktif et
                    rewardCoinText.text = $"You Won: {levelData.GetRewardPrice()} Coins"; // �d�l coinini yazd�r
                }

                // Timer kapat�l�r
                if (timerText != null)
                {
                    timerText.gameObject.SetActive(false);
                }

                // Hedefe ula��ld���nda hedef puan metnini gizle
                if (targetScoreText != null)
                {
                    targetScoreText.gameObject.SetActive(false);
                }

                yield break; // Timer sonlan�r
            }
        }

        Debug.Log("Zaman doldu! Hedefe ula��lamad�.");

        // Timer kapat�l�r
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // Zaman doldu�unda patlatma i�lemi ba�lat�l�r
        if (fruitManager != null)
        {
            StartCoroutine(fruitManager.ExplodeFruits(GameState.Gameover));
        }
        else
        {
            Debug.LogWarning("FruitManager referans� atanmad�!");
        }

        // Hedef puan metnini gizle
        if (targetScoreText != null)
        {
            targetScoreText.gameObject.SetActive(false);
        }

        // E�er �d�l coin metnini g�stermek istiyorsan�z:
        if (rewardCoinText != null)
        {
            rewardCoinText.gameObject.SetActive(false); // �d�l metnini gizle
        }
    }


    private void UpdateLevelButtonsInteractability()
    {
        int bestScore = ScoreManager.instance.GetBestScore();

        for (int i = 0; i < levelDatas.Length; i++)
        {
            if ((SaveSystem.Instance.LastPlayedLevelIndex == i - 1 || SaveSystem.Instance.UnlockedLevels.Contains(i) || i == 0) && levelDatas[i].GetRequiredHighScore() <= bestScore)
            {
                levelButtonParents[i].GetChild(0).GetComponent<LevelButton>().Enable();

                Debug.Log("enable");

                if(SaveSystem.Instance.UnlockedLevels.Contains(i) == false)
                {
                    SaveSystem.Instance.UnlockedLevels.Add(i);
                }
            }
        }
    }
}