using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    // 2 adet background image referansý
    public Image backgroundImage1; // Oyun baþladýðýnda aktif olacak olan background
    public Image backgroundImage2; // Diðer durumlar için background

    // Canvas'ý ve background RectTransform'larýný ayarlýyoruz
    public Canvas canvas;

    void Update()
    {
        ScaleBackground(backgroundImage1);
        ScaleBackground(backgroundImage2);
    }

    // Arka plan görselini canvas'a göre ölçeklendirir
    void ScaleBackground(Image backgroundImage)
    {
        RectTransform backgroundRectTransform = backgroundImage.GetComponent<RectTransform>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // Canvas'ýn çözünürlüðünü alýyoruz
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        // Background image'ýnýn çözünürlüðünü alýyoruz
        float backgroundWidth = backgroundRectTransform.rect.width;
        float backgroundHeight = backgroundRectTransform.rect.height;

        // Canvas ile background arasýndaki oraný hesaplýyoruz
        float widthRatio = canvasWidth / backgroundWidth;
        float heightRatio = canvasHeight / backgroundHeight;

        // En küçük oraný kullanarak background'ý ölçeklendiriyoruz
        // Bu, hem yatay hem de dikeyde uygun bir boyut saðlar ve görüntünün orantýsýný bozulmadan ayarlar
        float scaleFactor = Mathf.Min(widthRatio, heightRatio);

        // Yeni ölçeði ayarlýyoruz
        backgroundRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
}
