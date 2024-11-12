using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [Header("Actions")]
    public static Action<FruitType, Vector2> onMergeProcessed;

    [Header("Settings")]
    Fruit lastSender;

    private void Awake()
    {
        Fruit.onCollisionWithFruit += CollisionBetweenFruitsCallback;
    }

    private void OnDestroy()
    {
        Fruit.onCollisionWithFruit -= CollisionBetweenFruitsCallback;
    }

    private void CollisionBetweenFruitsCallback(Fruit sender, Fruit otherFruit)
    {
        if (lastSender != null)
        {
            return;
        }

        lastSender = sender;

        ProcessMerge(sender, otherFruit);

        Debug.Log("Collision detected by " + sender.name);
    }

    private void ProcessMerge(Fruit sender, Fruit otherFruit)
    {
        FruitType mergeFruitType = sender.GetFruitType();
        mergeFruitType += 1;

        Vector2 fruitSpawnPos = (sender.transform.position + otherFruit.transform.position) / 2;

        sender.Merge();
        otherFruit.Merge();

        StartCoroutine(ResetLastSenderCoroutine());

        onMergeProcessed?.Invoke(mergeFruitType, fruitSpawnPos);

        if (mergeFruitType == FruitType.NoodlePlate + 1)
        {
            ParticleManager.Instance.PlayMergeFinalParticleEffect(fruitSpawnPos);
        }
    }

    IEnumerator ResetLastSenderCoroutine()
    {
        yield return new WaitForEndOfFrame();
        lastSender = null;
    }
}
