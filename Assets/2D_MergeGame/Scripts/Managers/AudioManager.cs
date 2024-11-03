using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Elements")]
    [SerializeField] private AudioSource mergeSource;
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource boilingSource;

    [Header("SFX")]
    [SerializeField] private AudioClip[] mergeClips;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip boilingSoundClip;

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

        MergeManager.onMergeProcessed += MergeProcessedCallback;
        SettingsManager.onSFXValueChanged += SFXValueChangedCallback;
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;
        SettingsManager.onSFXValueChanged -= SFXValueChangedCallback;
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 mergePos)
    {
        PlayMergeSound();
    }

    public void PlayMergeSound()
    {
        mergeSource.pitch = Random.Range(0.9f, 1.1f);
        mergeSource.clip = mergeClips[Random.Range(0, mergeClips.Length)];
        mergeSource.Play();
    }

    private void SFXValueChangedCallback(bool sfxActive)
    {
        mergeSource.mute = !sfxActive;
        backgroundMusicSource.mute = !sfxActive;
        boilingSource.mute = !sfxActive;
    }

    private void GameStateChangedCallback(GameState newState)
    {
        if (newState == GameState.Game)
        {
            PlayBackgroundMusic();
            PlayBoilingSound();
        }
        else
        {
            backgroundMusicSource.Stop();
            boilingSource.Stop();
        }
    }

    private void PlayBackgroundMusic()
    {
        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.loop = true;
        backgroundMusicSource.Play();
    }

    private void PlayBoilingSound()
    {
        boilingSource.clip = boilingSoundClip;
        boilingSource.loop = true;
        boilingSource.Play();
    }
}
