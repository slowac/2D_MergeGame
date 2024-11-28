using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinSpawnerManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject coinPrefab; // Coin Image prefab
    public Transform spawnableCoinContainer; // Container to hold spawned coins
    public Transform coinTarget; // Target container for coins (coinContainer)
    public int coinCount = 10; // Number of coins to spawn
    public float spawnRadius = 100f; // Radius around the container for spawning coins
    public float animationDuration = 1f; // Duration for the coins to reach the target

    [Header("Coin Animation")]
    public float spreadDuration = 0.3f; // Duration for the initial spread animation
    public Ease spreadEase = Ease.OutQuad; // Ease for the initial spread animation
    public Ease moveEase = Ease.InOutQuad; // Ease for the movement to target
    public float moveDelay = 0.1f; // Delay between each coin's movement

    private List<GameObject> spawnedCoins = new List<GameObject>();

    public void SpawnCoins()
    {
        // Clear previously spawned coins
        foreach (var coin in spawnedCoins)
        {
            if (coin != null)
                Destroy(coin);
        }
        spawnedCoins.Clear();

        // Spawn coins
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 randomPosition = spawnableCoinContainer.position + (Vector3)Random.insideUnitCircle * spawnRadius;
            GameObject spawnedCoin = Instantiate(coinPrefab, randomPosition, Quaternion.identity, spawnableCoinContainer);
            spawnedCoins.Add(spawnedCoin);

            // Animate initial spread
            spawnedCoin.transform.DOMove(randomPosition, spreadDuration).SetEase(spreadEase);
        }

        // Start collection animation after spread animation
        StartCoroutine(AnimateCoinsToTarget());
    }

    private IEnumerator AnimateCoinsToTarget()
    {
        yield return new WaitForSeconds(spreadDuration);

        for (int i = 0; i < spawnedCoins.Count; i++)
        {
            var coin = spawnedCoins[i];
            if (coin != null)
            {
                coin.transform.DOMove(coinTarget.position, animationDuration)
                    .SetEase(moveEase)
                    .OnComplete(() => DestroyCoin(coin));

                // Delay between each coin's animation
                yield return new WaitForSeconds(moveDelay);
            }
        }
    }

    private void DestroyCoin(GameObject coin)
    {
        spawnedCoins.Remove(coin);
        Destroy(coin);
    }
}