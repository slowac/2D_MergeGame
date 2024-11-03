using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DeadlineDisplay : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject deadline;
    [SerializeField] private Transform fruitsParent;

    private void Awake()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;

    }

    private void GameStateChangedCallback(GameState gameState)
    {
        if(gameState == GameState.Game)
        {
            StartCheckingForNearbyFruits();
        }
        else
        {
            StopCheckingForNearbyFruits();
        }
    }

    private void StartCheckingForNearbyFruits()
    {
        StartCoroutine(CheckForNearbyFruitsCoroutine());
    }

    private void StopCheckingForNearbyFruits()
    {
        HideDeadline();
        StopCoroutine(CheckForNearbyFruitsCoroutine());
    }

    IEnumerator CheckForNearbyFruitsCoroutine()
    {
        while (true)
        { 
            bool foundNearbyFruit = false;

            for(int i = 0; i < fruitsParent.childCount; i++)
            {
                if (!fruitsParent.GetChild(i).GetComponent<Fruit>().HasCollided())
                {
                    continue;
                }

                float distance = Mathf.Abs(fruitsParent.GetChild(i).position.y - deadline.transform.position.y);

                if (distance <= 1)
                {
                    ShowDeadline();
                    foundNearbyFruit = true;
                    break;
                }
            }

            if (!foundNearbyFruit)
            {
                HideDeadline();
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void ShowDeadline()
    {
        deadline.SetActive(true);
    }

    private void HideDeadline()
    {
        deadline.SetActive(false);
    }
}
