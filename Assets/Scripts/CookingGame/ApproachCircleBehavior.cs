using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachCircleBehavior : MonoBehaviour
{
    private GameProgressionManager GameProgressionManager;

    private float speed = 2000f;

    void Start()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();

        switch (GameProgressionManager.sceneNumber)
        {
            case 1:
                speed = 3f;
                break;

            case 2:
                speed = 4f;
                break;
        }
    }

    void Update()
    {
        transform.position += Vector3.down * 100f * speed * Time.deltaTime;
    }
}
