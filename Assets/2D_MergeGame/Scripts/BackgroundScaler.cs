using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    // 2 adet background image referans�
    public Image backgroundImage1; // Oyun ba�lad���nda aktif olacak olan background
    public Image backgroundImage2; // Di�er durumlar i�in background

    // Canvas'� ve background RectTransform'lar�n� ayarl�yoruz
    public Canvas canvas;

    void Update()
    {
        ScaleBackground(backgroundImage1);
        ScaleBackground(backgroundImage2);
    }

    // Arka plan g�rselini canvas'a g�re �l�eklendirir
    void ScaleBackground(Image backgroundImage)
    {
        RectTransform backgroundRectTransform = backgroundImage.GetComponent<RectTransform>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // Canvas'�n ��z�n�rl���n� al�yoruz
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        // Background image'�n�n ��z�n�rl���n� al�yoruz
        float backgroundWidth = backgroundRectTransform.rect.width;
        float backgroundHeight = backgroundRectTransform.rect.height;

        // Canvas ile background aras�ndaki oran� hesapl�yoruz
        float widthRatio = canvasWidth / backgroundWidth;
        float heightRatio = canvasHeight / backgroundHeight;

        // En k���k oran� kullanarak background'� �l�eklendiriyoruz
        // Bu, hem yatay hem de dikeyde uygun bir boyut sa�lar ve g�r�nt�n�n orant�s�n� bozulmadan ayarlar
        float scaleFactor = Mathf.Min(widthRatio, heightRatio);

        // Yeni �l�e�i ayarl�yoruz
        backgroundRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
}
