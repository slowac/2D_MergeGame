using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("Data")]
    [SerializeField] private FruitType fruitType;
    private bool hasCollided;
    private bool canBeMerged;

    [Header("Actions")]
    public static Action<Fruit, Fruit> onCollisionWithFruit;

    [Header("Effects")]
    [SerializeField] private ParticleSystem mergeParticles;

    [Header("Color Data")]
    [SerializeField] private List<FruitColorDataSO> colorDataList; // Renk verilerini tutan liste
    private FruitColorDataSO currentColorData;  // Þu anda kullanýlacak renk verisi

    private void Start()
    {
        Invoke("AllowMerge", .25f);

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
            // Trail efekti baþlat
            SetTrailColor();
        }
    }

    private void AllowMerge()
    {
        canBeMerged = true;
    }

    private void SetTrailColor()
    {
        // FruitType'a göre doðru renk verisini alýyoruz
        currentColorData = colorDataList.Find(data => data.fruitType == fruitType);

        if (currentColorData != null)
        {
            // Gradient oluþturuluyor
            Gradient gradient = new Gradient();
            gradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(currentColorData.startColor, 0f), // Baþlangýç rengi
                new GradientColorKey(currentColorData.endColor, 1f)    // Bitiþ rengi
            };
            gradient.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f), // Baþlangýçta tamamen opak
                new GradientAlphaKey(1f, 1f)  // Bitiþte tamamen opak
            };

            // TrailRenderer'ýn colorGradient özelliðine bu gradient'i atýyoruz
            trailRenderer.colorGradient = gradient;
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        transform.position = targetPosition;
    }

    public void EnablePhysics()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ManageCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ManageCollision(collision);
    }

    private void ManageCollision(Collision2D collision)
    {
        hasCollided = true;

        if (!canBeMerged)
        {
            return;
        }

        if (collision.collider.TryGetComponent(out Fruit otherFruit))
        {
            if (otherFruit.GetFruitType() != fruitType)
            {
                return;
            }

            if (!otherFruit.CanBeMerged())
            {
                return;
            }

            onCollisionWithFruit?.Invoke(this, otherFruit);
        }

        if (trailRenderer != null)
        {
            Invoke("DisableTrail", 0.25f);
        }
    }

    private void DisableTrail()
    {
        trailRenderer.enabled = false;
    }

    public void Merge()
    {
        if(mergeParticles != null)
        {
            mergeParticles.transform.SetParent(null);
            mergeParticles.Play();
        }

        Destroy(gameObject);
    }

    public FruitType GetFruitType()
    {
        return fruitType;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    public bool HasCollided()
    {
        return hasCollided;
    }

    public bool CanBeMerged()
    {
        return canBeMerged;
    }
}
