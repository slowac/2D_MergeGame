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

    // Ürün týklandýðýnda bu fonksiyonu çaðýrýn
    public void OnProductClick()
    {
        if (centerOnClick != null)
        {
            // Týklanan ürünün RectTransform bileþenini alýyoruz
            centerOnClick.CenterToItem(GetComponent<RectTransform>());
        }
    }
}