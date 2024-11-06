using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SettingsManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject resetProgressPrompt;
    [SerializeField] private GameObject mainMenuPrompt;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private Slider pushMagnitudeSlider;
    [SerializeField] private Toggle sfxToggle;

    [Header("Actions")]
    public static Action<float> onPushMagnitudeChanged;
    public static Action<bool> onSFXValueChanged;

    [Header("Data")]
    private const string lastPushMagnitudeKey = "lastPushMagnitude";
    private const string sfxActiveKey = "sfxActive";
    private bool canSave;   

    private void OnEnable()
    {
        GameManager.onGameStateChanged += MainMenuButtonCheck;

        LoadData();        
    }

    private void OnDisable()
    {
        GameManager.onGameStateChanged -= MainMenuButtonCheck;
    }

    IEnumerator Start()
    {
        // initialize the push magnitude value for the merge push effect
        Initialize();
        
        yield return new WaitForSeconds(0.5f);

        canSave = true;
    }

    private void Initialize()
    {
        onPushMagnitudeChanged?.Invoke(pushMagnitudeSlider.value);
        onSFXValueChanged?.Invoke(sfxToggle.isOn);
    }

    public void ResetProgressButtonCallback()
    {
        resetProgressPrompt.SetActive(true);
    }

    public void ResetProgressYes()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    public void ResetProgressNo()
    {
        resetProgressPrompt.SetActive(false);
    }

    private void MainMenuButtonCheck(GameState gameState)
    {
        if (gameState == GameState.Game)
        {
            mainMenuButton.SetActive(true);
        }
        else
        {
            mainMenuButton.SetActive(false);
        }
    }

    public void MainMenuButtonCallback()
    {
        mainMenuPrompt.SetActive(true);
    }

    public void MainMenuYes()
    {
        SceneManager.LoadScene(0);
    }

    public void MainMenuNo()
    {
        mainMenuPrompt.SetActive(false);
    }

    public void SliderValueChangedCallback()
    {
        onPushMagnitudeChanged?.Invoke(pushMagnitudeSlider.value);
        SaveData();
    }

    public void ToggleCallback(bool sfxActive)
    {
        onSFXValueChanged?.Invoke(sfxActive);
        SaveData();
    }

    private void LoadData()
    {
        pushMagnitudeSlider.value = PlayerPrefs.GetFloat(lastPushMagnitudeKey, 0.25f);
        sfxToggle.isOn = PlayerPrefs.GetInt(sfxActiveKey, 1) == 1;
    }

    private void SaveData()
    {
        if(!canSave)
        {
            return;
        }

        PlayerPrefs.SetFloat(lastPushMagnitudeKey, pushMagnitudeSlider.value);
        
        int sfxValue = sfxToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt(sfxActiveKey, sfxValue);
    }
}
