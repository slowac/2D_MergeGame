using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject tutorialPanel;
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;

    [Header("UI Elements")]
    public Button nextButton;
    public Button continueButton;  // Continue button
    public Button closeButton;
    public Toggle dontShowAgainToggle;

    [Header("Animation Settings")]
    public Vector3 panelHiddenPosition;  // Sahnenin alt�nda ba�lang�� pozisyonu
    public Vector3 panelVisiblePosition; // G�r�nen pozisyon (ekranda ortada)

    private const string TutorialKey = "ShowTutorial";

    private void Start()
    {
        // Tutorial panelini ba�lang��ta sahnenin alt�na konumland�r�r
        tutorialPanel.GetComponent<RectTransform>().anchoredPosition = panelHiddenPosition;

        // E�er tutorial daha �nce kapat�lmad�ysa, 2 saniye sonra g�ster
        if (PlayerPrefs.GetInt(TutorialKey, 1) == 1)
        {
            Invoke("ShowTutorial", 0.2f); // 0.2 saniye gecikmeyle paneli g�ster
        }
        else
        {
            tutorialPanel.SetActive(false);
        }

        // Button eventlerini atama
        nextButton.onClick.AddListener(ShowPage2);
        continueButton.onClick.AddListener(ShowPage3);  // Add listener for the Continue button
        closeButton.onClick.AddListener(CloseTutorial);
    }

    private void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        page1.SetActive(true);
        page2.SetActive(false);
        page3.SetActive(false);  // Ensure page 3 is hidden initially

        // Paneli alt k�s�mdan yukar� kayd�rarak g�r�n�r yap
        LeanTween.cancel(tutorialPanel);
        LeanTween.move(tutorialPanel.GetComponent<RectTransform>(), panelVisiblePosition, 0.6f)
                 .setEase(LeanTweenType.easeInOutSine);
    }

    private void ShowPage2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    private void ShowPage3()
    {
        page2.SetActive(false);
        page3.SetActive(true);  // Show page 3 when Continue button is pressed
    }

    private void CloseTutorial()
    {
        // Paneli kapat�rken a�a�� do�ru kayd�rarak sahneden ��kar
        LeanTween.move(tutorialPanel.GetComponent<RectTransform>(), panelHiddenPosition, 0.3f)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setOnComplete(() => tutorialPanel.SetActive(false));

        // Toggle durumu kaydedilir
        if (dontShowAgainToggle.isOn)
        {
            PlayerPrefs.SetInt(TutorialKey, 0);
        }
        else
        {
            PlayerPrefs.SetInt(TutorialKey, 1);
        }

        PlayerPrefs.Save();
    }
}
