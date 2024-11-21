using UnityEngine;

public class LineRendererFade : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float fadeStartPoint = 0.3f; // �effafl���n ba�lad��� oran (0 ile 1 aras�nda)

    public void ApplyFade()
    {
        int pointCount = lineRenderer.positionCount;

        // Yeni bir gradient olu�tur
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[pointCount];

        // Renkleri sabit tut
        colorKeys[0].color = Color.white;
        colorKeys[0].time = 0f;
        colorKeys[1].color = Color.white;
        colorKeys[1].time = 1f;

        // Alfa de�erlerini hesapla
        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);

            // �effafl���n ba�lang�� oran�n� kullan
            float alpha = t < fadeStartPoint ? 1f : Mathf.Lerp(1f, 0f, (t - fadeStartPoint) / (1f - fadeStartPoint));
            alphaKeys[i] = new GradientAlphaKey(alpha, t);
        }

        // Gradient ayarlar�n� uygula
        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;
    }
}
