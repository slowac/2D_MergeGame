using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject resetProgressPrompt;
    [SerializeField] private Slider pushMagnitudeSlider;
    [SerializeField] private Toggle sfxToggle;

    [Header("Actions")]
    public static Action<float> onPushMagnitudeChanged;
    public static Action<bool> onSFXValueChanged;

    [Header("Data")]
    private const string lastPushMagnitudeKey = "lastPushMagnitude";
    private const string sfxActiveKey = "sfxActive";
    private bool canSave;   

    private void Awake()
    {
        LoadData();        
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

    public void MainMenuYes()
    {
        SceneManager.LoadScene(0);
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
        pushMagnitudeSlider.value = PlayerPrefs.GetFloat(lastPushMagnitudeKey);
        sfxToggle.isOn = PlayerPrefs.GetInt(sfxActiveKey) == 1;
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
