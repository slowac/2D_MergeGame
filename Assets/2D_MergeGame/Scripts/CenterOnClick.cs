using UnityEngine;
using UnityEngine.UI;

public class CenterOnClick : MonoBehaviour
{
    public ScrollRect scrollRect; // ScrollRect bile�enini atay�n

    public void CenterToItem(RectTransform target)
    {
        // Content alan�n� al�yoruz
        RectTransform contentRect = scrollRect.content;

        // ScrollView ve i�erik boyutlar�n� al�yoruz
        float contentWidth = contentRect.rect.width;
        float scrollViewWidth = scrollRect.viewport.rect.width;

        // Hedef nesnenin ScrollView i�indeki x konumunu hesapl�yoruz
        float targetX = target.localPosition.x;

        // Hedef konumu ScrollView'in merkezine getirmek i�in normalle�tirilmi� x de�eri hesapl�yoruz
        float normalizedPositionX = (targetX - (scrollViewWidth / 2)) / (contentWidth - scrollViewWidth);

        // Normalle�tirilmi� de�eri s�n�rl�yoruz (0 ile 1 aras�nda kalmas�n� sa�l�yoruz)
        normalizedPositionX = Mathf.Clamp01(normalizedPositionX);

        // ScrollRect'in yatay kayd�rma konumunu ayarl�yoruz
        scrollRect.horizontalNormalizedPosition = normalizedPositionX;
    }
}