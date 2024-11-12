using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitColorData", menuName = "Scriptable Objects/FruitColorData", order = 1)]
public class FruitColorDataSO : ScriptableObject
{
    [Header("Type")]
    public FruitType fruitType;

    [Header("Fruit Colors")]
    public Color startColor;
    public Color endColor;
}
