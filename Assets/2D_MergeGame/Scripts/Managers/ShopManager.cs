using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("Elements")]
    [SerializeField] private SkinButton skinButtonPrefab;
    [SerializeField] private Transform skinButtonsParent;
    [SerializeField] private GameObject purchaseButton;
    [SerializeField] private TextMeshProUGUI skinLabelText;
    [SerializeField] private TextMeshProUGUI skinPriceText;

    [Header("Data")]
    [SerializeField] private SkinDataSO[] skinDataSOs;
    private bool[] unlockedStates;

    [Header("Variables")]
    private int lastSelectedSkin;

    [Header("Scroll View")]
    [SerializeField] private ScrollRect skinScrollView;  // ScrollRect
    [SerializeField] private RectTransform content;      // Content

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI recipeIngredients;
    [SerializeField] private RectTransform rewardLimitPanel; // Panel for reward limit notification
    private Vector2 rewardLimitPanelOpenedPosition;
    private Vector2 rewardLimitPanelClosedPosition;

    [Header("Actions")]
    public static Action<SkinDataSO> onSkinSelected;

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

        unlockedStates = new bool[skinDataSOs.Length];
    }

    private void Start()
    {
        Initialize();
        LoadData();
        DailyRewardLimitPanelInitialize();

        if (skinButtonsParent.childCount > 0)
        {
            SkinButtonClickedCallback(lastSelectedSkin, false); // select the first product and center it
        }

    }

    void Update()
    {
        if (rewardLimitPanel == null)
        {
            Debug.LogError("RewardLimitPanel has been destroyed!");
        }
    }

    public void PurchaseButtonCallback()
    {
        CoinManager.instance.AddCoins(-skinDataSOs[lastSelectedSkin].GetPrice());

        //check if  we have enough coin


        // if thats case, unlock the skin
        unlockedStates[lastSelectedSkin] = true;

        SaveData();

        //calling the method to trigger the event and hide the purchase button.
        SkinButtonClickedCallback(lastSelectedSkin);
    }

    private void Initialize()
    {
        //Hide the purchase button
        purchaseButton.SetActive(false);

        for (int i = 0; i < skinDataSOs.Length; i++)
        {
            SkinButton skinButtonInstance = Instantiate(skinButtonPrefab, skinButtonsParent);

            //BURASI KODUN ESKÝ HALÝ
            //skinButtonInstance.Configure(skinDataSOs[i].GetIconSprite());

            skinButtonInstance.Configure(skinDataSOs[i].GetIconSprite(), skinDataSOs[i].GetFlagSprite());

            //if(i == 0)
            //{
            //    skinButtonInstance.Select();
            //}

            int j = i; //button index
            skinButtonInstance.GetButton().onClick.AddListener(() => SkinButtonClickedCallback(j));
        }
    }

    private void SkinButtonClickedCallback(int skinButtonIndex, bool shouldSaveLastSkin = true)
    {
        lastSelectedSkin = skinButtonIndex;

        // Seçilen butonu vurgulama
        for (int i = 0; i < skinButtonsParent.childCount; i++)
        {
            SkinButton currentSkinButton = skinButtonsParent.GetChild(i).GetComponent<SkinButton>();

            if (i == skinButtonIndex)
            {
                currentSkinButton.Select();
            }
            else
            {
                currentSkinButton.Unselect();
            }
        }

        // Smooth scroll kullanarak ortala
        StartCoroutine(SmoothScroll(skinButtonIndex));

        if (IsSkinUnlocked(skinButtonIndex))
        {
            onSkinSelected?.Invoke(skinDataSOs[skinButtonIndex]);

            if (shouldSaveLastSkin)
            {
                SaveLastSelectedSkin();
            }
        }

        ManagePurhcaseButtonVisibility(skinButtonIndex);
        UpdateSkinLabel(skinButtonIndex);

        // Recipe Ingredients'i güncelle
        UpdateRecipeIngredients(skinButtonIndex);
    }

    private void UpdateRecipeIngredients(int skinButtonIndex)
    {
        // ingredients listesini alýyoruz
        List<string> ingredients = skinDataSOs[skinButtonIndex].GetIngredients();

        // Listeyi string formatýna çevirip metin objesine yazdýrýyoruz
        if (ingredients != null && ingredients.Count > 0)
        {
            recipeIngredients.text = "";
            foreach (string ingredient in ingredients)
            {
                recipeIngredients.text += ingredient + "\n";  // Liste elemanlarýný alt alta yazdýrýyoruz
            }
        }
        else
        {
            recipeIngredients.text = "No ingredients available.";
        }
    }

    private IEnumerator SmoothScroll(int skinButtonIndex)
    {
        float totalItems = skinButtonsParent.childCount;
        float targetPosition = Mathf.Clamp01((float)skinButtonIndex / (totalItems - 1));
        float startPosition = skinScrollView.horizontalNormalizedPosition;
        float duration = 0.25f; // Kaydýrmanýn süresi
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            skinScrollView.horizontalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        // Hedef pozisyona tam olarak ulaþ
        skinScrollView.horizontalNormalizedPosition = targetPosition;
    }

    private void UpdateSkinLabel(int skinButtonIndex)
    {
        skinLabelText.text = skinDataSOs[skinButtonIndex].GetName();
    }

    private void ManagePurhcaseButtonVisibility(int skinButtonIndex)
    {
        bool canPurchase = CoinManager.instance.CanPurchase(skinDataSOs[skinButtonIndex].GetPrice());
        purchaseButton.GetComponent<Button>().interactable = canPurchase;

        purchaseButton.SetActive(!IsSkinUnlocked(skinButtonIndex));

        //update the price text
        skinPriceText.text = skinDataSOs[skinButtonIndex].GetPrice().ToString();
    }

    private bool IsSkinUnlocked(int skinButtonIndex)
    {
        return unlockedStates[skinButtonIndex];
    }

    private void DailyRewardLimitPanelInitialize()
    {
        rewardLimitPanel.gameObject.SetActive(false);

        rewardLimitPanelOpenedPosition = Vector2.zero;
        rewardLimitPanelClosedPosition = new Vector2(0, Screen.height);

        rewardLimitPanel.anchoredPosition = rewardLimitPanelClosedPosition;
    }

    public void ShowLimitReachedPanel()
    {
        rewardLimitPanel.gameObject.SetActive(true);

        LeanTween.cancel(rewardLimitPanel);
        LeanTween.move(rewardLimitPanel, rewardLimitPanelOpenedPosition, 0.5f).setEase(LeanTweenType.easeInOutSine);
    }

    public void CloseRewardLimitPanel()
    {
        LeanTween.cancel(rewardLimitPanel);
        LeanTween.move(rewardLimitPanel, rewardLimitPanelClosedPosition, 0.5f).setEase(LeanTweenType.easeInOutSine);

        LeanTween.delayedCall(0.5f, () => rewardLimitPanel.gameObject.SetActive(false));
    }


    private void LoadData()
    {
        for (int i = 0; i < unlockedStates.Length; i++)
        {
            int unlockedValue = 0;

            if (SaveSystem.Instance.ShopSkinKeyValues.Count > i)
            {
                unlockedValue = SaveSystem.Instance.ShopSkinKeyValues[i];
            }

            if (i == 0)
            {
                unlockedValue = 1;
            }

            if (unlockedValue == 1)
            {
                unlockedStates[i] = true;
            }
        }

        LoadLastSelectedSkin();
    }

    private void SaveData()
    {
        for (int i = 0; i < unlockedStates.Length; i++)
        {
            int unlockedValue = unlockedStates[i] ? 1 : 0;

            if (SaveSystem.Instance.ShopSkinKeyValues.Count <= i)
            {
                SaveSystem.Instance.ShopSkinKeyValues.Add(unlockedValue);
            }
            else
            {
                SaveSystem.Instance.ShopSkinKeyValues[i] = unlockedValue;
            }
        }
    }
    public void LoadLastSelectedSkin()
    {
        int lastSelectedSkinIndex = SaveSystem.Instance.ShopLastSelectedSkin;
        SkinButtonClickedCallback(lastSelectedSkinIndex, false);
        Debug.Log("Last selected skin loaded: " + lastSelectedSkinIndex);
    }

    private void SaveLastSelectedSkin()
    {
        SaveSystem.Instance.ShopLastSelectedSkin = lastSelectedSkin;

        Debug.Log("Last selected skin saved: " + lastSelectedSkin);
    }
}