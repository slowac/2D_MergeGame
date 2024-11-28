using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    private const string LanguageKey = "SelectedLanguage";
    private const string LanguageIndexKey = "SelectedLanguageIndex";

    void Start()
    {
        // load dropdown settings and set selected language
        LoadLanguagePreference();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    public void OnLanguageChanged(int index)
    {
        // update selected language and save
        if (index == 0)
        {
            SetLanguage("en");
        }
        else if (index == 1)
        {
            SetLanguage("tr-TR");
        }
        PlayerPrefs.SetInt(LanguageIndexKey, index);
    }

    private void SetLanguage(string localeCode)
    {
        // Set the language asynchronously
        StartCoroutine(SetLocale(localeCode));
    }

    private IEnumerator SetLocale(string localeCode)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == localeCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                // Wait until the locale is applied properly
                yield return null;
                PlayerPrefs.SetString(LanguageKey, localeCode);
                yield break;
            }
        }
    }

    private void LoadLanguagePreference()
    {
        // Load the saved language preference
        int savedIndex = PlayerPrefs.GetInt(LanguageIndexKey, 0);
        string savedLocaleCode = PlayerPrefs.GetString(LanguageKey, "en");

        // Set the dropdown value and the locale
        languageDropdown.value = savedIndex;
        SetLanguage(savedLocaleCode);
    }
}
