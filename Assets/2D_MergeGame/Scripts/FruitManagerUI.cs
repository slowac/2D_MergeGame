using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(FruitManager))]
public class FruitManagerUI : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image nextFruitImage;
    //[SerializeField] private TextMeshProUGUI nextFruitText;
    private FruitManager fruitManager;

    private void Awake()
    {
        FruitManager.onNextFruitIndexSet += UpdateNextFruitImage;
    }

    private void OnDestroy()
    {
        FruitManager.onNextFruitIndexSet -= UpdateNextFruitImage;
    }

    private void Start()
    {
        //fruitManager = GetComponent<FruitManager>();
    }

    private void UpdateNextFruitImage()
    {
        //nextFruitText.text = fruitManager.GetNextFruitName();

        if(fruitManager == null)
        {
            fruitManager = GetComponent<FruitManager>();
        }

        nextFruitImage.sprite = fruitManager.GetNextFruitSprite();
    }
}
