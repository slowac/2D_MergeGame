using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject bubbleParticle;

    private void OnEnable()
    {
        GameManager.onGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Game)
        {
            bubbleParticle.SetActive(true);
        }
        else
        {
            bubbleParticle.SetActive(false);
        }
    }
}
