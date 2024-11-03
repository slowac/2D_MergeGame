using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectionOutline;

    public void Configure(Sprite sprite)
    {
        iconImage.sprite = sprite;
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
