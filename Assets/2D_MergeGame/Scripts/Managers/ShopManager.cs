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
    [SerializeField] private RectTransform content;      // Content

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
            SkinButtonClickedCallback(lastSelectedSkin, false); // select the first product and center it
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

            //BURASI KODUN ESK� HAL�
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

        // Se�ilen butonu vurgulama
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
    }

    private IEnumerator SmoothScroll(int skinButtonIndex)
    {
        float totalItems = skinButtonsParent.childCount;
        float targetPosition = Mathf.Clamp01((float)skinButtonIndex / (totalItems - 1));
        float startPosition = skinScrollView.horizontalNormalizedPosition;
        float duration = 0.25f; // Kayd�rman�n s�resi
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            skinScrollView.horizontalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        // Hedef pozisyona tam olarak ula�
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