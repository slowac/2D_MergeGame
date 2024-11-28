using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [Header("Variables")]
    private int coins;

    [Header("Actions")]
    public static Action onCoinsUpdated;

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

        LoadData();
        UpdateCoinText();

        MergeManager.onMergeProcessed += MergeProcessedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;
    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 fruitSpawnPos)
    {
        int coinsToAdd = ((int)fruitType);
        //AddCoins(coinsToAdd);

        if (fruitType == FruitType.NoodlePlate + 1)
        {
            Debug.LogError("Plates are merged and gained" + coinsToAdd +  " coin!");
            AddCoins(coinsToAdd * 10);
        }
        else
        {
            AddCoins(coinsToAdd);
        }
    }

    public void AddCoins(int amout) 
    {
        coins += amout;
        coins = Mathf.Max(0, coins);

        SaveData();

        UpdateCoinText();
    }

    public bool CanPurchase(int price)
    {
        return coins >= price;
    }

    private void UpdateCoinText()
    {
        CoinText[] coinTexts = Resources.FindObjectsOfTypeAll(typeof(CoinText)) as CoinText[];

        for (int i = 0; i < coinTexts.Length; i++)
        {
            coinTexts[i].UpdateText(coins.ToString());
        }

        onCoinsUpdated?.Invoke();
    }
    
    private void LoadData()
    {
        coins = SaveSystem.Instance.Coins;
    }

    private void SaveData()
    {
        SaveSystem.Instance.Coins = coins;
    }
}
