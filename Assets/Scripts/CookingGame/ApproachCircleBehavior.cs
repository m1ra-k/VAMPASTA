using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApproachCircleBehavior : MonoBehaviour
{
    private GameProgressionManager GameProgressionManager;

    private float speed = 2000f;
    private bool missed;    

    void Start()
    {
        GameProgressionManager = FindObjectOfType<GameProgressionManager>();

        switch (GameProgressionManager.sceneNumber)
        {
            case 1:
                speed = 3f;
                break;

            case 3:
                speed = 4f;
                break;

            case 5:
                speed = 5f;
                break;
        }
    }

    void Update()
    {
        transform.position += Vector3.down * 100f * speed * Time.deltaTime;

        if (transform.position.y < 120f && !missed)
        {
            missed = true;
            StartCoroutine(MissedApproachCircle());
        }
    }

    private IEnumerator MissedApproachCircle()
    {
        float duration = 0.1f;
        float elapsedTime = 0f;
        
        Image circle = GetComponent<Image>();

        Color startColor = circle.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            circle.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            yield return null;
        }

        circle.color = endColor;
    }
}
