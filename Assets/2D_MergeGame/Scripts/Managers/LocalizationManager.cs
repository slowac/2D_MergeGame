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

    private void OnLanguageChanged(int index)
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
        //load locale setting from localization package
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
                PlayerPrefs.SetString(LanguageKey, localeCode);
                yield break;
            }
        }
    }

    private void LoadLanguagePreference()
    {
        //load registered language and set dropdown
        int savedIndex = PlayerPrefs.GetInt(LanguageIndexKey, 0);
        languageDropdown.value = savedIndex;
        string savedLocaleCode = PlayerPrefs.GetString(LanguageKey, "en");

        SetLanguage(savedLocaleCode);
    }
}
