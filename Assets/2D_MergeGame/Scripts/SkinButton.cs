using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image flagImage;
    [SerializeField] private GameObject selectionOutline;

    public void Configure(Sprite iconSprite, Sprite flagSprite)
    {
        iconImage.sprite = iconSprite;

        if (flagImage != null)
        {
            flagImage.sprite = flagSprite;
            flagImage.gameObject.SetActive(flagSprite != null); // If flagSprite does not exist, hide it
        }
    }

    public Button GetButton()
    {
        return button;
    }

    public void Select()
    {
        selectionOutline.SetActive(true);
    }

    public void Unselect()
    {
        selectionOutline.SetActive(false);
    }
}
