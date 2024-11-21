using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;


    [Header("Elements")]
    [SerializeField] private SkinDataSO skinData;
    [SerializeField] private Transform fruitsParent;
    [SerializeField] private LineRenderer fruitDropLine;
    private Fruit currentFruit;

    [Header("Settings")]
    [SerializeField] private float fruitsYSpawnPos;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int gameOverExplodeScore;
    [SerializeField] private int gameOverExplodeCoin;
    private bool canControl;
    private bool isControlling;
    private bool isExploding = false;

    [Header("Next Fruit Settings")]
    private int nextFruitIndex;

    [Header("Debug")]
    [SerializeField] private bool enableGizmos;

    [Header("Actions")]
    public static Action onNextFruitIndexSet;

    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private LineRendererFade lineRendererFade;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        MergeManager.onMergeProcessed += MergeProcessedCallback;
        ShopManager.onSkinSelected += SkinSelectedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;
        ShopManager.onSkinSelected -= SkinSelectedCallback;
    }

    private void Start()
    {
        SetNextFruitIndex();

        canControl = true;
        HideLine();
    }

    private void Update()
    {
        if (!GameManager.instance.IsGameState())
        {
            return;
        }

        if (GameManager.instance.IsGameState() && canControl && !isExploding)
        {
            ManagePlayerInput();
        }
    }

    public IEnumerator ExplodeFruits()
    {
        isExploding = true; // Set to true when the explosion process starts
        
        fruitDropLine.gameObject.SetActive(false);

        List<Fruit> fruits = new List<Fruit>(FindObjectsOfType<Fruit>());

        while (fruits.Count > 0)
        {
            Fruit fruitToDestroy = fruits[0];

            ScoreManager.instance.AddScore(gameOverExplodeScore); 
            CoinManager.instance.AddCoins(gameOverExplodeCoin);
            AudioManager.instance.PlayMergeSound(); 
            fruitToDestroy.Merge();
            fruits.RemoveAt(0);

            yield return new WaitForSeconds(0.10f);

            // update the list
            fruits = new List<Fruit>(FindObjectsOfType<Fruit>());
        }

        yield return new WaitForSeconds(1f);
        GameManager.instance.SetGameState(GameState.Gameover); // set GameState to GameOver
    }

    private void SkinSelectedCallback(SkinDataSO skinDataSO)
    {
        skinData = skinDataSO;
    }

    private void ManagePlayerInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MouseDownCallback();
        }

        else if (Input.GetMouseButton(0))
        {
            if (isControlling)
            {
                MouseDragCallback();
            }
            else
            {
                MouseDownCallback();
            }
        }

        else if(Input.GetMouseButtonUp(0) && isControlling)
        {
            MouseUpCallback();
        }
    }

    private void MouseDownCallback()
    {
        // Prevent spawning if the settings panel is active
        if (uiManager.IsSettingsPanelActive())
        {
            return;
        }


        if (!IsClickDetected())
        {
            return;
        }

        DisplayLine();

        PlaceLineAtClickedPosition();

        SpawnFruit();

        isControlling = true;
    }

    private void MouseDragCallback()
    {
        PlaceLineAtClickedPosition();

        //currentFruit.transform.position = GetSpawnPosition();
        currentFruit.MoveTo(new Vector2(GetSpawnPosition().x, fruitsYSpawnPos));
    }
    private void MouseUpCallback()
    {
        HideLine();

        if(currentFruit != null)
        {
            currentFruit.EnablePhysics();
        }

        canControl = false;

        StartControlTimer();
        
        isControlling = false;
    }

    private bool IsClickDetected()
    {
        Vector2 mousePos = Input.mousePosition;

        return mousePos.y > Screen.height / 6;
    }

    private void SpawnFruit()
    {
        Vector2 spawnPosition = GetSpawnPosition();
        Fruit toInstantiate = skinData.GetSpawnablePrefabs()[nextFruitIndex];

        currentFruit = Instantiate(
            toInstantiate,
            spawnPosition,
            Quaternion.identity,
            fruitsParent);

        //currentFruit.name = "Fruit_" + Random.Range(0, 1000);

        SetNextFruitIndex();
    }

    private void SetNextFruitIndex()
    {
        nextFruitIndex = Random.Range(0, skinData.GetSpawnablePrefabs().Length);
        onNextFruitIndexSet?.Invoke();
    }

    public string GetNextFruitName()
    {
        return skinData.GetSpawnablePrefabs()[nextFruitIndex].name;
    }

    public Sprite GetNextFruitSprite()
    {
        return skinData.GetSpawnablePrefabs()[nextFruitIndex].GetSprite();
    }

    public Fruit[] GetSmallFruits()
    {
        List<Fruit> smallFruits = new List<Fruit>();

        for (int i = 0; i < fruitsParent.childCount; i++)
        {
            Fruit fruit = fruitsParent.GetChild(i).GetComponent<Fruit>();

            int fruitTypeInt = (int)(fruit.GetFruitType());

            if (fruitTypeInt < 3)
            {
                smallFruits.Add(fruit);
            }
        }
        return smallFruits.ToArray();
    }

    private Vector2 GetClickedWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 worldClickedPosition = GetClickedWorldPosition();
        worldClickedPosition.y = fruitsYSpawnPos;
        return worldClickedPosition;
    }

    private void PlaceLineAtClickedPosition()
    {
        fruitDropLine.SetPosition(0, GetSpawnPosition());
        fruitDropLine.SetPosition(1, GetSpawnPosition() + Vector2.down * 5f);
    }

    private void HideLine()
    {
        fruitDropLine.enabled = false;
    }

    private void DisplayLine()
    {
        fruitDropLine.enabled = true;
        lineRendererFade.ApplyFade();
    }

    private void StartControlTimer()
    {
        Invoke("StopControlTimer", spawnDelay);
    }

    private void StopControlTimer()
    {
        canControl = true;
    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 spawnPosition)
    {
        for (int i = 0; i < skinData.GetObjectPrefabs().Length; i++)
        {
            if (skinData.GetObjectPrefabs()[i].GetFruitType() == fruitType)
            {
                SpawnMergedFruit(skinData.GetObjectPrefabs()[i], spawnPosition);
                break;
            }
        }
    }

    private void SpawnMergedFruit(Fruit fruit, Vector2 spawnPosition)
    {
        Fruit fruitInstance = Instantiate(fruit, spawnPosition, Quaternion.identity, fruitsParent);
        fruitInstance.EnablePhysics();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enableGizmos)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-50, fruitsYSpawnPos, 0), new Vector3(50, fruitsYSpawnPos, 0));
    }
#endif
}
