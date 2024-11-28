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
    [SerializeField] private TextMeshProUGUI timerText; // Kalan süreyi gösterecek TextMeshPro referansý
    [SerializeField] private TextMeshProUGUI targetScoreText; // Hedef puaný gösterecek TextMeshPro referansý
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

        // Seviye prefabýný yükle
        Instantiate(levelDatas[buttonIndex].GetLevel(), transform);

        SaveSystem.Instance.LastPlayedLevelIndex = levelDatas[buttonIndex].LevelIndex;

        // Zamanlý seviye kontrolü
        if (levelDatas[buttonIndex].IsTimed())
        {
            StartTimedLevel(levelDatas[buttonIndex]);
        }
        else
        {
            // Zamanlý deðilse, timer text'i gizle
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

        // Oyun baþlatma aksiyonu
        onLevelButtonClicked?.Invoke();
    }

    private void StartTimedLevel(LevelDataSO levelData)
    {
        float timeLimit = levelData.GetTimeLimit();
        int targetScore = levelData.GetTargetScore();

        Debug.Log($"Zamana Karþý Seviye Baþladý! Süre: {timeLimit}, Hedef Puan: {targetScore}");

        // Zamanlý seviyelerde hedef puanýný göstermek
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

        // Timer baþlat
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
            // Kalan süreyi ekranda güncelle
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(remainingTime).ToString();
            }

            yield return new WaitForSeconds(1f);
            remainingTime--;

            // Hedefe ulaþýldý mý kontrol et
            if (ScoreManager.instance.GetCurrentScore() >= targetScore)
            {
                Debug.Log("Hedefe ulaþýldý! Ödül veriliyor.");
                StartCoroutine(fruitManager.ExplodeFruits(GameState.LevelCompleted));
                CoinManager.instance.AddCoins(levelData.GetRewardPrice()); // Ödül ekle

                // Kazanýlan coini göster
                if (rewardCoinText != null)
                {
                    rewardCoinText.gameObject.SetActive(true); // Metin kutusunu aktif et
                    rewardCoinText.text = $"You Won: {levelData.GetRewardPrice()} Coins"; // Ödül coinini yazdýr
                }

                // Timer kapatýlýr
                if (timerText != null)
                {
                    timerText.gameObject.SetActive(false);
                }

                // Hedefe ulaþýldýðýnda hedef puan metnini gizle
                if (targetScoreText != null)
                {
                    targetScoreText.gameObject.SetActive(false);
                }

                yield break; // Timer sonlanýr
            }
        }

        Debug.Log("Zaman doldu! Hedefe ulaþýlamadý.");

        // Timer kapatýlýr
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // Zaman dolduðunda patlatma iþlemi baþlatýlýr
        if (fruitManager != null)
        {
            StartCoroutine(fruitManager.ExplodeFruits(GameState.Gameover));
        }
        else
        {
            Debug.LogWarning("FruitManager referansý atanmadý!");
        }

        // Hedef puan metnini gizle
        if (targetScoreText != null)
        {
            targetScoreText.gameObject.SetActive(false);
        }

        // Eðer ödül coin metnini göstermek istiyorsanýz:
        if (rewardCoinText != null)
        {
            rewardCoinText.gameObject.SetActive(false); // Ödül metnini gizle
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