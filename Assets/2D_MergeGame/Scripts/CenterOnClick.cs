using UnityEngine;
using UnityEngine.UI;

public class CenterOnClick : MonoBehaviour
{
    public ScrollRect scrollRect; // ScrollRect bileşenini atayın

    public void CenterToItem(RectTransform target)
    {
        // Content alanını alıyoruz
        RectTransform contentRect = scrollRect.content;

        // ScrollView ve içerik boyutlarını alıyoruz
        float contentWidth = contentRect.rect.width;
        float scrollViewWidth = scrollRect.viewport.rect.width;

        // Hedef nesnenin ScrollView içindeki x konumunu hesaplıyoruz
        float targetX = target.localPosition.x;

        // Hedef konumu ScrollView'in merkezine getirmek için normalleştirilmiş x değeri hesaplıyoruz
        float normalizedPositionX = (targetX - (scrollViewWidth / 2)) / (contentWidth - scrollViewWidth);

        // Normalleştirilmiş değeri sınırlıyoruz (0 ile 1 arasında kalmasını sağlıyoruz)
        normalizedPositionX = Mathf.Clamp01(normalizedPositionX);

        // ScrollRect'in yatay kaydırma konumunu ayarlıyoruz
        scrollRect.horizontalNormalizedPosition = normalizedPositionX;
    }
}