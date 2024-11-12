using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private SkinButton skinButtonPrefab;
    [SerializeField] private Transform skinButtonsParent;
    [SerializeField] private GameObject purchaseButton;
    [SerializeField] private TextMeshProUGUI skinLabelText;
    [SerializeField] private TextMeshProUGUI skinPriceText;

    [Header("Data")]
    [SerializeField] private SkinDataSO[] skinDataSOs;
    private bool[] unlockedStates;
    private const string skinButtonKey = "SkinButton_";
    private const string lastSelectedSkinKey = "LastSelectedSkin";

    [Header("Variables")]
    private int lastSelectedSkin;

    [Header("Scroll View")]
    [SerializeField] private ScrollRect skinScrollView;  // ScrollRect
    [SerializeField] private RectTransform content;      // Content kýsmý

    [Header("Actions")]
    public static Action<SkinDataSO> onSkinSelected;

    private void Awake()
    {
        unlockedStates = new bool[skinDataSOs.Length];
    }

    private void Start()
    {
        Initialize();
        LoadData();

        if (skinButtonsParent.childCount > 0)
        {
            SkinButtonClickedCallback(0, false); // Ýlk ürünü seçiyoruz ve ortalýyoruz
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

            skinButtonInstance.Configure(skinDataSOs[i].GetIconSprite());

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

        // Seçilen butonun RectTransform'ýný al
        RectTransform selectedButtonRectTransform = skinButtonsParent.GetChild(skinButtonIndex).GetComponent<RectTransform>();

        // Content'in geniþliðini ve butonun geniþliðini al
        float contentWidth = content.rect.width;
        float buttonWidth = selectedButtonRectTransform.rect.width;

        // Content'in pozisyonunu ve seçilen butonun pozisyonunu hesapla
        float buttonPosX = selectedButtonRectTransform.localPosition.x;

        // Seçilen butonun yatayda ortalanabilmesi için kaydýrma pozisyonunu hesapla
        float targetPosX = buttonPosX - (contentWidth / 2) + (buttonWidth / 2);

        // Horizontal scroll için kaydýrma oranýný hesapla
        float normalizedPosition = Mathf.Clamp01(targetPosX / (content.rect.width - contentWidth));

        // Coroutine ile kaydýrma iþlemini yap
        StartCoroutine(SmoothScroll(normalizedPosition));

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
    }

    private IEnumerator SmoothScroll(float targetNormalizedPosition)
    {
        float startPos = skinScrollView.horizontalNormalizedPosition;
        float elapsedTime = 0f;
        float duration = 0.25f; // Lerp süresi, isteðe göre ayarlayabilirsiniz

        while (elapsedTime < duration)
        {
            skinScrollView.horizontalNormalizedPosition = Mathf.Lerp(startPos, targetNormalizedPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Son pozisyonu ayarlama
        skinScrollView.horizontalNormalizedPosition = targetNormalizedPosition;
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

    private void LoadData()
    {
        for (int i = 0; i < unlockedStates.Length; i++)
        {
            int unlockedValue = PlayerPrefs.GetInt(skinButtonKey + i);

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
            PlayerPrefs.SetInt(skinButtonKey + i, unlockedValue);
        }
    }

    private void LoadLastSelectedSkin()
    {
        int lastSelectedSkinIndex = PlayerPrefs.GetInt(lastSelectedSkinKey);
        SkinButtonClickedCallback(lastSelectedSkinIndex, false);
    }

    private void SaveLastSelectedSkin()
    {
        PlayerPrefs.SetInt(lastSelectedSkinKey, lastSelectedSkin);
    }
}