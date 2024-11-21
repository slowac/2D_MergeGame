using UnityEngine;

public class LineRendererFade : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float fadeStartPoint = 0.3f; // Þeffaflýðýn baþladýðý oran (0 ile 1 arasýnda)

    public void ApplyFade()
    {
        int pointCount = lineRenderer.positionCount;

        // Yeni bir gradient oluþtur
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[pointCount];

        // Renkleri sabit tut
        colorKeys[0].color = Color.white;
        colorKeys[0].time = 0f;
        colorKeys[1].color = Color.white;
        colorKeys[1].time = 1f;

        // Alfa deðerlerini hesapla
        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);

            // Þeffaflýðýn baþlangýç oranýný kullan
            float alpha = t < fadeStartPoint ? 1f : Mathf.Lerp(1f, 0f, (t - fadeStartPoint) / (1f - fadeStartPoint));
            alphaKeys[i] = new GradientAlphaKey(alpha, t);
        }

        // Gradient ayarlarýný uygula
        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;
    }
}
