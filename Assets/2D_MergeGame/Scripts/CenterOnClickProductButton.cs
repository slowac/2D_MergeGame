using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnClickProductButton : MonoBehaviour
{
    private CenterOnClick centerOnClick;

    private void Start()
    {
        // CenterOnClick script'ini buluyoruz
        centerOnClick = FindObjectOfType<CenterOnClick>();
    }

    // �r�n t�kland���nda bu fonksiyonu �a��r�n
    public void OnProductClick()
    {
        if (centerOnClick != null)
        {
            // T�klanan �r�n�n RectTransform bile�enini al�yoruz
            centerOnClick.CenterToItem(GetComponent<RectTransform>());
        }
    }
}