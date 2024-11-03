using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI levelIndexText;
    [SerializeField] private Button button;

    private void Start()
    {
        GetComponent<Image>().color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);
    }

    public void Configure(int levelIndex)
    {
        levelIndexText.text = levelIndex.ToString();
    }

    public void Enable()
    {
        button.interactable = true;
    }

    public void Lock()
    {
        button.interactable = false;
        GetComponent<Image>().color = Color.black;  // Buton rengini siyah yap
    }

    public Button GetButton()
    {
        return button;
    }
}
