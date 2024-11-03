using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFix : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform rightWall;
    [SerializeField] private Transform leftWall;

    private void Start()
    {
        FixWalls();
    }

    private void FixWalls()
    {
        float aspectRatio = (float)Screen.height / Screen.width;

        Camera mainCamera = Camera.main;

        float halfHorizontalFov = mainCamera.orthographicSize / aspectRatio;

        rightWall.transform.position = new Vector3(halfHorizontalFov + 0.15F, 0, 0);
        leftWall.transform.position = -rightWall.transform.position;
    }
}
