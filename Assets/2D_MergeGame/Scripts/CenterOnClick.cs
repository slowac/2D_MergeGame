using UnityEngine;
using UnityEngine.UI;

public class CenterOnClick : MonoBehaviour
{
    public ScrollRect scrollRect; // ScrollRect bileþenini atayýn

    public void CenterToItem(RectTransform target)
    {
        // Content alanýný alýyoruz
        RectTransform contentRect = scrollRect.content;

        // ScrollView ve içerik boyutlarýný alýyoruz
        float contentWidth = contentRect.rect.width;
        float scrollViewWidth = scrollRect.viewport.rect.width;

        // Hedef nesnenin ScrollView içindeki x konumunu hesaplýyoruz
        float targetX = target.localPosition.x;

        // Hedef konumu ScrollView'in merkezine getirmek için normalleþtirilmiþ x deðeri hesaplýyoruz
        float normalizedPositionX = (targetX - (scrollViewWidth / 2)) / (contentWidth - scrollViewWidth);

        // Normalleþtirilmiþ deðeri sýnýrlýyoruz (0 ile 1 arasýnda kalmasýný saðlýyoruz)
        normalizedPositionX = Mathf.Clamp01(normalizedPositionX);

        // ScrollRect'in yatay kaydýrma konumunu ayarlýyoruz
        scrollRect.horizontalNormalizedPosition = normalizedPositionX;
    }
}