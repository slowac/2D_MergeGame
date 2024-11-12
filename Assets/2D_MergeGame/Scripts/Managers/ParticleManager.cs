using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [Header("Elements")]
    [SerializeField] private GameObject bubbleParticle;
    public ParticleSystem mergeFinalParticleEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        bubbleParticle.SetActive(newState == GameState.Game);
    }

    public void PlayMergeFinalParticleEffect(Vector2 position)
    {
        mergeFinalParticleEffect.transform.position = position;
        mergeFinalParticleEffect.Play();
    }
}
