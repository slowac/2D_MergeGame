using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private RectTransform settingsPanel;

    [Header("Settings")]
    private Vector2 openedPosition;
    private Vector2 closedPosition;

    private void Start()
    {
        openedPosition = Vector2.zero;
        closedPosition = new Vector2(settingsPanel.rect.width, 0);

        settingsPanel.anchoredPosition = closedPosition;
    }

    public void Open()
    {
        settingsPanel.gameObject.SetActive(true);
        LeanTween.cancel(settingsPanel);
        LeanTween.move(settingsPanel, openedPosition, 0.3f).setEase(LeanTweenType.easeInOutSine);
    }

    public void Close()
    {
        LeanTween.cancel(settingsPanel);
        LeanTween.move(settingsPanel, closedPosition, 0.3f).setEase(LeanTweenType.easeInOutSine);
        settingsPanel.gameObject.SetActive(false);

    }
}
